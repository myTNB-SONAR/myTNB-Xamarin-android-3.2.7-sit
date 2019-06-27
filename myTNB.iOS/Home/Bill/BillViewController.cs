using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Home.Bill;
using System.Globalization;
using myTNB.DataManager;
using Foundation;
using myTNB.Enums;
using System.Diagnostics;

namespace myTNB
{
    public partial class BillViewController : CustomBillUIViewController
    {
        UIView _headerView;

        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        PaymentHistoryResponseModel _paymentHistory = new PaymentHistoryResponseModel();
        BillHistoryResponseModel _billingHistory = new BillHistoryResponseModel();
        AccountSelectionComponent _accountSelectionComponent;
        TitleBarComponent titleBarComponent;
        DueAmountResponseModel _dueAmount = new DueAmountResponseModel();

        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();
        public bool IsFromNavigation;
        bool _paymentNeedsUpdate, isAnimating, isREAccount, isOwnedAccount, isFromReceiptScreen;
        bool isBcrmAvailable = true;

        public BillViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
            }
            SetSubviews();
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> BILLS LanguageDidChange");
            titleBarComponent?.SetTitle("Bill_Bills".Translate());
        }

        void HandleAppWillEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);

            if (topVc != null)
            {
                if (topVc is BillViewController)
                {
                    DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;
                    ViewWillAppear(true);
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!isFromReceiptScreen)
            {
                InitializeValues();
            }
            else
            {
                isFromReceiptScreen = false;
            }
        }

        void InitializeValues()
        {
            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount;
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;

            InitializedSubviews();
            titleBarComponent.SetBackVisibility(!IsFromNavigation);
            DataManager.DataManager.SharedInstance.selectedTag = 0;
            SetChargesValues(null, null, TNBGlobal.DEFAULT_VALUE);

            SetDetailsView();

            if (!isREAccount && isBcrmAvailable && !DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable
                && !DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable)
            {
                ShowToast();
            }

            if (!ServiceCall.HasAccountList())
            {
                _billingHistory = new BillHistoryResponseModel();
                _paymentHistory = new PaymentHistoryResponseModel();
                SetupContent();
                ToggleButtons();
            }
            else
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            if (ServiceCall.HasAccountList() && ServiceCall.HasSelectedAccount())
                            {
                                _paymentNeedsUpdate = true;
                                if (DataManager.DataManager.SharedInstance.IsBillUpdateNeeded)
                                {
                                    LoadBillingAccountDetails();
                                }
                                else
                                {
                                    DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;
                                    LoadBillHistory();
                                }
                            }
                            else
                            {
                                InitializeBillTableView();
                                SetupContent();
                                ToggleButtons();
                                SetButtonPayEnable();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No Network");
                            _billingHistory = new BillHistoryResponseModel();
                            _paymentHistory = new PaymentHistoryResponseModel();
                            InitializeBillTableView();
                            SetupContent();
                            ToggleButtons();
                            DisplayNoDataAlert();
                        }
                    });
                });
            }
            SetEvents();
        }

        /// <summary>
        /// Shows the toast.
        /// </summary>
        private void ShowToast()
        {
            var status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
            if (status != null && !string.IsNullOrEmpty(status?.DowntimeTextMessage))
            {
                View.BringSubviewToFront(toastView);
                ToastHelper.ShowToast(toastView, ref isAnimating, status?.DowntimeTextMessage);
            }
        }

        /// <summary>
        /// Handler for on done.
        /// </summary>
        public void OnDone()
        {
            isFromReceiptScreen = true;
        }

        void SetDetailsView()
        {
            if (_lblBreakdownHeader != null)
            {
                _lblBreakdownHeader.Text = (isREAccount ? "Bill_CurrentPaymentAdvice" : "Bill_BillDetails").Translate();
            }
            if (_lblTotalDueAmountTitle != null)
            {
                _lblTotalDueAmountTitle.Text = (isREAccount ? "Bill_MyEarnings" : "Common_TotalAmountDue").Translate();
            }
            if (_btnBills != null)
            {
                _btnBills.SetTitle((isREAccount ? "Bill_PaymentAdviceInfo" : "Bill_Bills").Translate(), UIControlState.Normal);
            }
        }

        void SetButtonPayEnable()
        {
            bool isEnabled = false;
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                isEnabled = true;//DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal > 0;
            }
            if (!ServiceCall.HasAccountList() || (!isBcrmAvailable)
                || (isBcrmAvailable && !DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable
                && !DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable))
            {
                isEnabled = false;
            }
            if (_btnPay != null)
            {
                _btnPay.Enabled = isEnabled;
                _btnPay.BackgroundColor = isEnabled ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
            }
        }

        void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 89, true);
            UIView headerView = gradientViewComponent.GetUI();
            titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Bill_Bills".Translate());
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(!IsFromNavigation);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                NavigationController?.PopViewController(true);
            }));

            headerView.AddSubview(titleBarView);

            _accountSelectionComponent = new AccountSelectionComponent(headerView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            headerView.AddSubview(accountSelectionView);

            View.AddSubview(headerView);
        }

        void ExecuteGetBillHistoryCall()
        {
            ActivityIndicator.Show();
            GetBillHistory().ContinueWith(task =>
            {
                if (_billingHistory?.d?.didSucceed == true && _billingHistory?.d?.data != null
                   && _billingHistory?.d?.data?.Count > 0)
                {
                    DataManager.DataManager.SharedInstance.SaveToBillHistory(_billingHistory.d, DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                }
                InvokeOnMainThread(DisplayBillHistory);
            });
        }

        /// <summary>
        /// Displaies the bill history.
        /// </summary>
        private void DisplayBillHistory()
        {
            InitializeBillTableView();
            SetupContent();
            ToggleButtons();
            SetButtonPayEnable();
            ActivityIndicator.Hide();
        }

        Task GetBillHistory()
        {
            var emailAddress = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
            }
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                    email = emailAddress
                };
                _billingHistory = serviceManager.GetBillHistory("GetBillHistory", requestParameter);
            });
        }

        Task GetPaymentHistory()
        {
            var emailAddress = string.Empty;
            if (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                emailAddress = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
            }
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter;

                if (!isREAccount)
                {
                    requestParameter = new
                    {
                        apiKeyID = TNBGlobal.API_KEY_ID,
                        accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,//220706336302
                        isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                        email = emailAddress
                    };
                }
                else
                {
                    requestParameter = new
                    {
                        ApiKeyID = TNBGlobal.API_KEY_ID,
                        AccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,//220706336302
                        IsOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned,
                        Email = emailAddress
                    };
                }

                _paymentHistory = serviceManager.GetPaymentHistory(!isREAccount ? "GetPaymentHistory" : "GetREPaymentHistory", requestParameter);
            });
        }

        void SetupContent()
        {
            _accountSelectionComponent?.SetAccountName(DataManager.DataManager.SharedInstance.SelectedAccount?.accDesc);
            if (IsFromNavigation)
            {
                _accountSelectionComponent?.SetDropdownVisibility(IsFromNavigation);
            }
            else
            {
                _accountSelectionComponent?.SetDropdownVisibility(false);//ServiceCall.GetAccountListCount() > 1 ? IsFromNavigation : true);
            }
            _accountSelectionComponent?.SetLeafVisibility(!isREAccount);

            _lblAccountName.Text = DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex < DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count
                ? DataManager.DataManager.SharedInstance.AccountRecordsList?.d[DataManager.DataManager.SharedInstance.CurrentSelectedAccountIndex]?.ownerName
                : string.Empty;
            _lblAccountNumber.Text = DataManager.DataManager.SharedInstance.SelectedAccount?.accNum;

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        _lblAddress.Text = DataManager.DataManager.SharedInstance.SelectedAccount?.accountStAddress;
                        RefitAccountDetailsToWidget();
                        _headerView.Frame = GetHeaderFrame();
                        await LoadAmountDue();
                        var currentAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCurrentChg ?? 0;
                        var outstandingAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amOutstandingChg ?? 0;
                        var payableAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amPayableChg ?? 0;
                        var balanceAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCustBal ?? 0;

                        var current = isREAccount ? ChartHelper.UpdateValueForRE(currentAmt) : currentAmt;
                        var outstanding = isREAccount ? ChartHelper.UpdateValueForRE(outstandingAmt) : outstandingAmt;
                        var payable = isREAccount ? ChartHelper.UpdateValueForRE(payableAmt) : payableAmt;
                        var balance = isREAccount ? ChartHelper.UpdateValueForRE(balanceAmt) : balanceAmt;

                        SetChargesValues(string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, current.ToString("N2", CultureInfo.InvariantCulture))
                            , string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, outstanding.ToString("N2", CultureInfo.InvariantCulture))
                            , balance.ToString("N2", CultureInfo.InvariantCulture));

                        if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null && _billingAccountDetailsList?.d?.didSucceed == true
                            && _billingAccountDetailsList?.d?.data != null && _billingAccountDetailsList.d.data.IsItemisedBilling)
                        {
                            AddItemisedBillingDetails(_billingAccountDetailsList.d.data, ItemisedBillingTooltipAction);
                            _headerView.Frame = GetHeaderFrame();
                            billTableView.ReloadData();
                        }
                    }
                    else
                    {
                        _lblAddress.Text = string.Empty;
                        SetChargesValues(string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE)
                            , string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE)
                            , TNBGlobal.DEFAULT_VALUE);
                        DisplayNoDataAlert();
                    }
                });
            });


            _lblHistoryHeader.Text = isREAccount ? "Bill_REPaymentSectionHeader".Translate() : "Bill_PaymentSectionHeader".Translate();
        }
        /// <summary>
        /// Loads the amount due of the selected account.
        /// </summary>
        private async Task LoadAmountDue()
        {
            var due = DataManager.DataManager.SharedInstance.GetDue(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
            string _dateDue;
            double _amountDue;
            double _dueIncrementDays;

            await GetBillingAccountDetails().ContinueWith(task =>
             {
                 InvokeOnMainThread(() =>
                 {
                     if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                         && _billingAccountDetailsList?.d?.data != null)
                     {
                         var billDetails = _billingAccountDetailsList.d.data;
                         DataManager.DataManager.SharedInstance.BillingAccountDetails = billDetails;
                         if (!isREAccount)
                         {
                             DataManager.DataManager.SharedInstance.SaveToBillingAccounts(billDetails, billDetails.accNum);
                         }
                     }
                 });
             });
            await GetAccountDueAmount().ContinueWith(dueTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_dueAmount != null && _dueAmount?.d != null
                        && _dueAmount?.d?.didSucceed == true)
                    {
                        _amountDue = _dueAmount.d.data.amountDue;
                        _dateDue = _dueAmount.d.data.billDueDate;
                        _dueIncrementDays = _dueAmount.d.data.IncrementREDueDateByDays;
                        SetAmountInBillingDetails(_amountDue);
                        SaveDueToCache(_dueAmount.d.data);
                        SetBillAndPaymentDetails(_dateDue, _dueIncrementDays);
                    }
                });
            });
        }
        /// <summary>
        /// Gets the account due amount if no cached data
        /// </summary>
        /// <returns>The account due amount.</returns>
        internal Task GetAccountDueAmount()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _dueAmount = serviceManager.GetAccountDueAmount("GetAccountDueAmount", requestParameter);
            });
        }
        /// <summary>
        /// Sets the amount in billing details in the cache
        /// </summary>
        /// <param name="amount">Amount.</param>
        void SetAmountInBillingDetails(double amount)
        {
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                DataManager.DataManager.SharedInstance
                           .BillingAccountDetails.amCustBal = amount;
            }
        }
        /// <summary>
        /// Saves the due to cache.
        /// </summary>
        /// <param name="model">Model.</param>
        private void SaveDueToCache(DueAmountDataModel model)
        {
            var acct = DataManager.DataManager.SharedInstance.SelectedAccount;
            var item = new DueAmountDataModel
            {
                accNum = acct.accNum,
                accNickName = acct.accountNickName,
                IsReAccount = acct.IsREAccount,
                amountDue = model.amountDue,
                billDueDate = model.billDueDate,
                IncrementREDueDateByDays = model.IncrementREDueDateByDays
            };
            DataManager.DataManager.SharedInstance.SaveDue(item);
        }
        /// <summary>
        /// Sets the bill and payment details.
        /// </summary>
        /// <param name="dateDue">Date due.</param>
        /// <param name="incrementDays">Increment days.</param>
        internal void SetBillAndPaymentDetails(string dateDue, double incrementDays)
        {
            var balanceAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCustBal ?? 0;
            var balance = !isREAccount ? balanceAmt : ChartHelper.UpdateValueForRE(balanceAmt);
            string dateString = (!string.IsNullOrEmpty(dateDue) && balance > 0) ? dateDue : string.Empty;

            string formattedDate = string.Empty;
            string prefix = string.Empty;
            if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A")
               || (DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter && !isBcrmAvailable))
            {
                formattedDate = TNBGlobal.EMPTY_DATE;
            }
            else
            {
                if (isREAccount && incrementDays > 0)
                {
                    try
                    {
                        var format = @"dd/MM/yyyy";
                        DateTime dueD = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                        dueD = dueD.AddDays(incrementDays);
                        dateString = dueD.ToString(format);
                    }
                    catch (FormatException)
                    {
                        Debug.WriteLine("Unable to parse '{0}'", dateString);
                    }
                }
                formattedDate = DateHelper.GetFormattedDate(dateString, "dd MMM yyyy");
                prefix = isREAccount ? string.Format("{0} ", "Bill_By".Translate()) : string.Empty;
            }

            string dueDate = prefix + formattedDate;

            _lblDueDateTitle.Text = dueDate;
        }

        void InitializeBillTableView()
        {
            billTableView.Source = new BillTableViewDataSource(_billingHistory
                , _paymentHistory, this, NetworkUtility.isReachable, isREAccount, isOwnedAccount);
            billTableView.ReloadData();
        }

        void InitializedSubviews()
        {
            _headerView = new UIView()
            {
                BackgroundColor = UIColor.White,
            };
            billTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            billTableView.TableHeaderView = _headerView;

            if (isREAccount)
            {
                CreateREView();
            }
            else
            {
                CreateNormalView();
            }
            _headerView.AddSubviews(new UIView[] { _viewAccountDetails, _viewCharges, _viewHistory });
            _lblHistoryHeader.Text = isREAccount ? "Bill_REPaymentSectionHeader".Translate() : "Bill_PaymentSectionHeader".Translate();
            _headerView.Frame = GetHeaderFrame();
            if (_btnPay != null)
            {
                _btnPay.Enabled = false;
                _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
            }
        }

        void SetEvents()
        {
            if (_accountSelectionComponent != null && !IsFromNavigation)
            {
                if (ServiceCall.GetAccountListCount() > 0)
                {
                    UITapGestureRecognizer accountSelectionGesture = new UITapGestureRecognizer(() =>
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        SelectAccountTableViewController viewController =
                            storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    });
                    _accountSelectionComponent.SetSelectAccountEvent(accountSelectionGesture);
                }
            }

            _btnBills.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.selectedTag = 0;
                ToggleButtons();
            };

            _btnPayments.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.selectedTag = 1;
                ToggleButtons();
            };
            if (_btnPay != null)
            {
                _btnPay.TouchUpInside += (sender, e) =>
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                                SelectBillsViewController selectBillsVC =
                                    storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                                if (selectBillsVC != null)
                                {
                                    selectBillsVC.SelectedAccountDueAmount = DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal;
                                    var navController = new UINavigationController(selectBillsVC);
                                    PresentViewController(navController, true, null);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("No Network");
                                DisplayNoDataAlert();
                            }
                        });
                    });
                };
            }
        }

        void ToggleButtons()
        {
            bool isBillSelected = DataManager.DataManager.SharedInstance.selectedTag == 0;
            _btnBills.SetTitleColor(isBillSelected ? MyTNBColor.PowerBlue : UIColor.LightGray, UIControlState.Normal);
            _btnBills.BackgroundColor = isBillSelected ? UIColor.White : MyTNBColor.SelectionGrey;
            _btnPayments.SetTitleColor(isBillSelected ? UIColor.LightGray : MyTNBColor.PowerBlue, UIControlState.Normal);
            _btnPayments.BackgroundColor = isBillSelected ? MyTNBColor.SelectionGrey : UIColor.White;

            if (_paymentNeedsUpdate && !isBillSelected)
            {
                LoadPaymentHistory();
            }
            else
            {
                InitializeBillTableView();
            }
        }

        internal void ViewReceipt(string merchantTransactionID)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                        ReceiptViewController viewController =
                            storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                        if (viewController != null)
                        {
                            viewController.MerchatTransactionID = merchantTransactionID;//"MYTN201801041414";//
                            viewController.OnDone = OnDone;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No Network");
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        /// <summary>
        /// Loads the payment history.
        /// </summary>
        private void LoadPaymentHistory()
        {
            var cachedDetails = DataManager.DataManager.SharedInstance.GetCachedPaymentHistory(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);

            if (cachedDetails != null)
            {
                _paymentHistory = new PaymentHistoryResponseModel();
                _paymentHistory.d = cachedDetails;
                DisplayPaymentHistory();
            }
            else
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            ActivityIndicator.Show();
                            GetPaymentHistory().ContinueWith(task =>
                            {
                                if (_paymentHistory?.d?.didSucceed == true && _paymentHistory?.d?.data?.Count > 0)
                                {
                                    DataManager.DataManager.SharedInstance.SaveToPaymentHistory(_paymentHistory.d, DataManager.DataManager.SharedInstance.SelectedAccount.accNum);
                                }
                                InvokeOnMainThread(DisplayPaymentHistory);
                            });
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                    });
                });
            }

        }

        /// <summary>
        /// Displays the payment history.
        /// </summary>
        private void DisplayPaymentHistory()
        {
            _paymentNeedsUpdate = false;
            InitializeBillTableView();
            ActivityIndicator.Hide();
        }

        /// <summary>
        /// Loads the billing account details.
        /// </summary>
        private void LoadBillingAccountDetails()
        {
            if (isREAccount)
            {
                ExecuteGetBillAccountDetailsCall();
            }
            else
            {
                var cachedDetails = DataManager.DataManager.SharedInstance.GetCachedBillingAccountDetails(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);

                if (cachedDetails != null)
                {
                    DataManager.DataManager.SharedInstance.BillingAccountDetails = cachedDetails;
                    LoadBillHistory();
                }
                else
                {
                    ExecuteGetBillAccountDetailsCall();
                }
            }
        }

        /// <summary>
        /// Loads the bill history.
        /// </summary>
        private void LoadBillHistory()
        {
            var cachedDetails = DataManager.DataManager.SharedInstance.GetCachedBillHistory(DataManager.DataManager.SharedInstance.SelectedAccount.accNum);

            if (cachedDetails != null)
            {
                _billingHistory = new BillHistoryResponseModel();
                _billingHistory.d = cachedDetails;
                DisplayBillHistory();
            }
            else
            {
                ExecuteGetBillHistoryCall();
            }
        }

        void ExecuteGetBillAccountDetailsCall()
        {
            ActivityIndicator.Show();
            GetBillingAccountDetails().ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_billingAccountDetailsList != null && _billingAccountDetailsList?.d != null
                        && _billingAccountDetailsList?.d?.data != null)
                    {
                        var billDetails = _billingAccountDetailsList.d.data;
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = billDetails;
                        if (!isREAccount)
                        {
                            DataManager.DataManager.SharedInstance.SaveToBillingAccounts(billDetails, billDetails.accNum);
                        }
                        LoadBillHistory();
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        DataManager.DataManager.SharedInstance.BillingAccountDetails = new BillingAccountDetailsDataModel();
                        DisplayServiceError(_billingAccountDetailsList?.d?.message);
                        ActivityIndicator.Hide();
                    }
                });
            });
        }

        Task GetBillingAccountDetails()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    CANum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                };
                _billingAccountDetailsList = serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });
        }

        private void ItemisedBillingTooltipAction()
        {
            string title = _billingAccountDetailsList.d.data.WhatIsThisTitle ?? "Bill_WhatIsThisTitle".Translate();
            string msg = _billingAccountDetailsList.d.data.WhatIsThisMessage ?? "Bill_WhatIsThisMessage".Translate();
            string btnText = _billingAccountDetailsList.d.data.WhatIsThisButtonText ?? "Bill_WhatIsThisButtonText".Translate();

            DisplayCustomAlert(title, msg, btnText);
        }
    }
}
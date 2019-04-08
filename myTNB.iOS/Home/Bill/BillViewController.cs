using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using System.Threading.Tasks;
using myTNB.Model;
using myTNB.Home.Bill;
using System.Drawing;
using CoreGraphics;
using CoreAnimation;
using System.Globalization;
using myTNB.DataManager;
using Foundation;
using myTNB.Extensions;
using myTNB.Enums;
using System.Diagnostics;

namespace myTNB
{
    public partial class BillViewController : UIViewController
    {
        UILabel _lblAmount;
        UIView _viewAmount;

        UIView _headerView;
        UIView _historySelectionView;
        UIView _currentBillDetailsView;
        UIView _currentBillHeaderView;
        UIView _historyHeaderView;
        UIButton _btnPayment;
        UIButton _btnBills;
        UIButton _btnPay;

        UILabel _lblAccountName;
        UILabel _lblAccountNumber;
        UILabel _lblViewAddress;
        UILabel _lblCurrentChargesTitle;
        UILabel _lblCurrentChargesValue;
        UILabel _lblOutstandingChargesTitle;
        UILabel _lblOutstandingChargesValue;
        UILabel _lblTotalPayableTitle;
        UILabel _lblTotalPayableValue;
        UILabel _lblHistoryHeader;

        UILabel _lblCurrentBillHeader;
        UILabel _lblTotalDueAmountTitle;
        UILabel _lblDueDateTitle;
        UIView _lineView;
        UIView _viewCharges;

        UIImageView _imgLeaf;
        UIRefreshControl refreshControl;

        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();
        PaymentHistoryResponseModel _paymentHistory = new PaymentHistoryResponseModel();
        BillHistoryResponseModel _billingHistory = new BillHistoryResponseModel();
        AccountSelectionComponent _accountSelectionComponent;
        TitleBarComponent titleBarComponent;
        DueAmountResponseModel _dueAmount = new DueAmountResponseModel();

        public UserNotificationDataModel NotificationInfo = new UserNotificationDataModel();
        public bool IsFromNavigation = false;
        bool _paymentNeedsUpdate = false;
        bool isAnimating = false;
        bool isREAccount = false;
        bool isOwnedAccount = false;
        bool isBcrmAvailable = true;
        bool isFromReceiptScreen = false;
        nfloat headerMarginY = 15.0f;
        bool isRefreshing = false;

        public BillViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
            }
            SetSubviews();
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            refreshControl = new UIRefreshControl();
        }

        void HandleAppWillEnterForeground(NSNotification notification)
        {
            Debug.WriteLine("HandleAppWillEnterForeground");
            DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;
            ViewWillAppear(true);
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
            InitializedSubviews();
            titleBarComponent.SetBackVisibility(!IsFromNavigation);
            DataManager.DataManager.SharedInstance.selectedTag = 0;
            if (_lblAmount != null)
            {
                _lblAmount.Text = TNBGlobal.DEFAULT_VALUE;
            }

            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount;
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;

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
                InitializeBillTableView();
                SetupContent();
                AdjustFrames();
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
                            //ActivityIndicator.Show();
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
                                AdjustFrames();
                                ToggleButtons();
                                SetButtonPayEnable();
                                //ActivityIndicator.Hide();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No Network");
                            _billingHistory = new BillHistoryResponseModel();
                            _paymentHistory = new PaymentHistoryResponseModel();
                            InitializeBillTableView();
                            SetupContent();
                            AdjustFrames();
                            ToggleButtons();
                            ErrorHandler.DisplayNoDataAlert(this);
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
            if (_lblCurrentBillHeader != null)
            {
                _lblCurrentBillHeader.Text = isREAccount ? "Bill_CurrentPaymentAdvice".Translate() : "Bill_CurrentBill".Translate();
            }
            if (_lblTotalDueAmountTitle != null)
            {
                _lblTotalDueAmountTitle.Text = isREAccount ? "Bill_PaymentAdviceAmount".Translate() : "Common_TotalAmountDue".Translate();
            }

            if (isREAccount)
            {
                if (_btnPay != null)
                {
                    _btnPay.RemoveFromSuperview();
                }
                //_viewCharges.RemoveFromSuperview();
#if true
                _viewCharges.Hidden = true;
#else
                _lblCurrentChargesTitle.Text = "Bill_RECurrentAmount".Translate();
                _lblOutstandingChargesTitle.Text = "Bill_REOutstandingAmount".Translate();

#endif
                _lblTotalDueAmountTitle.Text = "Bill_MyEarnings".Translate();
                _btnBills.SetTitle("Bill_PaymentAdviceInfo".Translate(), UIControlState.Normal);
                _lblTotalPayableTitle.Hidden = true;
                _lblTotalPayableValue.Hidden = true;

                AdjustFrameSetY(_lblTotalDueAmountTitle, _viewCharges.Frame.Y + headerMarginY + 3);
                AdjustFrameSetY(_lblDueDateTitle, _lblTotalDueAmountTitle.Frame.GetMaxY() + 3);
                AdjustFrameSetY(_viewAmount, _viewCharges.Frame.Y + headerMarginY);
                //AdjustFrameHeight(_viewCharges, adjHeight * -1.0f);
                //AdjustFrameSetY(_lineView, _lblOutstandingChargesTitle.Frame.GetMaxY() + headerMarginY);
                //AdjustFrameSetY(_lblTotalDueAmountTitle, _viewCharges.Frame.GetMaxY() + headerMarginY + 6);
                //AdjustFrameSetY(_viewAmount, _viewCharges.Frame.GetMaxY() + headerMarginY);
                //_lblTotalDueAmountTitle.Frame = new CGRect(18, 20, 160, 18);
                //_viewAmount.Frame = new CGRect(View.Frame.Width - 120, 14, 0, 24);
                //_headerView.Frame = new RectangleF(0, 0, (float)View.Frame.Width, 393);
                //_btnPay.Frame = new CGRect(18, 54, View.Frame.Width - 36, 48);
                //_historyHeaderView.Frame = new RectangleF(0, 297, (float)View.Frame.Width, 48);

                var adjY = (_viewCharges.Frame.Height + _btnPay.Frame.Height + headerMarginY * 2) * -1.0f;
                AdjustFrameHeight(_currentBillDetailsView, adjY);
                AdjustFrameAddY(_historyHeaderView, adjY);
                AdjustFrameAddY(_historySelectionView, adjY);
                AdjustFrameHeight(_headerView, adjY);
                billTableView.TableHeaderView = _headerView;

            }
            else
            {
                //_currentBillDetailsView.AddSubview(_viewCharges);
                _lblCurrentChargesTitle.Text = "Bill_CurrentCharges".Translate();
                _lblOutstandingChargesTitle.Text = "Bill_OutstandingCharges".Translate();
                _lblTotalDueAmountTitle.Text = "Common_TotalAmountDue".Translate();
                _btnBills.SetTitle("Bill_Bills".Translate(), UIControlState.Normal);
                _viewCharges.Hidden = false;
                _lblTotalPayableTitle.Hidden = false;
                _lblTotalPayableValue.Hidden = false;
                //AdjustFrameHeight(_viewCharges, adjHeight);
                //AdjustFrameSetY(_lineView, _lblTotalPayableValue.Frame.GetMaxY() + headerMarginY);
                AdjustFrameSetY(_lblTotalDueAmountTitle, _viewCharges.Frame.GetMaxY() + headerMarginY * 2 + 3);
                AdjustFrameSetY(_viewAmount, _viewCharges.Frame.GetMaxY() + headerMarginY * 2);
                //_lblTotalDueAmountTitle.Frame = new CGRect(18, 136, 160, 18);
                //_viewAmount.Frame = new CGRect(View.Frame.Width - 120, 130, 0, 24);
                //_headerView.Frame = new RectangleF(0, 0, (float)View.Frame.Width, 509);
                _btnPay.Frame = new CGRect(18, 180, View.Frame.Width - 36, 48);
                _historyHeaderView.Frame = new RectangleF(0, 413, (float)View.Frame.Width, 48);

                _headerView.Frame = new CGRect(0, 0, View.Frame.Width, 509);
                _currentBillDetailsView.Frame = new RectangleF(0, 170, (float)View.Frame.Width, 218);
                _historyHeaderView.Frame = new RectangleF(0, 413, (float)View.Frame.Width, 48);

                if (_btnPay != null)
                {
                    _btnPay.RemoveFromSuperview();
                }

                _currentBillDetailsView.AddSubview(_btnPay);
            }
        }

        void SetButtonPayEnable()
        {
            bool isEnabled = false;
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                isEnabled = true;//DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal > 0;
            }
            if (!ServiceCall.HasAccountList()
                || (!isBcrmAvailable)
                || (isBcrmAvailable && !DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable
                   && !DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable))
            {
                isEnabled = false;
            }
            _btnPay.Enabled = isEnabled;
            _btnPay.BackgroundColor = isEnabled ? myTNBColor.FreshGreen() : myTNBColor.SilverChalice();
        }

        void SetSubviews()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 89, true);
            UIView headerView = gradientViewComponent.GetUI();
            titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Bill_Bills".Translate());
            titleBarComponent.SetNotificationVisibility(true);

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
            AdjustFrames();
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

            if (NetworkUtility.isReachable)
            {
                _lblViewAddress.Text = DataManager.DataManager.SharedInstance.SelectedAccount?.accountStAddress;

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(async () =>
                    {
                        await LoadAmountDue();
                        var currentAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCurrentChg ?? 0;
                        var outstandingAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amOutstandingChg ?? 0;
                        var payableAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amPayableChg ?? 0;
                        var balanceAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCustBal ?? 0;

                        var current = !isREAccount ? currentAmt : ChartHelper.UpdateValueForRE(currentAmt);
                        var outstanding = !isREAccount ? outstandingAmt : ChartHelper.UpdateValueForRE(outstandingAmt);
                        var payable = !isREAccount ? payableAmt : ChartHelper.UpdateValueForRE(payableAmt);
                        var balance = !isREAccount ? balanceAmt : ChartHelper.UpdateValueForRE(balanceAmt);

                        _lblCurrentChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, current.ToString("N2", CultureInfo.InvariantCulture));
                        _lblOutstandingChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, outstanding.ToString("N2", CultureInfo.InvariantCulture));
                        _lblTotalPayableValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, payable.ToString("N2", CultureInfo.InvariantCulture));
                        _lblAmount.Text = balance.ToString("N2", CultureInfo.InvariantCulture);
                        AdjustFrames();
                    });
                });
            }
            else
            {
                _lblViewAddress.Text = string.Empty;
                _lblCurrentChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
                _lblOutstandingChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
                _lblTotalPayableValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, TNBGlobal.DEFAULT_VALUE);
                _lblAmount.Text = TNBGlobal.DEFAULT_VALUE;
            }

            _imgLeaf.Hidden = !isREAccount;
            _lblHistoryHeader.Text = isREAccount ? "Bill_REPaymentSectionHeader".Translate() : "Bill_PaymentSectionHeader".Translate();

            //if (!isOwnedAccount)
            //{
            //    ShowNonOwnerView();
            //}
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

            if (due != null)
            {
                _amountDue = due.amountDue;
                _dateDue = due.billDueDate;
                _dueIncrementDays = due.IncrementREDueDateByDays;
                SetAmountInBillingDetails(_amountDue);
                SetBillAndPaymentDetails(_dateDue, _dueIncrementDays);
            }
            else
            {
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
                        DateTime dueD = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
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
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    billTableView.Source = new BillTableViewDataSource(_billingHistory
                                                                       , _paymentHistory
                                                                       , this
                                                                       , NetworkUtility.isReachable
                                                                       , isREAccount
                                                                       , isOwnedAccount);
                    refreshControl.ValueChanged += PullDownTorefresh;
                    //billTableView.AddSubview(refreshControl); removed pull down to refresh
                    billTableView.ReloadData();
                });
            });
        }

        void InitializedSubviews()
        {
            //BillTableView Header
            _headerView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 509));
            _headerView.BackgroundColor = UIColor.White;
            billTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            billTableView.TableHeaderView = _headerView;

            //Acount Details
            var accountDetailsView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 122));
            accountDetailsView.BackgroundColor = UIColor.White;
            _headerView.AddSubview(accountDetailsView);

            _lblAccountName = new UILabel(new CGRect(18, 16, View.Frame.Width - 36, 18));
            _lblAccountName.Font = myTNBFont.MuseoSans14_500();
            _lblAccountName.TextAlignment = UITextAlignment.Left;
            _lblAccountName.TextColor = myTNBColor.TunaGrey();
            _lblAccountName.BackgroundColor = UIColor.Clear;
            accountDetailsView.AddSubview(_lblAccountName);

            _lblAccountNumber = new UILabel(new CGRect(18, 34, View.Frame.Width - 36, 16));
            _lblAccountNumber.Font = myTNBFont.MuseoSans12_300();
            _lblAccountNumber.TextAlignment = UITextAlignment.Left;
            _lblAccountNumber.TextColor = myTNBColor.TunaGrey();
            _lblAccountNumber.BackgroundColor = UIColor.Clear;
            accountDetailsView.AddSubview(_lblAccountNumber);

            _lblViewAddress = new UILabel(new CGRect(18, 50, View.Frame.Width - 36, 72));
            _lblViewAddress.Font = myTNBFont.MuseoSans12_300();
            _lblViewAddress.TextAlignment = UITextAlignment.Left;
            _lblViewAddress.Lines = 0;
            _lblViewAddress.LineBreakMode = UILineBreakMode.TailTruncation;
            _lblViewAddress.TextColor = myTNBColor.TunaGrey();
            _lblViewAddress.BackgroundColor = UIColor.Clear;
            _lblViewAddress.UserInteractionEnabled = false;
            accountDetailsView.AddSubview(_lblViewAddress);

            //RE Account Leaf
            _imgLeaf = new UIImageView(new CGRect(View.Frame.Width - 42, 16, 24, 24));
            _imgLeaf.Image = UIImage.FromBundle("IC-RE-Leaf-Green");
            _imgLeaf.Hidden = true;
            accountDetailsView.AddSubview(_imgLeaf);

            //Current Bill Header
            _currentBillHeaderView = new UIView(new RectangleF(0, 122, (float)View.Frame.Width, 48));
            _currentBillHeaderView.BackgroundColor = myTNBColor.SectionGrey();
            _headerView.AddSubview(_currentBillHeaderView);

            _lblCurrentBillHeader = new UILabel(new CGRect(18, 24, View.Frame.Width - 36, 18));
            _lblCurrentBillHeader.Font = myTNBFont.MuseoSans16();
            _lblCurrentBillHeader.TextAlignment = UITextAlignment.Left;
            _lblCurrentBillHeader.Text = "Bill_CurrentBill".Translate();
            _lblCurrentBillHeader.TextColor = myTNBColor.PowerBlue();
            _lblCurrentBillHeader.BackgroundColor = UIColor.Clear;
            _currentBillHeaderView.AddSubview(_lblCurrentBillHeader);

            //Current Bill Details
            _currentBillDetailsView = new UIView(new RectangleF(0, 170, (float)View.Frame.Width, 218));
            _currentBillDetailsView.BackgroundColor = UIColor.White;
            _headerView.AddSubview(_currentBillDetailsView);

            _viewCharges = new UIView(new CGRect(0, 0, View.Frame.Width, 113 - headerMarginY));
            _viewCharges.BackgroundColor = UIColor.White;
            _currentBillDetailsView.AddSubview(_viewCharges);

            #region _viewCharges
            _lblCurrentChargesTitle = new UILabel(new CGRect(18, 16, 119, 16));
            _lblCurrentChargesTitle.Font = myTNBFont.MuseoSans12();
            _lblCurrentChargesTitle.TextAlignment = UITextAlignment.Left;
            _lblCurrentChargesTitle.Text = "Bill_CurrentCharges".ToUpper();
            _lblCurrentChargesTitle.TextColor = myTNBColor.TunaGrey();
            _lblCurrentChargesTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblCurrentChargesTitle);

            _lblCurrentChargesValue = new UILabel(new CGRect(137, 16, View.Frame.Width - 155, 16));
            _lblCurrentChargesValue.Font = myTNBFont.MuseoSans12();
            _lblCurrentChargesValue.TextAlignment = UITextAlignment.Right;
            _lblCurrentChargesValue.TextColor = myTNBColor.TunaGrey();
            _lblCurrentChargesValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblCurrentChargesValue);

            _lblOutstandingChargesTitle = new UILabel(new CGRect(18, 48, 119, 16));
            _lblOutstandingChargesTitle.Font = myTNBFont.MuseoSans12();
            _lblOutstandingChargesTitle.TextAlignment = UITextAlignment.Left;
            _lblOutstandingChargesTitle.Text = "Bill_OutstandingCharges".ToUpper();
            _lblOutstandingChargesTitle.TextColor = myTNBColor.TunaGrey();
            _lblOutstandingChargesTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblOutstandingChargesTitle);

            _lblOutstandingChargesValue = new UILabel(new CGRect(137, 48, View.Frame.Width - 155, 16));
            _lblOutstandingChargesValue.Font = myTNBFont.MuseoSans12();
            _lblOutstandingChargesValue.TextAlignment = UITextAlignment.Right;
            _lblOutstandingChargesValue.TextColor = myTNBColor.TunaGrey();
            _lblOutstandingChargesValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblOutstandingChargesValue);

            _lblTotalPayableTitle = new UILabel(new CGRect(18, _lblOutstandingChargesTitle.Frame.GetMaxY() + headerMarginY, 119, 16));
            _lblTotalPayableTitle.Font = myTNBFont.MuseoSans12();
            _lblTotalPayableTitle.TextAlignment = UITextAlignment.Left;
            _lblTotalPayableTitle.Text = "Bill_TotalPayable".Translate();
            _lblTotalPayableTitle.TextColor = myTNBColor.TunaGrey();
            _lblTotalPayableTitle.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblTotalPayableTitle);

            _lblTotalPayableValue = new UILabel(new CGRect(137, _lblOutstandingChargesTitle.Frame.GetMaxY() + headerMarginY, View.Frame.Width - 155, 16));
            _lblTotalPayableValue.Font = myTNBFont.MuseoSans12();
            _lblTotalPayableValue.TextAlignment = UITextAlignment.Right;
            //_lblTotalPayableValue.Text = TNBGlobal.DEFAULT_VALUE;
            _lblTotalPayableValue.TextColor = myTNBColor.TunaGrey();
            _lblTotalPayableValue.BackgroundColor = UIColor.Clear;
            _viewCharges.AddSubview(_lblTotalPayableValue);

            _lineView = new UIView(new CGRect(18, _lblTotalPayableValue.Frame.GetMaxY() + headerMarginY, (float)View.Frame.Width - 36, 1));
            _lineView.BackgroundColor = UIColor.LightGray;
            _viewCharges.AddSubview(_lineView);
            #endregion

            _lblTotalDueAmountTitle = new UILabel(new CGRect(18, _viewCharges.Frame.GetMaxY() + headerMarginY + 3, 160, 18));
            _lblTotalDueAmountTitle.Font = myTNBFont.MuseoSans14_500();
            _lblTotalDueAmountTitle.TextAlignment = UITextAlignment.Left;
            _lblTotalDueAmountTitle.Text = "Common_TotalAmountDue".Translate();
            _lblTotalDueAmountTitle.TextColor = myTNBColor.TunaGrey();
            _lblTotalDueAmountTitle.BackgroundColor = UIColor.Clear;
            _currentBillDetailsView.AddSubview(_lblTotalDueAmountTitle);

            _lblDueDateTitle = new UILabel(new CGRect(18, _lblTotalDueAmountTitle.Frame.GetMaxY() + 16, _currentBillDetailsView.Frame.Width - 20, 14));
            _lblDueDateTitle.Font = myTNBFont.MuseoSans11_300();
            _lblDueDateTitle.TextColor = myTNBColor.SilverChalice();
            _lblDueDateTitle.TextAlignment = UITextAlignment.Left;
            _currentBillDetailsView.AddSubview(_lblDueDateTitle);

            _viewAmount = new UIView(new CGRect(View.Frame.Width - 120, _viewCharges.Frame.GetMaxY() + headerMarginY, 0, 24));
            var lblCurrency = new UILabel(new CGRect(0, 6, 24, 18));
            lblCurrency.Font = myTNBFont.MuseoSans14();
            lblCurrency.TextColor = myTNBColor.TunaGrey();
            lblCurrency.TextAlignment = UITextAlignment.Right;
            lblCurrency.Text = string.Format("{0} ", TNBGlobal.UNIT_CURRENCY);
            _viewAmount.BackgroundColor = UIColor.Clear;
            _viewAmount.AddSubview(lblCurrency);

            _lblAmount = new UILabel(new CGRect(24, 0, 75, 24));
            _lblAmount.Font = myTNBFont.MuseoSans24();
            _lblAmount.TextColor = myTNBColor.TunaGrey();
            _lblAmount.TextAlignment = UITextAlignment.Right;
            //_lblAmount.Text = TNBGlobal.DEFAULT_VALUE;
            _lblAmount.BackgroundColor = UIColor.Clear;
            _viewAmount.AddSubview(_lblAmount);

            _currentBillDetailsView.AddSubview(_viewAmount);

            _btnPay = new UIButton(UIButtonType.Custom);
            _btnPay.Frame = new CGRect(18, 180, View.Frame.Width - 36, 48);
            _btnPay.SetTitle("Bill_Pay".Translate(), UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnPay.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnPay.BackgroundColor = myTNBColor.SilverChalice();
            _btnPay.Layer.CornerRadius = 4.0f;
            _btnPay.Enabled = false;
            _currentBillDetailsView.AddSubview(_btnPay);

            //Bills and Payment History Header
            _historyHeaderView = new UIView(new RectangleF(0, 413, (float)View.Frame.Width, 48));
            _historyHeaderView.BackgroundColor = myTNBColor.SectionGrey();
            _headerView.AddSubview(_historyHeaderView);

            _lblHistoryHeader = new UILabel(new CGRect(18, 24, View.Frame.Width - 36, 18));
            _lblHistoryHeader.Font = myTNBFont.MuseoSans16();
            _lblHistoryHeader.TextAlignment = UITextAlignment.Left;
            _lblHistoryHeader.Text = isREAccount ? "Bill_REPaymentSectionHeader".Translate() : "Bill_PaymentSectionHeader".Translate();
            _lblHistoryHeader.TextColor = myTNBColor.PowerBlue();
            _lblHistoryHeader.BackgroundColor = UIColor.Clear;
            _historyHeaderView.AddSubview(_lblHistoryHeader);

            //Bills and Payment History Selection Buttons
            _historySelectionView = new UIView(new RectangleF(0, 461, (float)View.Frame.Width, 48));
            _historySelectionView.BackgroundColor = myTNBColor.SectionGrey();

            var btnWidth = (View.Frame.Width) / 2;

            _btnBills = new UIButton(UIButtonType.Custom);
            _btnBills.Frame = new CGRect(0, 0, btnWidth, 48);
            _btnBills.SetTitle("Bill_Bills".Translate(), UIControlState.Normal);
            _btnBills.SetTitleColor(myTNBColor.PowerBlue(), UIControlState.Normal);
            _btnBills.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnBills.BackgroundColor = UIColor.White;
            MakeTopCornerRadius(_btnBills);
            _btnBills.Tag = 0;
            _historySelectionView.AddSubview(_btnBills);

            _btnPayment = new UIButton(UIButtonType.Custom);
            _btnPayment.Frame = new CGRect(_btnBills.Frame.Width, 0, btnWidth, 48);
            _btnPayment.SetTitle("Common_Payment".Translate(), UIControlState.Normal);
            _btnPayment.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
            _btnPayment.TitleLabel.Font = myTNBFont.MuseoSans16();
            _btnPayment.BackgroundColor = myTNBColor.SelectionGrey();
            MakeTopCornerRadius(_btnPayment);
            _btnPayment.Tag = 1;
            _historySelectionView.AddSubview(_btnPayment);

            _headerView.AddSubview(_historySelectionView);

            //BillTableView Header
            /*var footerView = new UIView(new RectangleF(0, 0, (float)View.Frame.Width, 62));
            footerView.BackgroundColor = myTNBColor.SectionGrey();
            billTableView.TableFooterView = footerView;

            var lblFooterDetails = new UILabel(new CGRect(12, 16, View.Frame.Width - 24, 18));
            lblFooterDetails.Font = myTNBFont.MuseoSans12();
            lblFooterDetails.TextAlignment = UITextAlignment.Center;
            lblFooterDetails.Text = "For more Bill/Payment History, visit myTNB Self-Service Portal.";
            lblFooterDetails.TextColor = UIColor.Gray;
            lblFooterDetails.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(lblFooterDetails);

            var lblFooterSubDetail = new UILabel(new CGRect(12, 34, View.Frame.Width - 24, 18));
            lblFooterSubDetail.Font = myTNBFont.MuseoSans12();
            lblFooterSubDetail.TextAlignment = UITextAlignment.Center;
            lblFooterSubDetail.Text = "www.mytnb.com.my";
            lblFooterSubDetail.TextColor = myTNBColor.PowerBlue();
            lblFooterSubDetail.BackgroundColor = UIColor.Clear;
            footerView.AddSubview(lblFooterSubDetail);*/

            _btnPay.Enabled = false;
            _btnPay.BackgroundColor = myTNBColor.SilverChalice();
        }

        /// <summary>
        /// Shows the non owner view.
        /// </summary>
        public void ShowNonOwnerView()
        {
            _lblViewAddress.Hidden = true;
            float adjY = ((float)_lblViewAddress.Frame.Height + 16.0f) * -1.0f;

            AdjustFrameAddY(_currentBillHeaderView, adjY);
            AdjustFrameAddY(_currentBillDetailsView, adjY);
            AdjustFrameAddY(_historyHeaderView, adjY);
            AdjustFrameAddY(_historySelectionView, adjY);

            AdjustFrameHeight(_headerView, adjY);
            billTableView.TableHeaderView = _headerView;
        }

        private void AdjustFrameAddY(UIView adjView, nfloat adjY)
        {
            var temp = adjView.Frame;
            temp.Y += adjY;
            adjView.Frame = temp;
        }

        private void AdjustFrameSetY(UIView adjView, nfloat adjY)
        {
            var temp = adjView.Frame;
            temp.Y = adjY;
            adjView.Frame = temp;
        }

        private void AdjustFrameHeight(UIView adjView, nfloat adjY)
        {
            var temp = adjView.Frame;
            temp.Height += adjY;
            adjView.Frame = temp;
        }

        void MakeTopCornerRadius(UIButton btn)
        {
            var maskPath = UIBezierPath.FromRoundedRect(btn.Bounds
                                                        , UIRectCorner.TopLeft | UIRectCorner.TopRight
                                                        , new CGSize(10.0, 10.0));
            var maskLayer = new CAShapeLayer
            {
                Frame = btn.Bounds,
                Path = maskPath.CGPath
            };
            btn.Layer.Mask = maskLayer;
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        void AdjustFrames()
        {
            CGSize newSize = GetLabelSize(_lblAmount, View.Frame.Width / 2, _lblAmount.Frame.Height);
            double newWidth = Math.Ceiling(newSize.Width);
            _lblAmount.Frame = new CGRect(24, 0, newWidth, _lblAmount.Frame.Height);
            _viewAmount.Frame = new CGRect(View.Frame.Width - (newWidth + 24 + 17), _viewAmount.Frame.Y, newWidth + 24, 24);
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

            _btnPayment.TouchUpInside += (sender, e) =>
            {
                DataManager.DataManager.SharedInstance.selectedTag = 1;
                ToggleButtons();
            };

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
                            ErrorHandler.DisplayNoDataAlert(this);
                        }
                    });
                });
            };
        }

        void ToggleButtons()
        {
            bool isBillSelected = DataManager.DataManager.SharedInstance.selectedTag == 0;
            _btnBills.SetTitleColor(isBillSelected ? myTNBColor.PowerBlue() : UIColor.LightGray, UIControlState.Normal);
            _btnBills.BackgroundColor = isBillSelected ? UIColor.White : myTNBColor.SelectionGrey();
            _btnPayment.SetTitleColor(isBillSelected ? UIColor.LightGray : myTNBColor.PowerBlue(), UIControlState.Normal);
            _btnPayment.BackgroundColor = isBillSelected ? myTNBColor.SelectionGrey() : UIColor.White;

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
                        ErrorHandler.DisplayNoDataAlert(this);
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
                        ErrorHandler.DisplayServiceError(this, _billingAccountDetailsList?.d?.message);
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

        /// <summary>
        /// Pulls down to refresh.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void PullDownTorefresh(object sender, EventArgs e)
        {
            if (!isRefreshing)
            {
                await RefreshScreen();
            }
        }

        /// <summary>
        /// Refreshes the screen.
        /// </summary>
        /// <returns>The screen.</returns>
        private async Task RefreshScreen()
        {
            string _dateDue;
            double _amountDue;
            double _dueIncrementDays;
            isRefreshing = true;
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

            var currentAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCurrentChg ?? 0;
            var outstandingAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amOutstandingChg ?? 0;
            var payableAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amPayableChg ?? 0;
            var balanceAmt = DataManager.DataManager.SharedInstance.BillingAccountDetails?.amCustBal ?? 0;

            var current = !isREAccount ? currentAmt : ChartHelper.UpdateValueForRE(currentAmt);
            var outstanding = !isREAccount ? outstandingAmt : ChartHelper.UpdateValueForRE(outstandingAmt);
            var payable = !isREAccount ? payableAmt : ChartHelper.UpdateValueForRE(payableAmt);
            var balance = !isREAccount ? balanceAmt : ChartHelper.UpdateValueForRE(balanceAmt);

            _lblCurrentChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, current.ToString("N2", CultureInfo.InvariantCulture));
            _lblOutstandingChargesValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, outstanding.ToString("N2", CultureInfo.InvariantCulture));
            _lblTotalPayableValue.Text = string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, payable.ToString("N2", CultureInfo.InvariantCulture));
            _lblAmount.Text = balance.ToString("N2", CultureInfo.InvariantCulture);

            AdjustFrames();
            isRefreshing = false;
            refreshControl.EndRefreshing();
        }
    }
}
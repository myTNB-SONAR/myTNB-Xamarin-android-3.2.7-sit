using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Carousels;
using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using myTNB.DataManager;
using myTNB.Enums;
using myTNB.Model;
using myTNB.PushNotification;
using myTNB.Registration.CustomerAccounts;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB.Dashboard
{
    public partial class DashboardViewController : CustomUIViewController
    {
        public DashboardViewController(IntPtr handle) : base(handle)
        {
        }

        DashboardMainComponent _dashboardMainComponent;
        DueAmountResponseModel _dueAmount = new DueAmountResponseModel();
        BillingAccountDetailsResponseModel _billingAccountDetailsList = new BillingAccountDetailsResponseModel();

        double _amountDue, _dueIncrementDays, _lastContentOffset;
        string _dateDue = string.Empty;
        bool isAnimating, isREAccount, isFromForeground, isFromViewBillAdvice;
        bool isBcrmAvailable = true;
        bool isNormalChart = true;

        public bool ShouldShowBackButton;
        string _toolTipMessage = string.Empty;
        string _toolTipBtnTitle = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            if (appDelegate != null)
            {
                appDelegate._dashboardVC = this;
            }
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver((NSString)"NotificationDidChange", NotificationDidChange);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            NavigationController?.SetNavigationBarHidden(true, false);
            NavigationItem?.SetHidesBackButton(true, false);
            _dashboardMainComponent = new DashboardMainComponent(View);
            _dashboardMainComponent.ToolTipGestureRecognizer = new UITapGestureRecognizer((obj) =>
            {
                DisplayCustomAlert(string.Empty, _toolTipMessage, _toolTipBtnTitle, null);
            });
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        await PushNotificationHelper.GetNotifications();
                        if (_dashboardMainComponent._titleBarComponent != null)
                        {
                            _dashboardMainComponent._titleBarComponent.SetPrimaryImage(PushNotificationHelper.GetNotificationImage());
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        public void NotificationDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> Inner DASHBOARD NotificationDidChange");
            _dashboardMainComponent?._titleBarComponent?.SetPrimaryImage(PushNotificationHelper.GetNotificationImage());
            PushNotificationHelper.UpdateApplicationBadge();
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> INNER DASHBOARD LanguageDidChange");
        }

        internal void HandleAppWillEnterForeground(NSNotification notification)
        {
            Debug.WriteLine("HandleAppWillEnterForeground");
            isFromForeground = true;
            if (!DataManager.DataManager.SharedInstance.IsLoggedIn())
            {
                return;
            }
            ViewWillAppear(true);
            ViewDidAppear(true);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            DataManager.DataManager.SharedInstance.AccountsToBeAddedList = new CustomerAccountRecordListModel();
            DataManager.DataManager.SharedInstance.IsBillUpdateNeeded = true;

            isREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            isNormalChart = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;
            isBcrmAvailable = DataManager.DataManager.SharedInstance.IsBcrmAvailable;

            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count <= 1)
            {
                DataManager.DataManager.SharedInstance.IsSameAccount = false;
            }
            if (!DataManager.DataManager.SharedInstance.IsSameAccount && !isFromViewBillAdvice)
            {
                if (isNormalChart && !isBcrmAvailable)
                {
                    _dashboardMainComponent.ConstructBCRMDownDashboard();
                }
                else
                {
                    _dashboardMainComponent.ConstructInitialView(isNormalChart, isFromForeground);
                }
                isFromForeground = false;
                SetEventsAndText();
            }
            else if (isFromViewBillAdvice)
            {
                isFromViewBillAdvice = false;
            }

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (!NetworkUtility.isReachable)
                    {
                        Debug.WriteLine("No Network");
                        DisplayNoDataAlert();
                        _dashboardMainComponent.ConstructNoDataConnectionDashboard();
                        SetEventsAndText();
                    }
                });
            });
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!ServiceCall.HasAccountList())
            {
                RenderDisplay();
            }
            else
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(async () =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
                            {
                                await LoadDashboard();
                                await LoadAmountDue();
                            }
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
        /// Loads the amount due.
        /// </summary>
        /// <returns>The amount due.</returns>
        private async Task LoadAmountDue()
        {
            var acct = DataManager.DataManager.SharedInstance.SelectedAccount;
            var due = DataManager.DataManager.SharedInstance.GetDue(acct.accNum);

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
                        && _dueAmount?.d?.didSucceed == true
                        && _dueAmount?.d?.data != null)
                    {
                        _amountDue = _dueAmount.d.data.amountDue;
                        _dateDue = _dueAmount.d.data.billDueDate;
                        _dueIncrementDays = _dueAmount.d.data.IncrementREDueDateByDays;
                        SetAmountInBillingDetails(_amountDue);
                        SaveDueToCache(_dueAmount.d.data);
                    }
                    SetBillAndPaymentDetails();
                });
            });
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
        /// Gets the usage history.
        /// </summary>
        /// <returns>The usage history.</returns>
        private async Task GetUsageHistory()
        {
            bool isNormalMeter = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter;
            bool isResultSuccess = false;
            string errorMessage = string.Empty;

            _dashboardMainComponent._billAndPaymentView.Hidden = false;
            TNBGlobal.IsChartEmissionEnabled = false;
            DataManager.DataManager.SharedInstance.CurrentChartMode = ChartModeEnum.Cost;
            var accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
            if (isNormalMeter || isREAccount)
            {
                ChartDataModel cachedData = GetCachedChartData(accNum);
                if (cachedData != null)
                {
                    isResultSuccess = true;
                    DataManager.DataManager.SharedInstance.CurrentChart = cachedData;
                }
                else
                {
                    ChartModel chartResponse = await GetAccountUsageHistoryForGraph();
                    isResultSuccess = chartResponse.didSucceed;
                    ChartDataModel dataModel = chartResponse.data;
                    DataManager.DataManager.SharedInstance.CurrentChart = dataModel;

                    if (isResultSuccess)
                    {
                        DataManager.DataManager.SharedInstance.SaveChartToUsageHistory(dataModel, accNum);
                    }
                    else
                    {
                        errorMessage = !string.IsNullOrWhiteSpace(chartResponse.message) ? chartResponse.message : "Error_DefaultMessage".Translate();
                    }
                }
            }
            else
            {
                SmartChartDataModel cachedData = GetCachedSmartChartData(accNum);
                if (cachedData != null)
                {
                    isResultSuccess = true;
                    ChartHelper.RemoveExcessSmartMonthData(ref cachedData);
                    DataManager.DataManager.SharedInstance.CurrentChart = cachedData;
                }
                else
                {
                    SmartChartModel chartResponse = await GetSmartMeterAccountData();
                    isResultSuccess = chartResponse.didSucceed;

                    if (isResultSuccess)
                    {
                        if (chartResponse.StatusCode != TNBGlobal.Errors.NoSmartData)
                        {
                            TNBGlobal.IsChartEmissionEnabled = !chartResponse.data.IsEmissionDisabled;
                            SmartChartDataModel dataModel = chartResponse.data;
                            ChartHelper.RemoveExcessSmartMonthData(ref dataModel);
                            DataManager.DataManager.SharedInstance.CurrentChart = dataModel;
                            DataManager.DataManager.SharedInstance.SaveSmartChartToUsageHistory(dataModel, accNum);
                        }
                        else
                        {
                            DataManager.DataManager.SharedInstance.CurrentChart = new ChartDataModelBase { CustomMessage = chartResponse.StatusCode };
                        }
                    }
                    else if (chartResponse.StatusCode == TNBGlobal.Errors.FetchingSmartData)
                    {
                        var message = !string.IsNullOrWhiteSpace(chartResponse.message)
                            ? chartResponse.message : "Error_GetSmartMeterDataMessage".Translate();
                        ShowToast(message);
                        ChartModel normalChartResponse = await GetAccountUsageHistoryForGraph();
                        isResultSuccess = normalChartResponse.didSucceed;
                        DataManager.DataManager.SharedInstance.CurrentChart = normalChartResponse.data;

                        if (!isResultSuccess)
                        {
                            errorMessage = !string.IsNullOrWhiteSpace(normalChartResponse.message)
                                ? normalChartResponse.message : "Error_DefaultMessage".Translate();
                        }
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.CurrentChart = chartResponse.data;
                        errorMessage = !string.IsNullOrWhiteSpace(chartResponse.message)
                            ? chartResponse.message : "Error_DefaultMessage".Translate();
                    }
                }
            }

            if (isResultSuccess)
            {
                SaveAccountChartData();
            }

            InvokeOnMainThread(() =>
            {
                RenderDisplay(errorMessage);
            });
        }

        /// <summary>
        /// Saves the account chart data.
        /// </summary>
        private void SaveAccountChartData()
        {
            if (DataManager.DataManager.SharedInstance.CurrentChart != null)
            {
                if (DataManager.DataManager.SharedInstance.AccountChartDictionary
                   .ContainsKey(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                {
                    DataManager.DataManager.SharedInstance.AccountChartDictionary[DataManager.DataManager.SharedInstance.SelectedAccount.accNum]
                        = DataManager.DataManager.SharedInstance.CurrentChart;
                }
                else
                {
                    DataManager.DataManager.SharedInstance.AccountChartDictionary.Add(DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                        , DataManager.DataManager.SharedInstance.CurrentChart);
                }
            }
        }

        void SetAmountInBillingDetails(double amount)
        {
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                DataManager.DataManager.SharedInstance.BillingAccountDetails.amCustBal = amount;
            }
        }

        internal void SetEventsAndText()
        {
            SetAccountDetails();
            SetEvents();
        }

        internal bool IsEstimatedReading(List<SegmentDetailsModel> data)
        {
            int index = data.FindIndex((SegmentDetailsModel obj) =>
            {
                return obj.IsEstimatedReading != null && obj.IsEstimatedReading.ToLower() == "true";
            });
            return index != -1;
        }
        #region Events
        private void SetEvents()
        {
            SetAccountSelectionEvent();
            SetSelectorComponent();
            SetChartComponentEvent();
            SetCarouselEvent();
            SetBillAndPaymentEvent();
            SetNoAccountEvent();
            SetAccessEvent();
            SetNoDataConnectionEvent();
            SetTitleBarEvent();
            SetScrollEvent();
        }

        private void SetAccountSelectionEvent()
        {
            if (_dashboardMainComponent._accountSelectionComponent != null)
            {
                if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                   && DataManager.DataManager.SharedInstance.AccountRecordsList?.d != null
                   && DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count > 0)
                {
                    UITapGestureRecognizer accountSelectionGesture = new UITapGestureRecognizer(() =>
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                        SelectAccountTableViewController viewController =
                            storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    });
                    _dashboardMainComponent._accountSelectionComponent.SetSelectAccountEvent(accountSelectionGesture);
                }
                _dashboardMainComponent._accountSelectionComponent.SetDropdownVisibility(false);//DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count == 1);
                _dashboardMainComponent._accountSelectionComponent.SetLeafVisibility(!isREAccount);
            }
        }

        private void SetSelectorComponent()
        {
            if (_dashboardMainComponent._selectorComponent != null)
            {
                UISegmentedControl selector = _dashboardMainComponent._selectorComponent._selectorBar;
                selector.SelectedSegment = 1;
                if (_dashboardMainComponent._usageHistoryComponent != null && NetworkUtility.isReachable)
                {
                    selector.ValueChanged += (sender, e) =>
                    {
                        DataManager.DataManager.SharedInstance.IsMontView = selector.SelectedSegment != 0;
                        DataManager.DataManager.SharedInstance.CurrentChartIndex = 0;
                        DisplayCurrentChart();
                        OnUpdateSmartChartIndex(false, false);
                    };
                }
            }
        }

        private void SetChartComponentEvent()
        {
            if (_dashboardMainComponent._chartComponent != null)
            {
                // CHART_CAROUSEL interchange left and right gestures if have carousel
                List<SegmentDetailsModel> chartData = new List<SegmentDetailsModel>();

#if CHART_CAROUSEL
                UITapGestureRecognizer leftGesture = new UITapGestureRecognizer(() =>
#else
                UITapGestureRecognizer rightGesture = new UITapGestureRecognizer(() =>
#endif
                {
                    if (DataManager.DataManager.SharedInstance.CurrentChartIndex > 0)
                    {
                        DataManager.DataManager.SharedInstance.CurrentChartIndex--;
#if !CHART_CAROUSEL
                        DisplayCurrentChart(); // !CHART_CAROUSEL
#endif
                        OnUpdateSmartChartIndex(true);
                    }
                });

#if CHART_CAROUSEL
                UITapGestureRecognizer rightGesture = new UITapGestureRecognizer(() =>
#else
                UITapGestureRecognizer leftGesture = new UITapGestureRecognizer(() =>
#endif
                {
                    // assumes this is displayed only for smart meters
                    int count = ChartHelper.GetSmartDataChartCount(DataManager.DataManager.SharedInstance.CurrentChart);
                    if (DataManager.DataManager.SharedInstance.CurrentChartIndex < count - 1)
                    {
                        DataManager.DataManager.SharedInstance.CurrentChartIndex++;
#if !CHART_CAROUSEL
                        DisplayCurrentChart(); // !CHART_CAROUSEL
#endif
                        OnUpdateSmartChartIndex(true);
                    }

                });
                _dashboardMainComponent._usageHistoryComponent.AddLeftNavigationEvent(leftGesture);
                _dashboardMainComponent._usageHistoryComponent.AddRightNavigationEvent(rightGesture);

                // chart mode handlers
                EventHandler amountHandler = (sender, e) =>
                {
                    OnTapChartMode(ChartModeEnum.Cost);
                };
                EventHandler consumptionHandler = (sender, e) =>
                {
                    OnTapChartMode(ChartModeEnum.Usage);
                };
                EventHandler emissionHandler = (sender, e) =>
                {
                    OnTapChartMode(ChartModeEnum.Emission);
                };
                _dashboardMainComponent._chartCompanionComponent?.AddChartModeHandler(ChartModeEnum.Cost, amountHandler);
                _dashboardMainComponent._chartCompanionComponent?.AddChartModeHandler(ChartModeEnum.Usage, consumptionHandler);
                _dashboardMainComponent._chartCompanionComponent?.AddChartModeHandler(ChartModeEnum.Emission, emissionHandler);
            }
        }

        private void SetCarouselEvent()
        {
            if (_dashboardMainComponent._chartCarousel != null)
            {
                // handle item selections
                _dashboardMainComponent._chartCarousel.CurrentItemIndexChanged += (sender, args) =>
                {
                    iCarousel currentCarousel = sender as iCarousel;
                    DataManager.DataManager.SharedInstance.CurrentChartIndex = (int)currentCarousel?.CurrentItemIndex;
                    OnUpdateSmartChartIndex(false, false);
                };
            }
        }

        private void SetBillAndPaymentEvent()
        {
            if (_dashboardMainComponent._billAndPaymentComponent != null)
            {
                if (isREAccount)
                {
                    _dashboardMainComponent._billAndPaymentComponent.SetREAccountButton();
                }
                //if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount)
                //{
                //    _dashboardMainComponent._billAndPaymentComponent.ToggleBillAndPayVisibility(true);
                //}

                if (!isBcrmAvailable
                   || (isBcrmAvailable && !DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable
                       && !DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable))
                {
                    _dashboardMainComponent._billAndPaymentComponent.SetPayButtonEnable(false);
                }

                if (!isBcrmAvailable)
                {
                    _dashboardMainComponent._billAndPaymentComponent.SetBillButtonEnable(false);
                }

                _dashboardMainComponent._billAndPaymentComponent._btnPay.TouchUpInside += (sender, e) =>
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
                                    selectBillsVC.SelectedAccountDueAmount = _amountDue;
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
                _dashboardMainComponent._billAndPaymentComponent._btnViewBill.TouchUpInside += (sender, e) =>
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                                ViewBillViewController viewController =
                                    storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                                if (viewController != null)
                                {
                                    viewController.OnDone = OnViewDone;
                                    var navController = new UINavigationController(viewController);
                                    PresentViewController(navController, true, null);
                                }
                            }
                            else
                            {
                                DisplayNoDataAlert();
                            }
                        });
                    });
                };
            }
        }

        private void SetNoAccountEvent()
        {
            if (_dashboardMainComponent._noAccountComponent != null)
            {
                _dashboardMainComponent._noAccountComponent._btnAddAccount.TouchUpInside += (sender, e) =>
                {
                    //Todo: Handle Add Account
                    Debug.WriteLine("Add account button tapped");
                    UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                    AccountsViewController viewController =
                        storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                    if (viewController != null)
                    {
                        viewController.isDashboardFlow = true;
                        viewController._needsUpdate = true;
                        var navController = new UINavigationController(viewController);
                        PresentViewController(navController, true, null);
                    }
                };
            }
        }

        private void SetTitleBarEvent()
        {
            if (_dashboardMainComponent._titleBarComponent != null)
            {
                _dashboardMainComponent._titleBarComponent.SetBackVisibility(!ShouldShowBackButton);

                if (ShouldShowBackButton)
                {
                    _dashboardMainComponent._titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
                    {
                        NavigationController.PopViewController(true);
                    }));
                }
                UITapGestureRecognizer notificationTap = new UITapGestureRecognizer(() =>
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                                PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                                var navController = new UINavigationController(viewController);
                                PresentViewController(navController, true, null);
                            }
                            else
                            {
                                Debug.WriteLine("No Network");
                                DisplayNoDataAlert();
                            }
                        });
                    });
                });
                _dashboardMainComponent._titleBarComponent.SetPrimaryAction(notificationTap);
            }
        }

        private void SetScrollEvent()
        {
            if (_dashboardMainComponent._dashboardScrollView != null)
            {
                _dashboardMainComponent._dashboardScrollView.Scrolled += OnScrollDashboard;
            }
        }

        private void SetNoDataConnectionEvent()
        {
            if (_dashboardMainComponent._noDataConnectionComponent != null)
            {
                _dashboardMainComponent._noDataConnectionComponent._btnRefresh.TouchUpInside += (sender, e) =>
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(async () =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                await LoadDashboard();
                                await LoadAmountDue();
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

        private void SetAccessEvent()
        {
            if (_dashboardMainComponent._getAccessComponent != null)
            {
                _dashboardMainComponent._getAccessComponent._btnGetAccess.TouchUpInside += (sender, e) =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("GetAccess", null);
                    GetAccessViewController viewController =
                        storyBoard.InstantiateViewController("GetAccessViewController") as GetAccessViewController;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                };
            }
        }
        #endregion
        internal void SetAccountDetails()
        {
            if (_dashboardMainComponent._accountSelectionComponent != null)
            {
                _dashboardMainComponent._accountSelectionComponent.SetAccountName(DataManager.DataManager.SharedInstance.SelectedAccount.accDesc);
            }
            if (_dashboardMainComponent._addressComponent != null)
            {
                string address = NetworkUtility.isReachable ? DataManager.DataManager.SharedInstance.SelectedAccount.accountStAddress : "";
                _dashboardMainComponent._addressComponent.SetAddress(address);
            }
            if (_dashboardMainComponent._titleBarComponent != null)
            {
                _dashboardMainComponent._titleBarComponent.SetPrimaryImage(PushNotificationHelper.GetNotificationImage());
            }
            SetBillAndPaymentDetails();
        }

        internal void SetBillAndPaymentDetails()
        {
            if (_dashboardMainComponent._billAndPaymentComponent != null)
            {
                string amount = NetworkUtility.isReachable ? _amountDue.ToString() : TNBGlobal.ZERO;
                _dashboardMainComponent._billAndPaymentComponent.SetAmount(amount, isREAccount);
                var adjAmount = !isREAccount ? _amountDue : ChartHelper.UpdateValueForRE(_amountDue);
                string dateString = !string.IsNullOrEmpty(_dateDue) && adjAmount > 0 ? _dateDue : string.Empty;

                string formattedDate = string.Empty;
                string prefix = string.Empty;
                if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                {
                    formattedDate = TNBGlobal.EMPTY_DATE;
                }
                else
                {
                    if (isREAccount && _dueIncrementDays > 0)
                    {
                        try
                        {
                            var format = @"dd/MM/yyyy";
                            DateTime due = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
                            due = due.AddDays(_dueIncrementDays);
                            dateString = due.ToString(format);
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
                _dashboardMainComponent._billAndPaymentComponent.SetDateDue(dueDate);
                _dashboardMainComponent._billAndPaymentComponent.SetPaymentTitle(isREAccount
                    ? "Bill_MyEarnings".Translate() : "Common_AmountDue".Translate());
                if (_dashboardMainComponent._billAndPaymentComponent._activity != null)
                {
                    _dashboardMainComponent._billAndPaymentComponent._activity.Hide();
                }

                if (_dueAmount != null && _dueAmount?.d != null && _dueAmount?.d?.didSucceed == true
                        && _dueAmount?.d?.data != null)
                {
                    if (_dueAmount.d.data.IsItemisedBilling)
                    {
                        _dashboardMainComponent._billAndPaymentComponent.DisplayInfoToolTip(_dueAmount.d.data.WhyThisAmountLink, DisplayToolTip);
                    }
                    else
                    {
                        _dashboardMainComponent._billAndPaymentComponent.HideInfo();
                    }
                }
            }
        }

        void DisplayToolTip()
        {
            string title = _dueAmount.d.data.WhyThisAmountTitle ?? "Dashboard_TooltipTitle".Translate();
            string msg = _dueAmount.d.data.WhyThisAmountMessage ?? "Dashboard_TooltipMessage".Translate();
            string primaryButton = _dueAmount.d.data.WhyThisAmountPriButtonText ?? "Common_GotIt".Translate();
            string secondaryButton = _dueAmount.d.data.WhyThisAmountSecButtonText ?? "Dashboard_BringMeThere".Translate();

            DisplayCustomAlert(title, msg
                , new Dictionary<string, Action>() {
                    { primaryButton, ()=>{
                        if (TabBarController != null)
                        {
                            TabBarController.SelectedIndex = 1;
                        }
                    }}
                    , { secondaryButton, null }
                });
        }

        /// <summary>
        /// Loads the dashboard.
        /// </summary>
        /// <returns>The dashboard.</returns>
        private async Task LoadDashboard()
        {
            await GetUsageHistory();
        }

        /// <summary>
        /// Renders the display.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        internal void RenderDisplay(string errorMessage = "")
        {
            DataManager.DataManager.SharedInstance.IsMontView = true;
            DataManager.DataManager.SharedInstance.CurrentChartIndex = 0;
            isNormalChart = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;

            if (DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.Count == 0)
            {
                _dashboardMainComponent.ConstructNoAccountDashboard();
            }
            else if (isNormalChart && !isBcrmAvailable)
            {
                _dashboardMainComponent.ConstructBCRMDownDashboard();
                ShowBcrmToast();
            }
            else if (DataManager.DataManager.SharedInstance.CurrentChart != null)
            {
                bool willShowDowntimeToast = (!isREAccount && !DataManager.DataManager.SharedInstance.IsPaymentCreditCardAvailable
                    && !DataManager.DataManager.SharedInstance.IsPaymentFPXAvailable) || !isBcrmAvailable;

                if (WillDisplayNoData())
                {
                    _dashboardMainComponent.ConstructSmartMeterDashboard();
                    if (willShowDowntimeToast)
                    {
                        ShowBcrmToast();
                    }
                    else if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        ShowToast(errorMessage);
                    }
                }
                else
                {
                    _dashboardMainComponent.ConstructChartDashboard(isNormalChart);
                    if (willShowDowntimeToast)
                    {
                        ShowBcrmToast();
                    }
                    else if (!string.IsNullOrWhiteSpace(errorMessage))
                    {
                        ShowToast(errorMessage);
                    }
                    DisplayCurrentChart();
                }
            }
            else
            {
                _dashboardMainComponent.ConstructNoDataConnectionDashboard();
            }
            SetEventsAndText();
        }

        /// <summary>
        /// Shows the planned downtime toast.
        /// </summary>
        private void ShowBcrmToast()
        {
            var status = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == SystemEnum.BCRM);
            if (status != null && !string.IsNullOrEmpty(status?.DowntimeTextMessage))
            {
                ShowToast(status?.DowntimeTextMessage);
            }
        }

        /// <summary>
        /// Shows the toast.
        /// </summary>
        private void ShowToast(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                View.BringSubviewToFront(toastView);
                ToastHelper.ShowToast(toastView, ref isAnimating, message);
            }
        }

        /// <summary>
        /// Checks if will display no data.
        /// </summary>
        /// <returns><c>true</c>, if display no data was willed, <c>false</c> otherwise.</returns>
        private bool WillDisplayNoData()
        {
            bool res = false;
            var msg = DataManager.DataManager.SharedInstance.CurrentChart?.CustomMessage;
            if (!string.IsNullOrEmpty(msg))
            {
                res = msg == TNBGlobal.Errors.NoSmartData;
            }
            return res;
        }

        /// <summary>
        /// Displays the current chart.
        /// </summary>
        private void DisplayCurrentChart()
        {
            List<SegmentDetailsModel> chartData = null;
            string dateRange = string.Empty;
            bool isNormalMeter = true;
            UsageMetrics smartMeterMetrics = null;
            bool res = ChartHelper.GetSelectedChartInfo(DataManager.DataManager.SharedInstance.CurrentChart, DataManager.DataManager.SharedInstance.IsMontView
                , DataManager.DataManager.SharedInstance.CurrentChartIndex, out smartMeterMetrics, out chartData, out dateRange, out isNormalMeter);
            DisplayChart(chartData, dateRange, isNormalMeter, DataManager.DataManager.SharedInstance.IsMontView
                , DataManager.DataManager.SharedInstance.CurrentChartMode, smartMeterMetrics, res);
        }

        /// <summary>
        /// Displays the chart.
        /// </summary>
        /// <param name="chartData">Chart data.</param>
        /// <param name="dateRange">Date range.</param>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        /// <param name="isMonthView">If set to <c>true</c> is month view.</param>
        /// <param name="chartMode">Chart mode.</param>
        /// <param name="smartMeterMetric">Smart meter metric.</param>
        private void DisplayChart(List<SegmentDetailsModel> chartData, string dateRange, bool isNormalMeter, bool isMonthView,
                                  ChartModeEnum chartMode, UsageMetrics smartMeterMetric, bool isGetChartSuccess)
        {

            if (isGetChartSuccess)
            {
#if CHART_CAROUSEL
                _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView
                    , DataManager.DataManager.SharedInstance.CurrentChart, isREAccount);
#else
                _dashboardMainComponent._chartComponent.ConstructSegmentViews(chartData, isNormalMeter, chartMode, isREAccount);
#endif
                _dashboardMainComponent._usageHistoryComponent.ToggleNavigationVisibility(isNormalMeter);
            }
            else
            {
                string range = string.Empty;
                if (isNormalMeter)
                {
                    var model = new ChartDataModel();
                    ByMonthModel month = new ByMonthModel();

                    ChartHelper.CreateDefaultChartData(isMonthView, out chartData, out range);
                    month.Range = range;
                    month.Months = new List<SegmentDetailsModel>(chartData);
                    model.ByMonth = month;
#if CHART_CAROUSEL
                    _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView, model);
#endif
                }
                else
                {
                    var model = new SmartChartDataModel();
                    model.ByMonth = new List<ByMonthModel>();

                    if (isMonthView)
                    {
                        ByMonthModel month = new ByMonthModel();
                        ChartHelper.CreateDefaultChartData(true, out chartData, out range);
                        month.Range = range;
                        month.Months = new List<SegmentDetailsModel>(chartData);
                        model.ByMonth.Add(month);
                    }
                    else
                    {
                        ChartHelper.CreateDefaultChartData(false, out chartData, out range);
                        ByDayModel dayModel = new ByDayModel();
                        dayModel.Range = range;
                        dayModel.Days = new List<SegmentDetailsModel>(chartData);
                        model.ByDay.Add(dayModel);
                    }
#if CHART_CAROUSEL
                    _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView, model);
#endif
                }

#if !CHART_CAROUSEL
                _dashboardMainComponent._chartComponent.ConstructSegmentViews(chartData, isNormalMeter, chartMode, isREAccount);
#endif
                _dashboardMainComponent._usageHistoryComponent.ToggleNavigationVisibility(true);
            }


            _dashboardMainComponent._viewChart.Hidden = false;

            if (!isNormalMeter)
            {
                int count = ChartHelper.GetSmartDataChartCount(DataManager.DataManager.SharedInstance.CurrentChart);
#if CHART_CAROUSEL
                DataManager.DataManager.SharedInstance.CurrentChartIndex = count - 1;
#endif
                OnUpdateSmartChartIndex(false);
            }
            _dashboardMainComponent._selectorComponent.SetHidden(isNormalMeter);
            _dashboardMainComponent._chartCompanionComponent.SetHidden(isNormalMeter);

            if (!isNormalMeter)
            {
                ToolTipsModel toolTipData = (DataManager.DataManager.SharedInstance.CurrentChart
                    as SmartChartDataModel).ToolTips?.Find(x => string.Compare(x.Type.ToUpper(), "PROJECTEDCOST") > -1);
                _toolTipMessage = toolTipData?.Message;
                _toolTipBtnTitle = toolTipData?.SMBtnText ?? "Common_GotIt".Translate();
                _dashboardMainComponent._chartCompanionComponent.SetUsageMetric(smartMeterMetric);
                _dashboardMainComponent._chartCompanionComponent.SetChartMode(DataManager.DataManager.SharedInstance.CurrentChartMode);
                _dashboardMainComponent._chartCompanionComponent.SetTooltipLink(toolTipData?.SMLink);
            }
            if (_dashboardMainComponent._dashboardScrollView != null)
            {
                _dashboardMainComponent._dashboardScrollView.ScrollEnabled = !isNormalMeter;
            }
            if (_dashboardMainComponent._chartCarousel != null)
            {
                _dashboardMainComponent._chartCarousel.ScrollEnabled = !isNormalMeter;
            }
            _dashboardMainComponent._usageHistoryComponent.SetFrameByMeterType(isNormalMeter);
            _dashboardMainComponent._billAndPaymentComponent?.SetMaskHidden(isNormalMeter);

            float yLoc;
            if (isNormalMeter)
            {
                yLoc = (float)_dashboardMainComponent._viewChart.Frame.GetMaxY() + 28f;
                if (DeviceHelper.IsIphoneXUpResolution())
                {
                    yLoc = (float)_dashboardMainComponent._viewChart.Frame.GetMaxY() + 5f;
                }
                else if (DeviceHelper.IsIphone6UpResolution())
                {
                    yLoc = (float)_dashboardMainComponent._viewChart.Frame.GetMaxY() + 40;
                }
            }
            else
            {
                yLoc = (float)_dashboardMainComponent._viewChartCompanion.Frame.GetMaxY() + 18f;
            }
            _dashboardMainComponent._addressComponent.SetFrameByPrecedingView(yLoc);
            _dashboardMainComponent._lblEstimatedReading.Hidden = (isMonthView) ? !IsEstimatedReading(chartData) : true;
            _dashboardMainComponent._usageHistoryComponent.SetDateRange(dateRange);
        }

        /// <summary>
        /// Handles the index update of the smart chart.
        /// </summary>
        /// <param name="willAnimate">If set to <c>true</c> will animate.</param>
        /// <param name="willScrollToIndex">If set to <c>true</c> will scroll to index.</param>
        private void OnUpdateSmartChartIndex(bool willAnimate, bool willScrollToIndex = true)
        {
            int count = ChartHelper.GetSmartDataChartCount(DataManager.DataManager.SharedInstance.CurrentChart);
            if (willScrollToIndex && _dashboardMainComponent._chartCarousel != null)
            {
                _dashboardMainComponent._chartCarousel.ScrollToItemAt(DataManager.DataManager.SharedInstance.CurrentChartIndex, willAnimate);
            }

            if (count > 1)
            {
#if CHART_CAROUSEL
                _dashboardMainComponent._usageHistoryComponent.ToggleRightNavigationVisibility(DataManager.DataManager.SharedInstance.CurrentChartIndex == count - 1);
                _dashboardMainComponent._usageHistoryComponent.ToggleLeftNavigationVisibility(DataManager.DataManager.SharedInstance.CurrentChartIndex == 0);
#else
                _dashboardMainComponent._usageHistoryComponent.ToggleRightNavigationVisibility(DataManager.DataManager.SharedInstance.CurrentChartIndex == 0);
                _dashboardMainComponent._usageHistoryComponent.ToggleLeftNavigationVisibility(DataManager.DataManager.SharedInstance.CurrentChartIndex == count - 1);
#endif
            }
            else
            {
                _dashboardMainComponent._usageHistoryComponent.ToggleNavigationVisibility(true);
            }
            iCarousel currentCarousel = _dashboardMainComponent._chartCarousel;
            if (currentCarousel?.CurrentItemView?.ViewWithTag(TNBGlobal.Tags.RangeLabel) is UILabel rangeLabel)
            {
                _dashboardMainComponent._usageHistoryComponent.SetDateRange(rangeLabel?.Text);
            }
        }

        /// <summary>
        /// Handler for completing view bill or payment advice.
        /// </summary>
        public void OnViewDone()
        {
            isFromViewBillAdvice = true;
        }

        internal Task GetAccountDueAmount()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    accNum = DataManager.DataManager.SharedInstance.SelectedAccount?.accNum
                };
                _dueAmount = serviceManager.GetAccountDueAmount("GetAccountDueAmount", requestParameter);
            });
        }

        /// <summary>
        /// Gets the account usage history for normal meter's graph.
        /// </summary>
        /// <returns>The account usage history for graph.</returns>
        private async Task<ChartModel> GetAccountUsageHistoryForGraph()
        {
            ChartModel chartResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum
            };

            _dashboardMainComponent?._componentActivity?.Show();
            chartResponse = await Task.Run(() =>
            {
                return serviceManager.GetAccountUsageHistoryForGraph("GetAccountUsageHistoryForGraph", requestParameter);
            });
            _dashboardMainComponent?._componentActivity?.Hide();

            return chartResponse;
        }

        /// <summary>
        /// Gets the smart meter account data.
        /// </summary>
        /// <returns>The smart meter account data.</returns>
        private async Task<SmartChartModel> GetSmartMeterAccountData()
        {
            SmartChartModel chartResponse = null;
            ServiceManager serviceManager = new ServiceManager();

            UserEntity user = (DataManager.DataManager.SharedInstance.UserEntity?.Count > 0)
                ? DataManager.DataManager.SharedInstance.UserEntity[0]
                : new SQLite.SQLiteDataManager.UserEntity();

            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                userEmail = user?.email,
                sspUserId = user?.userID,
                metercode = DataManager.DataManager.SharedInstance.SelectedAccount.smartMeterCode,
                isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned
            };
            _dashboardMainComponent?._componentActivity?.Show();
            chartResponse = await Task.Run(() =>
            {
                return serviceManager.GetSmartMeterAccountData("GetSmartMeterAccountData_V3", requestParameter);
            });

            _dashboardMainComponent?._componentActivity?.Hide();

            return chartResponse;
        }

        /// <summary>
        /// Gets the cached smart chart data.
        /// </summary>
        /// <returns>The cached chart data.</returns>
        /// <param name="accountNum">Account number.</param>
        private SmartChartDataModel GetCachedSmartChartData(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            bool isRefreshNeeded = default(bool);
            var model = DataManager.DataManager.SharedInstance.GetSmartAccountUsageHistory(accountNum, ref lastUpdate, ref isRefreshNeeded);
            if (model != null && lastUpdate.Date == DateTime.Today && !isRefreshNeeded)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Gets the cached chart data.
        /// </summary>
        /// <returns>The cached chart data.</returns>
        /// <param name="accountNum">Account number.</param>
        private ChartDataModel GetCachedChartData(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            bool isRefreshNeeded = default(bool);
            var model = DataManager.DataManager.SharedInstance.GetAccountUsageHistory(accountNum, ref lastUpdate, ref isRefreshNeeded);
            if (model != null && lastUpdate.Date == DateTime.Today && !isRefreshNeeded)
            {
                return model;
            }
            return null;
        }

        /// <summary>
        /// Handles the scroll in the dashboard.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnScrollDashboard(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            if (scrollView != null)
            {
                if (_lastContentOffset < 0 || _lastContentOffset < scrollView.ContentOffset.Y)
                {
                    //Pulling down
                    _dashboardMainComponent._billAndPaymentComponent.SetComponentHidden(true);
                }
                else if (_lastContentOffset > scrollView.ContentOffset.Y)
                {
                    //Pulling up
                    _dashboardMainComponent._billAndPaymentComponent.SetComponentHidden(false);
                }
                _lastContentOffset = scrollView.ContentOffset.Y;
            }
        }

        /// <summary>
        /// Ons the tap chart mode.
        /// </summary>
        /// <param name="chartMode">Chart mode.</param>
        private void OnTapChartMode(ChartModeEnum chartMode)
        {
            DataManager.DataManager.SharedInstance.CurrentChartMode = chartMode;
            _dashboardMainComponent._chartCompanionComponent?.SetChartMode(DataManager.DataManager.SharedInstance.CurrentChartMode);

#if CHART_CAROUSEL
            iCarousel currentCarousel = _dashboardMainComponent._chartCarousel;
            currentCarousel?.ReloadData();
#else
            DisplayCurrentChart();
#endif
        }
        /// <summary>
        /// Gets the billing account details.
        /// </summary>
        /// <returns>The billing account details.</returns>
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
    }
}
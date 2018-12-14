using System;
using System.Collections.Generic;
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
using UIKit;

namespace myTNB.Dashboard
{
    public partial class DashboardViewController : UIViewController
    {
        public DashboardViewController(IntPtr handle) : base(handle)
        {
        }

        DashboardMainComponent _dashboardMainComponent;
        DueAmountResponseModel _dueAmount = new DueAmountResponseModel();
        bool isAnimating = false;

        double _amountDue = 0;
        string _dateDue = string.Empty;
        bool isREAccount = false;
        double _lastContentOffset;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate._dashboardVC = this;
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            NavigationItem.HidesBackButton = true;
            _dashboardMainComponent = new DashboardMainComponent(View);
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.WillEnterForegroundNotification, HandleAppWillEnterForeground);
        }

        internal void HandleAppWillEnterForeground(NSNotification notification)
        {
            Console.WriteLine("HandleAppWillEnterForeground");
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
            if (!DataManager.DataManager.SharedInstance.IsSameAccount)
            {
                _dashboardMainComponent.ConstructInitialView();
                SetEventsAndText();
            }

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        if (DataManager.DataManager.SharedInstance.IsFromPushNotification)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                            PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                            var navController = new UINavigationController(viewController);
                            PresentViewController(navController, true, null);
                            DataManager.DataManager.SharedInstance.IsFromPushNotification = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
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

                                await GetAccountDueAmount().ContinueWith(dueTask =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        if (_dueAmount != null && _dueAmount.d != null
                                            && _dueAmount.d.isError.Equals("false"))
                                        {
                                            _amountDue = _dueAmount.d.data.amountDue;
                                            _dateDue = _dueAmount.d.data.billDueDate;
                                            SetAmountInBillingDetails(_amountDue);
                                        }
                                        SetBillAndPaymentDetails();
                                    });
                                });
                            }
                        }
                    });
                });
            }
        }

        /// <summary>
        /// Checks if display normal dashboard.
        /// </summary>
        /// <returns><c>true</c>, if normal dashboard display, <c>false</c> otherwise.</returns>
        private bool IsNormalDashboardDisplay()
        {
            return DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;
        }

        /// <summary>
        /// Gets the usage history.
        /// </summary>
        /// <returns>The usage history.</returns>
        private async Task GetUsageHistory()
        {
            bool isNormalMeter = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter;
            bool isResultSuccess = false;

            _dashboardMainComponent._billAndPaymentView.Hidden = false;
            TNBGlobal.IsChartEmissionEnabled = false;
            DataManager.DataManager.SharedInstance.CurrentChartMode = ChartModeEnum.Cost;
            if (isNormalMeter || isREAccount)
            {
                ChartModel chartResponse = await GetAccountUsageHistoryForGraph();
                isResultSuccess = chartResponse.didSucceed;
                DataManager.DataManager.SharedInstance.CurrentChart = chartResponse.data;
            }
            else
            {
                var accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                SmartChartDataModel cachedData = GetCachedChartData(accNum);
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
                        if (chartResponse.message != TNBGlobal.Errors.NoSmartData)
                        {
                            TNBGlobal.IsChartEmissionEnabled = !chartResponse.data.IsEmissionDisabled;
                            SmartChartDataModel dataModel = chartResponse.data;
                            ChartHelper.RemoveExcessSmartMonthData(ref dataModel);
                            DataManager.DataManager.SharedInstance.CurrentChart = dataModel;
                            DataManager.DataManager.SharedInstance.SaveToUsageHistory(dataModel, accNum);
                        }
                        else
                        {
                            DataManager.DataManager.SharedInstance.CurrentChart = new ChartDataModelBase { CustomMessage = chartResponse.message };
                        }
                    }
                    else if (chartResponse.message == TNBGlobal.Errors.FetchingSmartData)
                    {
                        View.BringSubviewToFront(toastView);
                        ToastHelper.ShowToast(toastView, ref isAnimating);
                        ChartModel normalChartResponse = await GetAccountUsageHistoryForGraph();
                        isResultSuccess = normalChartResponse.didSucceed;
                        DataManager.DataManager.SharedInstance.CurrentChart = normalChartResponse.data;
                    }
                    else
                    {
                        DataManager.DataManager.SharedInstance.CurrentChart = chartResponse.data;
                    }

                }
            }


            if (DataManager.DataManager.SharedInstance.CurrentChart != null && isResultSuccess)
            {
                if (DataManager.DataManager.SharedInstance.AccountChartDictionary
                   .ContainsKey(DataManager.DataManager.SharedInstance.SelectedAccount.accNum))
                {
                    DataManager.DataManager.SharedInstance
                               .AccountChartDictionary[DataManager.DataManager
                                                       .SharedInstance.SelectedAccount.accNum]
                               = DataManager.DataManager.SharedInstance.CurrentChart;
                }
                else
                {
                    DataManager.DataManager.SharedInstance.AccountChartDictionary
                               .Add(DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                                    , DataManager.DataManager.SharedInstance.CurrentChart);
                }

            }


            InvokeOnMainThread(RenderDisplay);

        }

        void SetAmountInBillingDetails(double amount)
        {
            if (DataManager.DataManager.SharedInstance.BillingAccountDetails != null)
            {
                DataManager.DataManager.SharedInstance
                           .BillingAccountDetails.amCustBal = amount;
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

        internal void SetEvents()
        {
            if (_dashboardMainComponent._accountSelectionComponent != null)
            {
                if (DataManager.DataManager.SharedInstance.AccountRecordsList != null
                   && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                   && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
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

            if (_dashboardMainComponent._billAndPaymentComponent != null)
            {
                if (isREAccount)
                {
                    _dashboardMainComponent._billAndPaymentComponent.SetREAccountButton();
                }
                //else if (!DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount)
                //{
                //    _dashboardMainComponent._billAndPaymentComponent.ToggleBillAndPayVisibility(true);
                //}
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
                                selectBillsVC.SelectedAccountDueAmount = _amountDue;
                                var navController = new UINavigationController(selectBillsVC);
                                PresentViewController(navController, true, null);
                            }
                            else
                            {
                                Console.WriteLine("No Network");
                                var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
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
                                var navController = new UINavigationController(viewController);
                                PresentViewController(navController, true, null);
                            }
                            else
                            {
                                Console.WriteLine("No Network");
                                var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
                            }
                        });
                    });
                };
            }

            if (_dashboardMainComponent._noAccountComponent != null)
            {
                _dashboardMainComponent._noAccountComponent._btnAddAccount.TouchUpInside += (sender, e) =>
                {
                    //Todo: Handle Add Account
                    Console.WriteLine("Add account button tapped");
                    UIStoryboard storyBoard = UIStoryboard.FromName("AccountRecords", null);
                    AccountsViewController viewController =
                        storyBoard.InstantiateViewController("AccountsViewController") as AccountsViewController;
                    viewController.isDashboardFlow = true;
                    viewController._needsUpdate = true;
                    var navController = new UINavigationController(viewController);
                    PresentViewController(navController, true, null);
                };
            }

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

                                await GetAccountDueAmount().ContinueWith(dueTask =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        if (_dueAmount != null && _dueAmount.d != null
                                            && _dueAmount.d.isError.Equals("false"))
                                        {
                                            _amountDue = _dueAmount.d.data.amountDue;
                                            _dateDue = _dueAmount.d.data.billDueDate;
                                        }
                                        SetBillAndPaymentDetails();
                                    });
                                });
                            }
                            else
                            {
                                Console.WriteLine("No Network");
                                var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
                            }
                        });
                    });
                };
            }

            if (_dashboardMainComponent._titleBarComponent != null)
            {
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
                                Console.WriteLine("No Network");
                                var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                                PresentViewController(alert, animated: true, completionHandler: null);
                            }
                        });
                    });
                });
                _dashboardMainComponent._titleBarComponent.SetNotificationAction(notificationTap);
            }

            if (_dashboardMainComponent._dashboardScrollView != null)
            {
                _dashboardMainComponent._dashboardScrollView.Scrolled += OnScrollDashboard;
            }

        }

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
                _dashboardMainComponent._titleBarComponent.SetNotificationImage(
                    DataManager.DataManager.SharedInstance.HasNewNotification ? "Notification-New" : "Notification");
            }
            SetBillAndPaymentDetails();
        }

        internal void SetBillAndPaymentDetails()
        {
            if (_dashboardMainComponent._billAndPaymentComponent != null)
            {
                string amount = NetworkUtility.isReachable ? _amountDue.ToString() : "0.00";
                _dashboardMainComponent._billAndPaymentComponent.SetAmount(amount);

                string dateString = NetworkUtility.isReachable && !string.IsNullOrEmpty(_dateDue) ? _dateDue : string.Empty;
                string formattedDate = string.Empty;
                if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                {
                    formattedDate = "Not Available";
                }
                else
                {
                    formattedDate = DateHelper.GetFormattedDate(dateString, "dd MMM yyyy");
                }

                string dueDate = isREAccount
                    ? formattedDate
                    : "Due " + formattedDate;
                _dashboardMainComponent._billAndPaymentComponent.SetDateDue(dueDate);
                //_dashboardMainComponent._billAndPaymentComponent.SetPayButtonEnable(_amountDue > 0);
                _dashboardMainComponent._billAndPaymentComponent.SetPaymentTitle(isREAccount
                                                                                 ? "Payment Advice Amount"
                                                                                 : "Total Amount Due");
                if (_dashboardMainComponent._billAndPaymentComponent._activity != null)
                {
                    _dashboardMainComponent._billAndPaymentComponent._activity.Hide();
                }
            }
        }

        /// <summary>
        /// Loads the dashboard.
        /// </summary>
        /// <returns>The dashboard.</returns>
        private async Task LoadDashboard()
        {
            if (DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount)
            {
                await GetUsageHistory();
            }
            else
            {
                _dashboardMainComponent.ConstructGetAccessDashboard(IsNormalDashboardDisplay());
                SetEventsAndText();
            }
        }

        internal void RenderDisplay()
        {
            DataManager.DataManager.SharedInstance.IsMontView = true;
            DataManager.DataManager.SharedInstance.CurrentChartIndex = 0;

            // todo: rra, remove temp
            //DataManager.DataManager.SharedInstance.CurrentChart = new ChartDataModelBase { CustomMessage = TNBGlobal.Errors.NoSmartData };

            if (DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count == 0)
            {
                _dashboardMainComponent.ConstructNoAccountDashboard();
            }
            else if (DataManager.DataManager.SharedInstance.CurrentChart != null)
            {
                if (!WillDisplayNoData())
                {
                    bool isNormalChart = DataManager.DataManager.SharedInstance.SelectedAccount.IsNormalMeter || isREAccount;
                    _dashboardMainComponent.ConstructChartDashboard(isNormalChart);
                    DisplayCurrentChart();
                }
                else
                {
                    _dashboardMainComponent.ConstructSmartMeterDashboard();
                }
            }
            else
            {
                _dashboardMainComponent.ConstructNoDataConnectionDashboard();
            }
            SetEventsAndText();
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

#if true
            bool res = ChartHelper.GetSelectedChartInfo(DataManager.DataManager.SharedInstance.CurrentChart, DataManager.DataManager.SharedInstance.IsMontView,
                                 DataManager.DataManager.SharedInstance.CurrentChartIndex, out smartMeterMetrics, out chartData, out dateRange, out isNormalMeter);
#else
            GetSelectedChartInfo(DataManager.DataManager.SharedInstance.CurrentChart, DataManager.DataManager.SharedInstance.IsMontView,
            out smartMeterMetrics, out chartData, out dateRange, out isNormalMeter);
#endif

            DisplayChart(chartData, dateRange, isNormalMeter, DataManager.DataManager.SharedInstance.IsMontView,
                         DataManager.DataManager.SharedInstance.CurrentChartMode, smartMeterMetrics, res);
        }

        /// <summary>
        /// Gets the selected chart info.
        /// </summary>
        /// <param name="chartModelBase">Chart model base.</param>
        /// <param name="isMonthView">If set to <c>true</c> is month view.</param>
        /// <param name="smartMeterMetric">Smart chart index.</param>
        /// <param name="chartData">Chart data.</param>
        /// <param name="dateRange">Date range.</param>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        private void GetSelectedChartInfo(ChartDataModelBase chartModelBase, bool isMonthView, out UsageMetrics smartMeterMetric,
                                          out List<SegmentDetailsModel> chartData, out string dateRange, out bool isNormalMeter)
        {
            isNormalMeter = (chartModelBase is ChartDataModel) ? true : false;
            chartData = null;
            dateRange = "Not Available";
            smartMeterMetric = null;

            if (isNormalMeter)
            {
                ChartDataModel model = chartModelBase as ChartDataModel;
                if (isMonthView)
                {
                    chartData = model.ByMonth.Months;
                    dateRange = model.ByMonth.Range;
                }
                else if (model.ByDay?.Count > 0 && DataManager.DataManager.SharedInstance.CurrentChartIndex < model.ByDay?.Count)
                {
                    chartData = model.ByDay[DataManager.DataManager.SharedInstance.CurrentChartIndex].Days;
                    dateRange = model.ByDay[DataManager.DataManager.SharedInstance.CurrentChartIndex].Range;
                }

            }
            else
            {
                SmartChartDataModel model = chartModelBase as SmartChartDataModel;

                if (isMonthView)
                {
                    if (model.ByMonth?.Count > 0 && DataManager.DataManager.SharedInstance.CurrentChartIndex < model.ByMonth?.Count)
                    {
                        chartData = model.ByMonth[DataManager.DataManager.SharedInstance.CurrentChartIndex].Months;
                        dateRange = model.ByMonth[DataManager.DataManager.SharedInstance.CurrentChartIndex].Range;
                    }

                }
                else if (model.ByDay?.Count > 0 && DataManager.DataManager.SharedInstance.CurrentChartIndex < model.ByDay?.Count)
                {
                    chartData = model.ByDay[DataManager.DataManager.SharedInstance.CurrentChartIndex].Days;
                    dateRange = model.ByDay[DataManager.DataManager.SharedInstance.CurrentChartIndex].Range;
                }

                smartMeterMetric = model.OtherUsageMetrics;

            }

            if (chartData == null)
            {
                chartData = new List<SegmentDetailsModel>();
            }
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
                _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView,
                                                                                    DataManager.DataManager.SharedInstance.CurrentChart);
#else
                _dashboardMainComponent._chartComponent.ConstructSegmentViews(chartData, isNormalMeter, chartMode);

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
                    _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView,
                                                                                    model);
#endif
                }
                else
                {
                    var model = new SmartChartDataModel();
                    model.ByMonth = new List<ByMonthModel>();
                    ByMonthModel month = new ByMonthModel();
                    ChartHelper.CreateDefaultChartData(true, out chartData, out range);
                    month.Range = range;
                    month.Months = new List<SegmentDetailsModel>(chartData);
                    model.ByMonth.Add(month);
                    ChartHelper.CreateDefaultChartData(false, out chartData, out range);
                    ByDayModel dayModel = new ByDayModel();
                    dayModel.Range = range;
                    dayModel.Days = new List<SegmentDetailsModel>(chartData);
                    model.ByDay.Add(dayModel);
#if CHART_CAROUSEL
                    _dashboardMainComponent._chartCarousel.DataSource = new ChartDataSource(_dashboardMainComponent._dashboardScrollView,
                                                                                    model);
#endif
                }

#if !CHART_CAROUSEL
                _dashboardMainComponent._chartComponent.ConstructSegmentViews(chartData, isNormalMeter, chartMode);
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
                _dashboardMainComponent._chartCompanionComponent.SetUsageMetric(smartMeterMetric);
                _dashboardMainComponent._chartCompanionComponent.SetChartMode(DataManager.DataManager.SharedInstance.CurrentChartMode);
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
                yLoc = !DeviceHelper.IsIphoneXUpResolution() ? (float)_dashboardMainComponent._viewChart.Frame.GetMaxY() + 18f
                                    : (float)_dashboardMainComponent._viewChart.Frame.GetMaxY() + 5f;
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
                _dashboardMainComponent._usageHistoryComponent.SetDateRange(rangeLabel.Text);
            }
        }


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
            chartResponse = await Task.Run(() =>
            {
                return serviceManager.GetAccountUsageHistoryForGraph("GetAccountUsageHistoryForGraph", requestParameter);
            });

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

#if true
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                accNum = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                userEmail = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                sspUserId = DataManager.DataManager.SharedInstance.UserEntity[0].userID,
                metercode = DataManager.DataManager.SharedInstance.SelectedAccount.smartMeterCode,
                isOwner = DataManager.DataManager.SharedInstance.SelectedAccount.isOwned
            };
#else // test
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                accNum = "220383318601", //DataManager.DataManager.SharedInstance.SelectedAccount.accNum
                userEmail = "test7@gmail.com",
                sspUserId = "3326BECA-2E8A-41A7-8143-A02D109F0070",
                metercode = "",
                isOwner = "true"
            };
#endif

            chartResponse = await Task.Run(() =>
            {
                return serviceManager.GetSmartMeterAccountData("GetSmartMeterAccountData", requestParameter);
            });

            return chartResponse;
        }

        /// <summary>
        /// Gets the cached chart data.
        /// </summary>
        /// <returns>The cached chart data.</returns>
        /// <param name="accountNum">Account number.</param>
        private SmartChartDataModel GetCachedChartData(string accountNum)
        {
            DateTime lastUpdate = default(DateTime);
            var model = DataManager.DataManager.SharedInstance.GetAccountUsageHistory(accountNum, ref lastUpdate);

            if (model != null && lastUpdate.Date == DateTime.Today)
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
                //Console.WriteLine("RRA: scroll: _lastContentOffset:{0}, currentOffset:{1}", _lastContentOffset, scrollView.ContentOffset.Y);
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

    }
}
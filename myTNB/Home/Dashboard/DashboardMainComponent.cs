﻿using Carousels;
using myTNB.Home.Components;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class DashboardMainComponent
    {
        UIView _parentView;
        public DashboardMainComponent(UIView view)
        {
            _parentView = view;
        }

        public GradientViewComponent _gradientViewComponent;
        public TitleBarComponent _titleBarComponent;
        public AccountSelectionComponent _accountSelectionComponent;
        public UsageHistoryComponent _usageHistoryComponent;
        public SelectorComponent _selectorComponent;
        public ChartComponent _chartComponent;
        public AddressComponent _addressComponent;
        public BillAndPaymentComponent _billAndPaymentComponent;
        public NoAccountComponent _noAccountComponent;
        public NoDataConnectionComponent _noDataConnectionComponent;
        public GetAccessComponent _getAccessComponent;
        EstimatedReadingComponent _estimatedReadingComponent;
        public SmartMeterComponent _smartMeterComponent;
        public ChartCompanionComponent _chartCompanionComponent;
        public UIView _viewChartCompanion;
        public UIView _viewSmartMeter;
        public UIView _viewChart;
        public UILabel _lblEstimatedReading;

        public UIView _gradientView;
        public UIView _billAndPaymentView;
        public UIScrollView _dashboardScrollView;
        public iCarousel _chartCarousel;
        public ChartDataSource _chartDataSource;

        internal void RemoveAllSubviews()
        {
            foreach (UIView item in _parentView)
            {
                if (item.Tag != TNBGlobal.Tags.DashboardToast)
                {
                    item.RemoveFromSuperview();
                }

            }
        }

        public void ConstructInitialView()
        {
            RemoveAllSubviews();

            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = _billAndPaymentComponent.GetUI();
            float gradientHeight = (float)(_parentView.Frame.Height - _billAndPaymentView.Frame.Height);
            _gradientViewComponent = new GradientViewComponent(_parentView, true, gradientHeight);//(_parentView, 0.68f);

            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add account selection
            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            _gradientView.AddSubview(accountSelectionView);

            _parentView.AddSubview(_gradientView);


            _parentView.AddSubview(_billAndPaymentView);

            ActivityIndicatorComponent gradientActivity = new ActivityIndicatorComponent(_gradientView);
            gradientActivity.Show();
        }

        /// <summary>
        /// Constructs the chart dashboard.
        /// </summary>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void ConstructChartDashboard(bool isNormalMeter)
        {
            RemoveAllSubviews();

            bool isFullScreen = !(isNormalMeter && DeviceHelper.IsIphoneXUpResolution());
            float percentage = isFullScreen ? 1.0f : 0.68f;

            _gradientViewComponent = new GradientViewComponent(_parentView, percentage);

            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add account selection
            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            _gradientView.AddSubview(accountSelectionView);

            // bill and payment creation
            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = !isFullScreen ? _billAndPaymentComponent.GetUI(_gradientViewComponent.GetUI().Frame.Height)
                                                : _billAndPaymentComponent.GetUI();

            int yLocation = !DeviceHelper.IsIphoneXUpResolution() ? 85 : 109;
            double contentHeight = _parentView.Frame.Height - yLocation;
            double frameHeight = _parentView.Frame.Height * 0.68f;
            _dashboardScrollView = new UIScrollView(new CoreGraphics.CGRect(0, yLocation, _gradientView.Frame.Width, contentHeight))
            {
                BackgroundColor = UIColor.Clear,
                ContentSize = new CoreGraphics.CGSize(_gradientView.Frame.Width, contentHeight + 50),
                Bounces = false
            };

            //Add UsageHistory
            _usageHistoryComponent = new UsageHistoryComponent(_dashboardScrollView);
            _dashboardScrollView.AddSubview(_usageHistoryComponent.GetUI());

            //Add Selector Bar
            _selectorComponent = new SelectorComponent(_dashboardScrollView);
            _dashboardScrollView.AddSubview(_selectorComponent.GetUI());

#if CHART_CAROUSEL // CHARTCAROUSEL
            //Add Chart
            _chartComponent = new ChartComponent(_dashboardScrollView);
            _viewChart = _chartComponent.GetUI(isNormalMeter);

            _chartCarousel = new iCarousel(_viewChart.Frame)
            {
                Type = iCarouselType.Linear,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,// | UIViewAutoresizing.FlexibleHeight,
                Bounces = false
            };

            _chartCarousel.GetValue = (sender, option, value) => {
                // set a nice spacing between items
                if (option == iCarouselOption.Spacing)
                {
                    return value * 1.5F;
                }

                // use the defaults for everything else
                return value;
            };

            _dashboardScrollView.AddSubview(_chartCarousel);

#else // original 
            //Add Chart
            _chartComponent = new ChartComponent(_dashboardScrollView);
            _viewChart = _chartComponent.GetUI(isNormalMeter);
            _dashboardScrollView.AddSubview(_viewChart);
#endif

            //Add Estimated Reading
            _estimatedReadingComponent = new EstimatedReadingComponent(_dashboardScrollView);
            _lblEstimatedReading = _estimatedReadingComponent.GetUI();
            _lblEstimatedReading.Hidden = true;
            _dashboardScrollView.AddSubview(_lblEstimatedReading);


            double frameY = _viewChart.Frame.Y + _viewChart.Frame.Height;

            // Section below chart
            _chartCompanionComponent = new ChartCompanionComponent(_dashboardScrollView, frameY);
            _viewChartCompanion = _chartCompanionComponent.GetUI();
            _dashboardScrollView.AddSubview(_viewChartCompanion);

            //Add Address
            _addressComponent = new AddressComponent(_dashboardScrollView);
            _dashboardScrollView.AddSubview(_addressComponent.GetUI());

            //Add smart meter
            //_smartMeterComponent = new SmartMeterComponent(_gradientView);
            //_viewSmartMeter = _smartMeterComponent.GetUI();
            //_viewSmartMeter.Hidden = true;
            //_dashboardScrollView.AddSubview(_viewSmartMeter);

            _gradientView.AddSubview(_dashboardScrollView);

            _parentView.AddSubview(_gradientView);
            _parentView.AddSubview(_billAndPaymentView);

        }

        public void ConstructNoAccountDashboard()
        {
            RemoveAllSubviews();

            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = _billAndPaymentComponent.GetUI();

            float gradientHeight = (float)(_parentView.Frame.Height - _billAndPaymentView.Frame.Height);
            _gradientViewComponent = new GradientViewComponent(_parentView, true, gradientHeight);//(_parentView, true, 314);
            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add no account view
            _noAccountComponent = new NoAccountComponent(_gradientView);
            _gradientView.AddSubview(_noAccountComponent.GetUI());

            _parentView.AddSubview(_gradientView);

            _billAndPaymentComponent.ToggleBillAndPayVisibility(true);
            _billAndPaymentComponent.SetPayButtonEnable(false);
            _parentView.AddSubview(_billAndPaymentView);
        }

        public void ConstructNoDataConnectionDashboard()
        {
            RemoveAllSubviews();

            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = _billAndPaymentComponent.GetUI();

            float gradientHeight = (float)(_parentView.Frame.Height - _billAndPaymentView.Frame.Height);
            _gradientViewComponent = new GradientViewComponent(_parentView, true, gradientHeight); // (_parentView, 0.68f);
            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add account selection
            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            _gradientView.AddSubview(accountSelectionView);

            //Add UsageHistory
            _usageHistoryComponent = new UsageHistoryComponent(_gradientView);
            _gradientView.AddSubview(_usageHistoryComponent.GetUI());

            _usageHistoryComponent.ToggleNavigationVisibility(true);

            //Add Selector Bar
            //_selectorComponent = new SelectorComponent(_gradientView);
            //_gradientView.AddSubview(_selectorComponent.GetUI());

            //Add No Data Connection
            _noDataConnectionComponent = new NoDataConnectionComponent(_gradientView);
            _gradientView.AddSubview(_noDataConnectionComponent.GetUI());

            //Add Address
            _addressComponent = new AddressComponent(_gradientView);
            _gradientView.AddSubview(_addressComponent.GetUI());

            _parentView.AddSubview(_gradientView);

            _parentView.AddSubview(_billAndPaymentView);
        }

        /// <summary>
        /// Constructs the get access dashboard when non-owner.
        /// </summary>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void ConstructGetAccessDashboard(bool isNormalMeter)
        {
            RemoveAllSubviews();

            bool isFullScreen = !(isNormalMeter && DeviceHelper.IsIphoneXUpResolution());
            float percentage = isFullScreen ? 1.0f : 0.68f;

            _gradientViewComponent = new GradientViewComponent(_parentView, percentage);
            //float gradientHeight = (float)(_parentView.Frame.Height - _billAndPaymentView.Frame.Height);
            //_gradientViewComponent = new GradientViewComponent(_parentView, true, gradientHeight); // (_parentView, true, 284);//316);
            _gradientView = _gradientViewComponent.GetUI();

            // bill and payment
            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = !isFullScreen ? _billAndPaymentComponent.GetUI(_gradientViewComponent.GetUI().Frame.Height)
                                                : _billAndPaymentComponent.GetUI();

            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add account selection
            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            _gradientView.AddSubview(accountSelectionView);

            //Add GetAccess view
            _getAccessComponent = new GetAccessComponent(_gradientView);
            _gradientView.AddSubview(_getAccessComponent.GetUI());

            _parentView.AddSubview(_gradientView);

            _billAndPaymentComponent.SetPayButtonEnable(true);
            _parentView.AddSubview(_billAndPaymentView);
        }

        public void ConstructEstiamtedDashboard()
        {
            RemoveAllSubviews();
            _gradientViewComponent = new GradientViewComponent(_parentView, 0.68f);
            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            //_accountSelectionComponent.SetAccountName("McDonalds KFC Jollibbee Wendy's");
            _gradientView.AddSubview(accountSelectionView);

            //Add UsageHistory
            _usageHistoryComponent = new UsageHistoryComponent(_gradientView);
            _gradientView.AddSubview(_usageHistoryComponent.GetUI());

            //Add Selector Bar
            //_selectorComponent = new SelectorComponent(_gradientView);
            //_gradientView.AddSubview(_selectorComponent.GetUI());

            //Add Estimated Reading
            _estimatedReadingComponent = new EstimatedReadingComponent(_gradientView);
            UILabel lblEstimatedReading = _estimatedReadingComponent.GetUI();
            _gradientView.AddSubview(lblEstimatedReading);

            //Add Chart
            _chartComponent = new ChartComponent(_gradientView);
            _gradientView.AddSubview(_chartComponent.GetUI());

            //Add Address
            _addressComponent = new AddressComponent(_gradientView);
            _gradientView.AddSubview(_addressComponent.GetUI());

            _parentView.AddSubview(_gradientView);

            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _billAndPaymentView = _billAndPaymentComponent.GetUI(_gradientViewComponent.GetUI().Frame.Height);
            _parentView.AddSubview(_billAndPaymentView);
        }

        /// <summary>
        /// Constructs the smart meter dashboard when usage data not yet available.
        /// </summary>
        public void ConstructSmartMeterDashboard()
        {
            RemoveAllSubviews();

            _gradientViewComponent = new GradientViewComponent(_parentView, 1.0f);
            _gradientView = _gradientViewComponent.GetUI();
            //Add Title Bar
            _titleBarComponent = new TitleBarComponent(_gradientView);
            _gradientView.AddSubview(_titleBarComponent.GetUI());

            //Add account selection
            _accountSelectionComponent = new AccountSelectionComponent(_gradientView);
            UIView accountSelectionView = _accountSelectionComponent.GetUI();
            _gradientView.AddSubview(accountSelectionView);

            //Add UsageHistory
            _usageHistoryComponent = new UsageHistoryComponent(_gradientView);
            _gradientView.AddSubview(_usageHistoryComponent.GetUI());
            int yLocation = !DeviceHelper.IsIphoneXUpResolution() ? 85 : 109;
            _usageHistoryComponent.SetFrameCustomLocationY(yLocation);
            _usageHistoryComponent.SetDateRange("Available Soon");
            _usageHistoryComponent.ToggleNavigationVisibility(true);

            //Add Selector Bar
            //_selectorComponent = new SelectorComponent(_gradientView);
            //_gradientView.AddSubview(_selectorComponent.GetUI());

            //Add smart meter
            _smartMeterComponent = new SmartMeterComponent(_gradientView);
            _gradientView.AddSubview(_smartMeterComponent.GetUI());

            //Add Address
            //_addressComponent = new AddressComponent(_gradientView);
            //_gradientView.AddSubview(_addressComponent.GetUI());

            _parentView.AddSubview(_gradientView);

            _billAndPaymentComponent = new BillAndPaymentComponent(_parentView);
            _parentView.AddSubview(_billAndPaymentComponent.GetUI());
        }
    }
}
using System;
using CoreGraphics;
using myTNB.Components;
using UIKit;

namespace myTNB.DashboardV2
{
    public class DashboardV2BaseController : CustomUIViewController
    {
        public DashboardV2BaseController(IntPtr handle) : base(handle) { }

        internal UIScrollView _scrollViewContent;
        internal CustomUIView _accountSelector, _viewSeparator, _viewStatus
            , _viewChart, _viewToggle, _viewTips;
        internal UILabel _lblAddress;

        public override void ViewDidLoad()
        {
            IsGradientRequired = true;
            IsFullGradient = true;
            IsReversedGradient = true;
            base.ViewDidLoad();
            Title = "Usage";
            if (TabBarController != null && TabBarController.TabBar != null)
            { TabBarController.TabBar.Layer.ZPosition = -1; }
            AddCustomNavBar();
            AddScrollView();
            AddSubviews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(true, animated);
            if (TabBarController != null && TabBarController.TabBar != null)
            { TabBarController.TabBar.Layer.ZPosition = 0; }
        }

        private void AddScrollView()
        {
            nfloat height = UIScreen.MainScreen.Bounds.Height - _customNavBar.Frame.GetMaxY();
            _scrollViewContent = new UIScrollView(new CGRect(0, _customNavBar.Frame.GetMaxY(), ViewWidth, height))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_scrollViewContent);

            _accountSelector = new CustomUIView(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)));// { BackgroundColor = UIColor.Blue };
            _lblAddress = new UILabel(new CGRect(BaseMargin, 0, BaseMarginedWidth, 0))
            {
                //BackgroundColor = UIColor.Red,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = MyTNBFont.MuseoSans10_300V2
            };
            _viewSeparator = new CustomUIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };
            _viewStatus = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewChart = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _viewToggle = new CustomUIView(new CGRect(0, 0, ViewWidth, 0)) { BackgroundColor = UIColor.Green };
            _viewTips = new CustomUIView(new CGRect(0, 0, ViewWidth, 0)) { BackgroundColor = UIColor.Cyan };

            _scrollViewContent.AddSubviews(new UIView[] { _accountSelector
                , _lblAddress, _viewSeparator, _viewStatus, _viewChart, _viewTips });
        }

        private void SetContentView()
        {
            _lblAddress.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_accountSelector.Frame, 8F)), _lblAddress.Frame.Size);
            _viewSeparator.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_lblAddress.Frame, 16F)), _viewSeparator.Frame.Size);
            _viewStatus.Frame = new CGRect(new CGPoint(BaseMargin, GetYLocationFromFrame(_viewSeparator.Frame, 16F)), _viewStatus.Frame.Size);
            _viewChart.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewStatus.Frame, 16F)), _viewChart.Frame.Size);
            _viewToggle.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewChart.Frame, 16F)), _viewToggle.Frame.Size);
            _viewTips.Frame = new CGRect(new CGPoint(0, GetYLocationFromFrame(_viewToggle.Frame, 24F)), _viewTips.Frame.Size);

            _scrollViewContent.ContentSize = new CGSize(ViewWidth, _viewTips.Frame.GetMaxY());
        }

        private void AddSubviews()
        {
            AddAccountSelector();
            SetAddress();
            SetChartView();
            SetContentView();
        }

        private void AddAccountSelector()
        {
            AccountSelector accountSelector = new AccountSelector();
            CustomUIView viewAccountSelector = accountSelector.GetUI();
            accountSelector.SetAction(null);
            accountSelector.Title = AccountManager.Instance.Nickname;
            _accountSelector.AddSubview(viewAccountSelector);
        }

        private void SetAddress()
        {
            _lblAddress.Text = AccountManager.Instance.Address.ToUpper();
            CGSize lblSize = GetLabelSize(_lblAddress, GetScaledHeight(42));
            CGRect lblFrame = _lblAddress.Frame;
            lblFrame.Height = lblSize.Height;
            _lblAddress.Frame = lblFrame;
        }

        private void SetChartView()
        {
            ChartView chartView = new ChartView();
            CustomUIView chart = chartView.GetUI();
            _viewChart.AddSubview(chart);
            CGRect chartFrame = _viewChart.Frame;
            chartFrame.Size = new CGSize(ViewWidth, chart.Frame.Height);
            _viewChart.Frame = chartFrame;
        }
    }
}
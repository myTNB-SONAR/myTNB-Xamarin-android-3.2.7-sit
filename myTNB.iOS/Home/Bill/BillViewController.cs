using System;
using CoreAnimation;
using CoreGraphics;
using myTNB.Home.Bill;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public partial class BillViewController : CustomUIViewController
    {
        private UIView _headerViewContainer, _headerView, _footerView, _navbarView, _viewRefreshContainer;
        private UIImageView _bgImageView;
        private CAGradientLayer _gradientLayer;
        private CustomUIView _accountSelectorContainer;
        private nfloat _navBarHeight, _previousScrollOffset;
        private nfloat titleBarHeight = 24f;

        public BillViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            NavigationController.NavigationBarHidden = true;
            PageName = BillConstants.Pagename_Bills;
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            SetNavigation();
            SetHeaderView();
            AddAccountSelector();
            AddTableView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            // NavigationController.SetNavigationBarHidden(false, true);
        }

        private void SetNavigation()
        {
            if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.Hidden = true;
                _navBarHeight = NavigationController.NavigationBar.Frame.Height;
            }

            _navbarView = new UIView(new CGRect(0, 0, ViewWidth, DeviceHelper.GetStatusBarHeight() + _navBarHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_ReadingHistoryBanner),
                BackgroundColor = UIColor.White
            };

            View.AddSubview(_bgImageView);
            View.SendSubviewToBack(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(12)
                , _navbarView.Frame.Width, GetScaledHeight(24)));
            UILabel lblTitle = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = "Bills",
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };
            viewTitleBar.AddSubview(lblTitle);
            _navbarView.AddSubview(viewTitleBar);

            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            _gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            _gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            _gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            _gradientLayer.Frame = _navbarView.Bounds;
            _gradientLayer.Opacity = 0f;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }

        private UILabel _lblPaymentStatus, _lblCurrency, _lblAmount, _lblDate;
        private UIView _viewAmount, _viewCTA;
        private CustomUIButtonV2 _btnMore, _btnPay;
        private void SetHeaderView()
        {
            _headerViewContainer = new CustomUIView(new CGRect(0, 0, ViewWidth, 0));
            _accountSelectorContainer = new CustomUIView(new CGRect(0, GetScaledHeight(8), ViewWidth, GetScaledHeight(24)));
            _headerView = new CustomUIView(new CGRect(0, _bgImageView.Frame.GetMaxY()
                - (DeviceHelper.GetStatusBarHeight() + _navBarHeight), ViewWidth, 0))
            { BackgroundColor = UIColor.White };
            _lblPaymentStatus = new UILabel(new CGRect(0, GetScaledHeight(16), ViewWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_14_500,
                TextAlignment = UITextAlignment.Center,
                Text = "I need to pay"
            };
            _viewAmount = new UIView(new CGRect(0, GetYLocationFromFrame(_lblPaymentStatus.Frame, 8)
                , ViewWidth, GetScaledHeight(36)));

            _lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(16), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_16_500,
                TextAlignment = UITextAlignment.Right,
                Text = "RM"
            };

            _lblAmount = new UILabel(new CGRect(0, 0, GetScaledWidth(100), GetScaledHeight(36)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_36_500,
                TextAlignment = UITextAlignment.Left,
                Text = "200.00"
            };

            UpdateViewAmount();

            _lblDate = new UILabel(new CGRect(0, GetYLocationFromFrame(_viewAmount.Frame, 8), ViewWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.GreyishBrown,
                Font = TNBFont.MuseoSans_14_300,
                TextAlignment = UITextAlignment.Center,
                Text = "by 24 Sep 2019"
            };
            nfloat btnWidth = (BaseMarginedWidth - GetScaledWidth(4)) / 2;
            _viewCTA = new UIView(new CGRect(0, GetYLocationFromFrame(_lblDate.Frame, 24), ViewWidth, GetScaledHeight(48)));
            _btnMore = new CustomUIButtonV2
            {
                Frame = new CGRect(BaseMargin, 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = UIColor.White
            };
            _btnMore.SetTitle("View More", UIControlState.Normal);
            _btnMore.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnMore.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

            _btnPay = new CustomUIButtonV2()
            {
                Frame = new CGRect(_btnMore.Frame.GetMaxX() + GetScaledWidth(4), 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnPay.SetTitle("Pay", UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);

            _viewCTA.AddSubviews(new CustomUIButtonV2[] { _btnMore, _btnPay });

            _viewAmount.AddSubviews(new UIView[] { _lblCurrency, _lblAmount });
            _headerView.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA });
            _headerViewContainer.AddSubviews(_headerView);//.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA });
            _headerViewContainer.AddSubviews(_accountSelectorContainer);

            CGRect frame = _headerView.Frame;
            frame.Height = GetYLocationFromFrame(_viewCTA.Frame, 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
        }

        private void UpdateViewAmount()
        {
            nfloat currencyWidth = _lblCurrency.GetLabelWidth(GetScaledWidth(ViewWidth / 2));
            _lblCurrency.Frame = new CGRect(0, _lblCurrency.Frame.Y, currencyWidth, _lblCurrency.Frame.Height);

            nfloat amountWidth = _lblAmount.GetLabelWidth(GetScaledWidth(ViewWidth - currencyWidth));
            _lblAmount.Frame = new CGRect(_lblCurrency.Frame.GetMaxX() + GetScaledWidth(6)
                , _lblAmount.Frame.Y, amountWidth, _lblAmount.Frame.Height);

            nfloat newXLoc = (ViewWidth - (currencyWidth + amountWidth + GetScaledWidth(6))) / 2;
            _viewAmount.Frame = new CGRect(newXLoc, _viewAmount.Frame.Y, _lblAmount.Frame.GetMaxY(), _viewAmount.Frame.Height);
        }

        private AccountSelector _accountSelector;
        private CustomUIView _viewAccountSelector;
        private void AddAccountSelector()
        {
            _accountSelector = new AccountSelector();
            _viewAccountSelector = _accountSelector.GetUI();
            _accountSelector.SetAction(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                //viewController.OnSelect = OnSelectAccount;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            });
            _accountSelectorContainer.AddSubview(_viewAccountSelector);
            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
        }

        private UITableView _historyTableView;
        private void AddTableView()
        {
            nfloat height = View.Frame.Height - _navbarView.Frame.Height - TabBarController.TabBar.Frame.Height;
            _historyTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, height));
            //_historyTableView.RegisterClassForCellReuse(typeof(SSMRReadingHistoryCell), SSMRConstants.Cell_ReadingHistory);
            _historyTableView.Source = new BillHistorySource();
            _historyTableView.BackgroundColor = UIColor.Clear;
            _historyTableView.RowHeight = UITableView.AutomaticDimension;
            //_historyTableView.EstimatedRowHeight = GetScaledHeight(68);
            _historyTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _historyTableView.Bounces = false;
            //_historyTableView.SectionFooterHeight = 0;
            _historyTableView.TableHeaderView = _headerViewContainer;
            //_historyTableView.TableFooterView = _footerView;
            View.AddSubview(_historyTableView);


            _historyTableView.Layer.BorderWidth = 1;
            _historyTableView.Layer.BorderColor = UIColor.Red.CGColor;
        }
    }
}

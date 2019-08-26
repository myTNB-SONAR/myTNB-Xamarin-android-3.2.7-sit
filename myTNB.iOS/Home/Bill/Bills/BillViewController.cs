using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
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
        private nfloat _tableViewOffset;
        private UITableView _historyTableView;
        private UILabel _lblPaymentStatus, _lblCurrency, _lblAmount, _lblDate;
        private UIView _viewAmount, _viewCTA;
        private CustomUIButtonV2 _btnMore, _btnPay;
        private AccountSelector _accountSelector;
        private CustomUIView _viewAccountSelector;
        private const string ParseFormat = "yyyyMMdd";
        private const string DateFormat = "dd MMM yyyy";

        public BillViewController(IntPtr handle) : base(handle) { }

        #region Life Cycle
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
            OnSelectAccount(0);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            // NavigationController.SetNavigationBarHidden(false, true);
        }
        #endregion

        #region Navigation
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
            _tableViewOffset = DeviceHelper.GetStatusBarHeight() + _navBarHeight;

            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                //Bills-Cleared-Banner
                //Bills-NeedToPay-Banner
                Image = UIImage.FromBundle("Bills-Cleared-Banner"),
                BackgroundColor = UIColor.White
            };

            View.AddSubview(_bgImageView);
            View.SendSubviewToBack(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(12)
                , _navbarView.Frame.Width, GetScaledHeight(24)));
            UILabel lblTitle = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(BillConstants.I18N_NavTitle),
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
        #endregion

        #region Header
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
                Text = GetI18NValue(BillConstants.I18N_NeedToPay)
            };
            _viewAmount = new UIView(new CGRect(0, GetYLocationFromFrame(_lblPaymentStatus.Frame, 8)
                , ViewWidth, GetScaledHeight(36)));

            _lblCurrency = new UILabel(new CGRect(0, GetScaledHeight(16), GetScaledWidth(100), GetScaledHeight(18)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_16_500,
                TextAlignment = UITextAlignment.Right,
                Text = TNBGlobal.UNIT_CURRENCY
            };

            _lblAmount = new UILabel(new CGRect(0, 0, GetScaledWidth(100), GetScaledHeight(36)))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                Font = TNBFont.MuseoSans_36_500,
                TextAlignment = UITextAlignment.Left,
                Text = "200.00"
            };
            _viewAmount.AddSubviews(new UIView[] { _lblCurrency, _lblAmount });
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
            _btnMore.SetTitle(GetI18NValue(BillConstants.I18N_ViewMore), UIControlState.Normal);
            _btnMore.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnMore.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnMore.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
                BillDetailsViewController viewController =
                    storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }));

            _btnPay = new CustomUIButtonV2()
            {
                Frame = new CGRect(_btnMore.Frame.GetMaxX() + GetScaledWidth(4), 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnPay.SetTitle(GetCommonI18NValue(BillConstants.I18N_Pay), UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);

            _viewCTA.AddSubviews(new CustomUIButtonV2[] { _btnMore, _btnPay });

            _headerView.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA });
            _headerViewContainer.AddSubviews(_headerView);//.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA });
            _headerViewContainer.AddSubviews(_accountSelectorContainer);

            CGRect frame = _headerView.Frame;
            frame.Height = GetYLocationFromFrame(_viewCTA.Frame, 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
        }

        private void UpdateViewAmount(bool isExtra = false)
        {
            nfloat currencyWidth = _lblCurrency.GetLabelWidth(GetScaledWidth(ViewWidth / 2));
            _lblCurrency.Frame = new CGRect(0, _lblCurrency.Frame.Y, currencyWidth, _lblCurrency.Frame.Height);

            nfloat amountWidth = _lblAmount.GetLabelWidth(GetScaledWidth(ViewWidth - currencyWidth));
            _lblAmount.Frame = new CGRect(_lblCurrency.Frame.GetMaxX() + GetScaledWidth(6)
                , _lblAmount.Frame.Y, amountWidth, _lblAmount.Frame.Height);

            nfloat newXLoc = (ViewWidth - (currencyWidth + amountWidth + GetScaledWidth(6))) / 2;
            _viewAmount.Frame = new CGRect(newXLoc, _viewAmount.Frame.Y, _lblAmount.Frame.GetMaxY(), _viewAmount.Frame.Height);

            _lblCurrency.TextColor = isExtra ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey;
            _lblAmount.TextColor = isExtra ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey;
        }

        private void AddAccountSelector()
        {
            _accountSelector = new AccountSelector();
            _viewAccountSelector = _accountSelector.GetUI();
            _accountSelector.SetAction(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                viewController.OnSelect = OnSelectAccount;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            });
            _accountSelectorContainer.AddSubview(_viewAccountSelector);
            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
        }
        #endregion

        private void OnSelectAccount(int index)
        {
            if (_accountSelector != null)
            {
                _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;//AccountManager.Instance.Nickname;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
               {
                   if (NetworkUtility.isReachable)
                   {
                       InvokeInBackground(async () =>
                       {
                           GetAccountsChargesResponseModel accountCharges = await GetAccountsCharges();
                           InvokeOnMainThread(() =>
                           {
                               if (accountCharges != null && accountCharges.d != null && accountCharges.d.IsSuccess
                                    && accountCharges.d.data != null && accountCharges.d.data.AccountCharges != null
                                    && accountCharges.d.data.AccountCharges.Count > 0 && accountCharges.d.data.AccountCharges[0] != null)
                               {
                                   UpdateHeaderData(accountCharges.d.data.AccountCharges[0]);
                               }
                           });
                       });
                   }
                   else
                   {
                       DisplayNoDataAlert();
                   }
               });
            });
        }

        private void UpdateHeaderData(AccountChargesModel data)
        {
            //Bills-Cleared-Banner
            //Bills-NeedToPay-Banner
            _lblAmount.Text = Math.Abs(data.AmountDue).ToString("N2", CultureInfo.InvariantCulture);
            CGRect ctaFrame = _viewCTA.Frame;
            _bgImageView.Image = UIImage.FromBundle(data.AmountDue > 0 ? BillConstants.IMG_NeedToPay : BillConstants.IMG_Cleared);

            if (data.AmountDue > 0)
            {
                _lblPaymentStatus.Text = GetI18NValue(BillConstants.I18N_NeedToPay);
                string result = DateTime.ParseExact(data.DueDate, ParseFormat, CultureInfo.InvariantCulture).ToString(DateFormat);
                _lblDate.Text = string.Format("{0} {1}", GetI18NValue(BillConstants.I18N_By), result);
                _lblDate.Hidden = false;

                ctaFrame.Y = GetYLocationFromFrame(_lblDate.Frame, 24);
            }
            else
            {
                _lblPaymentStatus.Text = GetI18NValue(data.AmountDue == 0 ? BillConstants.I18N_ClearedBills : BillConstants.I18N_PaidExtra);
                _lblDate.Hidden = true;
                ctaFrame.Y = GetYLocationFromFrame(_viewAmount.Frame, 24);
            }
            UpdateViewAmount(data.AmountDue < 0);

            _viewCTA.Frame = ctaFrame;

            CGRect frame = _headerView.Frame;
            frame.Height = GetYLocationFromFrame(_viewCTA.Frame, 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));

            AddGroupedDate();
            _historyTableView.ReloadData();
        }

        #region Table
        private void AddTableView()
        {
            nfloat height = View.Frame.Height - _navbarView.Frame.Height - TabBarController.TabBar.Frame.Height;
            _historyTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, height));
            _historyTableView.RegisterClassForCellReuse(typeof(BillHistoryViewCell), BillConstants.Cell_BillHistory);
            _historyTableView.RegisterClassForCellReuse(typeof(BillSectionViewCell), BillConstants.Cell_BillSection);
            _historyTableView.Source = new BillHistorySource() { OnTableViewScroll = OnTableViewScroll };
            _historyTableView.BackgroundColor = UIColor.Clear;
            _historyTableView.RowHeight = UITableView.AutomaticDimension;
            _historyTableView.EstimatedRowHeight = GetScaledHeight(90);
            _historyTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _historyTableView.Bounces = false;
            _historyTableView.TableHeaderView = _headerViewContainer;
            View.AddSubview(_historyTableView);
            //_historyTableView.Layer.BorderWidth = 1;
            //_historyTableView.Layer.BorderColor = UIColor.Red.CGColor;
            AddGroupedDate();
        }

        private void AddGroupedDate()
        {
            return;
            for (int j = 0; j < _historyTableView.Subviews.Length; j++)
            {
                if (_historyTableView.Subviews[j].Tag == 101)
                {
                    _historyTableView.Subviews[j].RemoveFromSuperview();
                }
            }
            for (int i = 0; i < 10; i++)
            {
                //if (i == 0) { continue; }

                //if (i == 0 || i == 3)
                // {
                CGRect cellRect = _historyTableView.RectForRowAtIndexPath(NSIndexPath.Create(0, i));
                nfloat yLoc = cellRect.GetMinY();// + GetScaledHeight(51);

                UIView viewGroupedDate = new UIView(new CGRect(ScaleUtility.GetScaledWidth(16), yLoc
                    , ScaleUtility.GetScaledWidth(70), ScaleUtility.GetScaledHeight(24)))
                {
                    ClipsToBounds = true,
                    Tag = 101
                };
                UILabel _lblGroupedDate = new UILabel(new CGRect(new CGPoint(0, 0), viewGroupedDate.Frame.Size))
                {
                    BackgroundColor = MyTNBColor.WaterBlue,
                    TextColor = UIColor.White,
                    Font = TNBFont.MuseoSans_12_500,
                    TextAlignment = UITextAlignment.Center,
                    Text = "Aug 2019" + i
                };
                viewGroupedDate.AddSubview(_lblGroupedDate);
                viewGroupedDate.Layer.CornerRadius = ScaleUtility.GetScaledHeight(12);
                viewGroupedDate.Layer.ZPosition = 99;
                _historyTableView.AddSubview(viewGroupedDate);
                _historyTableView.BringSubviewToFront(viewGroupedDate);
                // }
            }
        }
        #endregion

        #region Scroll Events
        public void OnTableViewScroll(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            CGRect frame = _bgImageView.Frame;
            if ((nfloat)Math.Abs(frame.Y) == frame.Height) { return; }

            nfloat newYLoc = 0 - scrollView.ContentOffset.Y;
            frame.Y = newYLoc;
            _bgImageView.Frame = frame;

            _previousScrollOffset = _historyTableView.ContentOffset.Y;
            var opac = _previousScrollOffset / _tableViewOffset;
            var absOpacity = Math.Abs((float)opac);
            AddViewWithOpacity(absOpacity);
        }

        private void AddViewWithOpacity(float opacity)
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            var gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor }
            };
            gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            gradientLayer.Frame = _navbarView.Bounds;
            gradientLayer.Opacity = opacity;
            _navbarView.Layer.ReplaceSublayer(_gradientLayer, gradientLayer);
            _gradientLayer = gradientLayer;
        }
        #endregion

        #region Services
        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                accounts = new List<string> { DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty }
            };
            GetAccountsChargesResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountsChargesResponseModel>("GetAccountsCharges", request);
            return response;
        }

        #endregion
    }
}

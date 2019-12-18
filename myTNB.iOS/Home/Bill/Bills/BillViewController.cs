using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Force.DeepCloner;
using Foundation;
using myTNB.Home.Bill;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public partial class BillViewController : CustomUIViewController
    {
        private UIView _headerViewContainer, _headerView, _navbarView
            , _shimmerView, _viewRefreshContainer;
        private UIImageView _bgImageView, _imgFilter;
        private CAGradientLayer _gradientLayer;
        private CustomUIView _accountSelectorContainer, _viewFilter;
        private nfloat _navBarHeight;
        public nfloat _previousScrollOffset;
        private nfloat _tableViewOffset;
        public UITableView _historyTableView;
        private UILabel _lblPaymentStatus, _lblCurrency, _lblAmount, _lblDate, _lblNavTitle;
        private UIView _viewAmount, _viewCTA;
        private CustomUIButtonV2 _btnMore, _btnPay;
        private AccountSelector _accountSelector;
        private CustomUIView _viewAccountSelector;

        private GetAccountsChargesResponseModel _accountCharges;
        private GetAccountBillPayHistoryResponseModel _billHistory;
        private bool _isBCRMAvailable;
        private List<string> FilterTypes = new List<string>();
        private List<string> FilterKeys = new List<string>();
        private int FilterIndex = 0;
        public bool NeedsUpdate = true;
        private UIView _tutorialContainer;
        private bool isGetAcctChargesLoading = true, isGetAcctBillPayHistoryLoading = true;

        private CGRect DefaultBannerRect;
        private CGRect FailBannerRect;

        public BillViewController(IntPtr handle) : base(handle) { }

        #region Life Cycle
        public override void ViewDidLoad()
        {
            foreach (UIView v in View.Subviews)
            {
                v.RemoveFromSuperview();
            }
            if (NavigationController != null) { NavigationController.NavigationBarHidden = true; }
            PageName = BillConstants.Pagename_Bills;
            base.ViewDidLoad();
            if ((DataManager.DataManager.SharedInstance.AccountRecordsList != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
                && (DataManager.DataManager.SharedInstance.SelectedAccount == null
                || string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.SelectedAccount.accNum)
                || string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.SelectedAccount.accNum)))
            {
                DataManager.DataManager.SharedInstance.SelectedAccount = DataManager.DataManager.SharedInstance.AccountRecordsList.d[0];
            }
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            _isBCRMAvailable = true;// DataManager.DataManager.SharedInstance.IsBcrmAvailable;
            View.BackgroundColor = UIColor.White;

            DefaultBannerRect = new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F);
            FailBannerRect = new CGRect(0, 0, ViewWidth, ViewWidth * 0.76875F);

            SetNavigation();
            SetHeaderView();
            AddAccountSelector();
            AddTableView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NeedsUpdate)
            {
                if (_isBCRMAvailable)
                {
                    OnSelectAccount(0);
                }
                else
                {
                    _historyTableView.Hidden = true;
                    DisplayRefresh();
                }
            }
            else
            {
                isGetAcctChargesLoading = false;
                isGetAcctBillPayHistoryLoading = false;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void OnEnterForeground(NSNotification notification)
        {
            NeedsUpdate = true;
            ViewWillAppear(true);
        }
        #endregion

        #region Tutorial Overlay Methods
        private void CheckTutorialOverlay()
        {
            bool isRE = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            string keyString = isRE ? BillConstants.Pref_BillTutorialRElOverlay : BillConstants.Pref_BillTutorialNormalOverlay;
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            bool tutorialOverlayHasShown = sharedPreference.BoolForKey(keyString);

            if (tutorialOverlayHasShown)
                return;

            if (!isGetAcctChargesLoading && !isGetAcctBillPayHistoryLoading)
            {
                InvokeOnMainThread(() =>
                {
                    UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is BillViewController)
                        {
                            ShowTutorialOverlay();
                        }
                    }
                });
            }
        }

        private void ShowTutorialOverlay()
        {
            if (_tutorialContainer != null)
                return;

            ScrollTableToTheTop();
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;

            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            currentWindow.AddSubview(_tutorialContainer);

            BillTutorialOverlay tutorialView = new BillTutorialOverlay(_tutorialContainer, this)
            {
                GetI18NValue = GetI18NValue,
                TabBarHeight = TabBarController.TabBar.Frame.Height,
                NavigationHeight = DeviceHelper.GetStatusBarHeight() + _navBarHeight,
                HeaderViewHeight = _headerViewContainer.Frame.Height,
                ViewCTA = _viewCTA,
                ViewCTAMinY = _headerView.Frame.GetMinY() + _viewCTA.Frame.GetMinY(),
                IsREAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount,
                OnDismissAction = HideTutorialOverlay,
                ScrollTableToTheTop = ScrollTableToTheTop,
                ScrollToHistorySection = ScrollToHistorySection
            };
            UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is BillViewController)
                {
                    _tutorialContainer.AddSubview(tutorialView.GetView());
                }
            }
        }

        private void HideTutorialOverlay()
        {
            ScrollTableToTheTop();
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);
                _tutorialContainer = null;

                bool isRE = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
                string keyString = isRE ? BillConstants.Pref_BillTutorialRElOverlay : BillConstants.Pref_BillTutorialNormalOverlay;
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, keyString);
            }
        }

        public void ScrollTableToTheTop()
        {
            _historyTableView.SetContentOffset(new CGPoint(0, 0), false);
        }

        private void ScrollToHistorySection()
        {
            int count = _billHistory?.d?.data?.BillPayHistories?.Count ?? 0;
            if (count > 1)
            {
                _historyTableView.ScrollToRow(NSIndexPath.FromRowSection(2, 0), UITableViewScrollPosition.Bottom, false);
            }
            else if (count > 0)
            {
                _historyTableView.ScrollToRow(NSIndexPath.FromRowSection(1, 0), UITableViewScrollPosition.Bottom, false);
            }
            else
            {
                _historyTableView.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, false);
            }
        }

        public nfloat GetDateAmountMaxY
        {
            get
            {
                return _headerView.Frame.GetMinY() + (_lblDate.Hidden ? _viewAmount.Frame.GetMaxY() : _lblDate.Frame.GetMaxY());
            }
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

            _bgImageView = new UIImageView(DefaultBannerRect)
            {
                Image = UIImage.FromBundle(BillConstants.IMG_Cleared),
                BackgroundColor = UIColor.White
            };

            View.AddSubview(_bgImageView);
            View.SendSubviewToBack(_bgImageView);

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(4)
                , _navbarView.Frame.Width, GetScaledHeight(24)));
            _lblNavTitle = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(BillConstants.I18N_NavTitle),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };
            _viewFilter = new CustomUIView(new CGRect(_navbarView.Frame.Width - GetScaledWidth(32), 0
                , GetScaledWidth(16), GetScaledWidth(16)))
            { Hidden = true };
            _imgFilter = new UIImageView(new CGRect(0, 0, GetScaledWidth(24), GetScaledWidth(24)))
            {
                Image = UIImage.FromBundle("IC-Action-Nav-Unfiltered")
            };
            _viewFilter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ShowFilterScreen();
            }));
            _viewFilter.AddSubview(_imgFilter);
            viewTitleBar.AddSubview(_lblNavTitle);
            viewTitleBar.AddSubview(_viewFilter);
            _navbarView.AddSubview(viewTitleBar);

            UIColor startColor = MyTNBColor.GradientPurpleDarkElement;
            UIColor endColor = MyTNBColor.GradientPurpleLightElement;
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
                Font = TNBFont.MuseoSans_36_300,
                TextAlignment = UITextAlignment.Left,
                Text = string.Empty
            };
            _viewAmount.AddSubviews(new UIView[] { _lblCurrency, _lblAmount });
            UpdateViewAmount();

            _lblDate = new UILabel(new CGRect(0, GetYLocationFromFrame(_viewAmount.Frame, 8), ViewWidth, GetScaledHeight(20)))
            {
                TextColor = MyTNBColor.GreyishBrown,
                Font = TNBFont.MuseoSans_14_300,
                TextAlignment = UITextAlignment.Center,
                Text = string.Empty
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
                NeedsUpdate = false;
                UIStoryboard storyBoard = UIStoryboard.FromName("BillDetails", null);
                BillDetailsViewController viewController =
                    storyBoard.InstantiateViewController("BillDetailsView") as BillDetailsViewController;
                if (viewController != null)
                {
                    viewController.AccountNumber = DataManager.DataManager.SharedInstance.SelectedAccount.accNum;
                    UINavigationController navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }
            }));

            _btnPay = new CustomUIButtonV2
            {
                Frame = new CGRect(_btnMore.Frame.GetMaxX() + GetScaledWidth(4), 0, btnWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnPay.SetTitle(GetI18NValue(BillConstants.I18N_Pay), UIControlState.Normal);
            _btnPay.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnPay.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                {
                    InvokeOnMainThread(() =>
                    {
                        if (NetworkUtility.isReachable)
                        {
                            NeedsUpdate = false;
                            UIStoryboard storyBoard = UIStoryboard.FromName("Payment", null);
                            SelectBillsViewController selectBillsVC =
                                storyBoard.InstantiateViewController("SelectBillsViewController") as SelectBillsViewController;
                            if (selectBillsVC != null)
                            {
                                UINavigationController navController = new UINavigationController(selectBillsVC);
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                PresentViewController(navController, true, null);
                            }
                        }
                        else
                        {
                            DisplayNoDataAlert();
                        }
                    });
                });
            }));

            if (AppLaunchMasterCache.IsPayDisabled)
            {
                _btnPay.Enabled = false;
                _btnPay.BackgroundColor = MyTNBColor.SilverChalice;
            }

            _viewCTA.AddSubviews(new CustomUIButtonV2[] { _btnMore, _btnPay });

            _shimmerView = GetShimmerView();

            _headerView.AddSubviews(new UIView[] { _lblPaymentStatus, _viewAmount, _lblDate, _viewCTA, _shimmerView });
            _headerViewContainer.AddSubviews(_headerView);
            _headerViewContainer.AddSubviews(_accountSelectorContainer);

            //Test
            _headerView.Layer.BorderColor = UIColor.Red.CGColor;
            _headerView.Layer.BorderWidth = 1;
            _accountSelectorContainer.Layer.BorderColor = UIColor.Green.CGColor;
            _accountSelectorContainer.Layer.BorderWidth = 1;

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
            _accountSelector = new AccountSelector(_accountSelectorContainer);
            _viewAccountSelector = _accountSelector.GetUI();
            _accountSelector.SetAction(() =>
            {
                NeedsUpdate = false;
                UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
                SelectAccountTableViewController viewController =
                    storyBoard.InstantiateViewController("SelectAccountTableViewController") as SelectAccountTableViewController;
                viewController.OnSelect = OnSelectAccount;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            });
            _accountSelectorContainer.AddSubview(_viewAccountSelector);
            _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;
        }

        private UIView GetShimmerView()
        {
            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(BaseMargin, 0, BaseMarginedWidth, GetScaledHeight(108))) { BackgroundColor = UIColor.White };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), viewParent.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), viewParent.Frame.Size)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView viewStatus = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(104)) / 2
                , GetScaledHeight(16), ScaleUtility.GetWidthByScreenSize(104), GetScaledHeight(20)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            UIView viewAmt = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(192)) / 2
                , GetYLocationFromFrame(viewStatus.Frame, 8), ScaleUtility.GetWidthByScreenSize(192), GetScaledHeight(36)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            UIView viewDate = new UIView(new CGRect((BaseMarginedWidth - ScaleUtility.GetWidthByScreenSize(104)) / 2
                , GetYLocationFromFrame(viewAmt.Frame, 8), ScaleUtility.GetWidthByScreenSize(104), GetScaledHeight(20)))
            { BackgroundColor = MyTNBColor.PaleGrey };

            viewShimmerContent.AddSubviews(new UIView[] { viewStatus, viewAmt, viewDate });

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return viewParent;
        }

        private void SetHeaderLoading(bool isLoading)
        {
            if (isLoading)
            {
                _bgImageView.Image = UIImage.FromBundle(BillConstants.IMG_LoadingBanner);
            }
            _shimmerView.Hidden = !isLoading;
            _viewCTA.Hidden = isLoading;
            _viewCTA.Hidden = !isLoading;
            if (isLoading)
            {
                _viewCTA.Frame = new CGRect(new CGPoint(_viewCTA.Frame.X
                    , GetYLocationFromFrame(_shimmerView.Frame, 16)), _viewCTA.Frame.Size);

                CGRect frame = _headerView.Frame;
                frame.Height = GetYLocationFromFrame(_shimmerView.Frame, 16);
                _headerView.Frame = frame;

                _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
               , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
            }

            _historyTableView.ReloadData();
            _historyTableView.Hidden = false;
            OnResetBGRect();
        }
        #endregion

        #region Refresh
        private void DisplayRefresh()
        {
            _historyTableView.Hidden = true;
            string errMessage = GetCommonI18NValue(SSMRConstants.I18N_RefreshDescription);
            if (!_isBCRMAvailable)
            {
                DowntimeDataModel bcrm = DataManager.DataManager.SharedInstance.SystemStatus?.Find(x => x.SystemType == Enums.SystemEnum.BCRM);
                errMessage = bcrm?.DowntimeMessage ?? GetI18NValue(BillConstants.I18N_BcrmDownMessage);
            }

            _bgImageView.Image = UIImage.FromBundle(_isBCRMAvailable ? BillConstants.IMG_Refresh : BillConstants.IMG_BCRMDown);
            if (_viewRefreshContainer != null) { _viewRefreshContainer.RemoveFromSuperview(); }
            _viewRefreshContainer = new UIView()
            { Tag = 10, BackgroundColor = UIColor.White };

            // HTML / Plain text for UITextView
            NSError htmlBodyError = null;
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.PowerBlue,
                Font = TNBFont.MuseoSans_14_300,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };

            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(errMessage
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(14));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.Grey
            }, new NSRange(0, htmlBody.Length));

            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;

            //Resize
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(BaseMarginedWidth, ViewHeight));
            txtViewDetails.Frame = new CGRect(BaseMargin, GetScaledHeight(16), BaseMarginedWidth, size.Height);
            txtViewDetails.TextAlignment = UITextAlignment.Center;
            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                RedirectAlert(url);
            });
            txtViewDetails.Delegate = new TextViewDelegate(action);
            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, GetYLocationFromFrame(txtViewDetails.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_Refresh,
                Hidden = !_isBCRMAvailable
            };
            btnRefresh.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_RefreshNow), UIControlState.Normal);
            btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                // call services again
                OnSelectAccount(0);
                if (_viewRefreshContainer != null)
                {
                    _viewRefreshContainer.RemoveFromSuperview();
                }
            }));

            _viewRefreshContainer.AddSubview(txtViewDetails);
            _viewRefreshContainer.AddSubview(btnRefresh);
            _viewRefreshContainer.Frame = new CGRect(0, _bgImageView.Frame.GetMaxY(), ViewWidth, ViewHeight);
            View.AddSubview(_viewRefreshContainer);
        }

        private void RedirectAlert(NSUrl url)
        {
            string absURL = url?.AbsoluteString;
            if (!string.IsNullOrEmpty(absURL))
            {
                int whileCount = 0;
                bool isContained = false;
                while (!isContained && whileCount < AlertHandler.RedirectTypeList.Count)
                {
                    isContained = absURL.Contains(AlertHandler.RedirectTypeList[whileCount]);
                    if (isContained) { break; }
                    whileCount++;
                }

                if (isContained)
                {
                    if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[0])
                    {
                        string key = absURL.Split(AlertHandler.RedirectTypeList[0])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        ViewHelper.GoToFAQScreenWithId(key);
                    }
                    else if (AlertHandler.RedirectTypeList[whileCount] == AlertHandler.RedirectTypeList[1])
                    {
                        string urlString = absURL.Split(AlertHandler.RedirectTypeList[1])[1];
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                            BrowserViewController viewController =
                                storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                            if (viewController != null)
                            {
                                viewController.URL = urlString;
                                viewController.IsDelegateNeeded = false;
                                UINavigationController navController = new UINavigationController(viewController);
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                topVc.PresentViewController(navController, true, null);
                            }
                        }
                    }
                    else
                    {
                        string urlString = absURL.Split(AlertHandler.RedirectTypeList[2])[1];
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                    }
                }
            }
        }
        #endregion
        private void OnSelectAccount(int index)
        {
            if (_imgFilter != null)
            {
                _imgFilter.Image = UIImage.FromBundle("IC-Action-Nav-Unfiltered");
            }
            if (_accountSelector != null)
            {
                _accountSelector.Title = DataManager.DataManager.SharedInstance.SelectedAccount.accountNickName;
            }
            FilterDisplay(false);
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
               {
                   if (NetworkUtility.isReachable)
                   {
                       SetHeaderLoading(true);
                       if (_viewRefreshContainer != null)
                       {
                           _viewRefreshContainer.RemoveFromSuperview();
                       }
                       _historyTableView.Source = new BillHistorySource(new List<BillPayHistoryModel>(), true)
                       {
                           GetI18NValue = GetI18NValue,
                           OnShowFilter = ShowFilterScreen
                       };

                       //Refresh Start
                       /*InvokeInBackground(async () =>
                        {
                            List<Task> taskList = new List<Task>
                            {
                                GetAccountsCharges(),
                                GetAccountBillPayHistory()
                            };
                            await Task.WhenAll(taskList.ToArray());
                            EvaluateResponse();
                        });*/
                       //Refresh End

                       InvokeInBackground(async () =>
                       {
                           isGetAcctChargesLoading = true;
                           _accountCharges = await GetAccountsCharges();
                           InvokeOnMainThread(() =>
                           {
                               SetHeaderLoading(false);
                               if (_accountCharges != null && _accountCharges.d != null && _accountCharges.d.IsSuccess
                                    && _accountCharges.d.data != null && _accountCharges.d.data.AccountCharges != null
                                    && _accountCharges.d.data.AccountCharges.Count > 0 && _accountCharges.d.data.AccountCharges[0] != null)
                               {
                                   AccountChargesCache.SetData(_accountCharges);
                                   UpdateHeaderData(_accountCharges.d.data.AccountCharges[0]);
                                   isGetAcctChargesLoading = false;
                                   CheckTutorialOverlay();
                               }
                               else
                               {
                                   isGetAcctChargesLoading = true;
                                   isGetAcctBillPayHistoryLoading = true;
                                   _historyTableView.Hidden = true;
                                   DisplayRefresh();
                               }
                           });
                       });
                       InvokeInBackground(async () =>
                       {
                           isGetAcctBillPayHistoryLoading = true;
                           _billHistory = await GetAccountBillPayHistory();
                           InvokeOnMainThread(() =>
                           {
                               if (_billHistory != null && _billHistory.d != null && _billHistory.d.IsSuccess
                                    && _billHistory.d.data != null)
                               {
                                   FilterTypes = GetHistoryFilterTypes(_billHistory.d.data);
                                   FilterIndex = 0;
                                   List<BillPayHistoryModel> historyList = _billHistory.d.data.BillPayHistories;
                                   _historyTableView.Source = new BillHistorySource(historyList, false)
                                   {
                                       OnTableViewScroll = OnTableViewScroll,
                                       GetI18NValue = GetI18NValue,
                                       OnSelectBill = DisplayBillPDF,
                                       OnSelectPayment = DisplayReceipt,
                                       OnShowFilter = ShowFilterScreen
                                   };
                                   _historyTableView.ReloadData();
                                   isGetAcctBillPayHistoryLoading = false;
                                   CheckTutorialOverlay();
                               }
                               else
                               {
                                   isGetAcctChargesLoading = true;
                                   isGetAcctBillPayHistoryLoading = true;
                                   _historyTableView.Hidden = true;
                                   DisplayRefresh();
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

        private void EvaluateResponse()
        {
            InvokeOnMainThread(() =>
            {
                if (_accountCharges != null && _accountCharges.d != null && _accountCharges.d.IsSuccess
                    && _accountCharges.d.data != null && _accountCharges.d.data.AccountCharges != null)
                {
                    SetHeaderLoading(false);
                    AccountChargesCache.SetData(_accountCharges);
                    UpdateHeaderData(_accountCharges.d.data.AccountCharges[0]);

                    isGetAcctChargesLoading = false;
                    isGetAcctBillPayHistoryLoading = false;

                    if (_billHistory != null && _billHistory.d != null && _billHistory.d.IsSuccess
                        && _billHistory.d.data != null && _billHistory.d.data.BillPayHistories != null)
                    {
                        FilterTypes = GetHistoryFilterTypes(_billHistory.d.data);
                        FilterIndex = 0;
                        List<BillPayHistoryModel> historyList = _billHistory.d.data.BillPayHistories;
                        _historyTableView.Source = new BillHistorySource(historyList, false)
                        {
                            OnTableViewScroll = OnTableViewScroll,
                            GetI18NValue = GetI18NValue,
                            OnSelectBill = DisplayBillPDF,
                            OnSelectPayment = DisplayReceipt,
                            OnShowFilter = ShowFilterScreen
                        };
                        _historyTableView.ReloadData();
                        CheckTutorialOverlay();
                    }
                    else
                    {
                        string message = GetErrorI18NValue(Constants.Refresh_BillPayHistory);
                        if (_billHistory != null && _billHistory.d != null)
                        {
                            if (_billHistory.d.IsPlannedDownTime)
                            {
                                message = "Planned";
                            }
                            else if (_billHistory.d.RefreshMessage.IsValid())
                            {
                                message = _billHistory.d.RefreshMessage;
                            }
                        }

                        bool isPlanned = _billHistory != null && _billHistory.d != null ? _billHistory.d.IsPlannedDownTime : false;

                        FilterTypes = GetHistoryFilterTypes(_billHistory != null && _billHistory.d != null
                            && _billHistory.d.data != null ? _billHistory.d.data : null);
                        FilterIndex = 0;
                        List<BillPayHistoryModel> historyList = _billHistory != null && _billHistory.d != null
                            && _billHistory.d.data != null ? _billHistory.d.data.BillPayHistories : null;
                        _historyTableView.Source = new BillHistorySource(historyList, false)
                        {
                            GetI18NValue = GetI18NValue,
                            OnShowFilter = ShowFilterScreen,
                            IsFailedService = true,
                            FailMessage = message,
                            OnRefresh = () => { Debug.WriteLine("History Refresh"); },
                            IsPlanned = isPlanned
                        };
                        _historyTableView.ReloadData();
                    }
                }
                else if (_billHistory != null && _billHistory.d != null && _billHistory.d.IsSuccess
                    && _billHistory.d.data != null && _billHistory.d.data.BillPayHistories != null)
                {
                    FilterTypes = GetHistoryFilterTypes(_billHistory.d.data);
                    FilterIndex = 0;
                    List<BillPayHistoryModel> historyList = _billHistory.d.data.BillPayHistories;
                    _historyTableView.Source = new BillHistorySource(historyList, false)
                    {
                        OnTableViewScroll = OnTableViewScroll,
                        GetI18NValue = GetI18NValue,
                        OnSelectBill = DisplayBillPDF,
                        OnSelectPayment = DisplayReceipt,
                        OnShowFilter = ShowFilterScreen
                    };
                    _historyTableView.ReloadData();

                    if (_accountCharges != null && _accountCharges.d != null && _accountCharges.d.IsPlannedDownTime)
                    {
                        //Todo: Display Top as Planned
                        Debug.WriteLine("Display Bot as Planned");
                    }
                    else
                    {
                        //Todo: Display Top as Refresh
                        Debug.WriteLine("Display Bot as Refresh");
                    }
                    //}
                }
                else
                {
                    if (_accountCharges.d.IsPlannedDownTime && _billHistory.d.IsPlannedDownTime)
                    {
                        //Todo: Display Planned
                        Debug.WriteLine("Display Full Planned");
                    }
                    else
                    {
                        //Todo: Display Refresh
                        Debug.WriteLine("Display Full Refresh");
                    }
                }

            });
        }

        private void UpdateHeaderData(AccountChargesModel data)
        {
            bool isRe = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount;
            if (isRe)
            {
                data.AmountDue *= -1;
            }
            _lblAmount.Text = Math.Abs(data.AmountDue).ToString("N2", CultureInfo.InvariantCulture);
            CGRect ctaFrame = _viewCTA.Frame;

            _bgImageView.Image = isRe ? UIImage.FromBundle(BillConstants.IMG_RE)
                : UIImage.FromBundle(data.AmountDue > 0 ? BillConstants.IMG_NeedToPay : BillConstants.IMG_Cleared);

            if (data.AmountDue > 0)
            {
                _lblPaymentStatus.Text = GetI18NValue(isRe ? BillConstants.I18N_MyEarnings : BillConstants.I18N_NeedToPay);
                string result = DateTime.ParseExact(data.DueDate, BillConstants.Format_DateParse
                    , CultureInfo.InvariantCulture).ToString(BillConstants.Format_Date, DateHelper.DateCultureInfo);
                _lblDate.Text = string.Format(BillConstants.Format_Default
                    , GetI18NValue(isRe ? BillConstants.I18N_GetBy : BillConstants.I18N_By), result);
                _lblDate.Hidden = false;
                ctaFrame.Y = GetYLocationFromFrame(_lblDate.Frame, 24);
            }
            else
            {
                _lblPaymentStatus.Text = isRe ? GetI18NValue(data.AmountDue == 0 ? BillConstants.I18N_MyEarnings : BillConstants.I18N_BeenPaidExtra)
                    : GetI18NValue(data.AmountDue == 0 ? BillConstants.I18N_ClearedBills : BillConstants.I18N_PaidExtra);
                _lblDate.Hidden = true;
                ctaFrame.Y = GetYLocationFromFrame(_viewAmount.Frame, 24);
            }
            UpdateViewAmount(data.AmountDue < 0);

            _viewCTA.Frame = ctaFrame;
            _viewCTA.Hidden = isRe;

            nfloat headerHeight = isRe ? _lblDate.Hidden ? _viewAmount.Frame.GetMaxY() : _lblDate.Frame.GetMaxY() : _viewCTA.Frame.GetMaxY();
            CGRect frame = _headerView.Frame;
            frame.Height = headerHeight + GetScaledHeight(isRe ? 24 : 16);
            _headerView.Frame = frame;

            _headerViewContainer.Frame = new CGRect(_headerViewContainer.Frame.Location
                , new CGSize(_headerViewContainer.Frame.Width, _headerView.Frame.GetMaxY()));
            _historyTableView.ReloadData();
            OnResetBGRect();
            isGetAcctChargesLoading = false;
        }

        #region Table
        private void AddTableView()
        {
            nfloat height = View.Frame.Height - _navbarView.Frame.Height - TabBarController.TabBar.Frame.Height;
            _historyTableView = new UITableView(new CGRect(0, _navbarView.Frame.GetMaxY(), ViewWidth, height));
            _historyTableView.RegisterClassForCellReuse(typeof(BillHistoryViewCell), BillConstants.Cell_BillHistory);
            _historyTableView.RegisterClassForCellReuse(typeof(BillSectionViewCell), BillConstants.Cell_BillSection);
            _historyTableView.RegisterClassForCellReuse(typeof(NoDataViewCell), Constants.Cell_NoHistoryData);
            _historyTableView.RegisterClassForCellReuse(typeof(BillHistoryShimmerViewCell), BillConstants.Cell_BillHistoryShimmer);
            _historyTableView.Source = new BillHistorySource(new List<BillPayHistoryModel>(), true)
            {
                OnTableViewScroll = OnTableViewScroll,
                GetI18NValue = GetI18NValue,
                OnShowFilter = ShowFilterScreen
            };
            _historyTableView.BackgroundColor = UIColor.Clear;
            _historyTableView.RowHeight = UITableView.AutomaticDimension;
            _historyTableView.EstimatedRowHeight = GetScaledHeight(90);
            _historyTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            _historyTableView.Bounces = false;
            _historyTableView.TableHeaderView = _headerViewContainer;
            View.AddSubview(_historyTableView);
        }
        #endregion

        #region Events
        private void OnTableViewScroll(object sender, EventArgs e)
        {
            UIScrollView scrollView = sender as UIScrollView;
            CGRect frame = _bgImageView.Frame;
            if ((nfloat)Math.Abs(frame.Y) == frame.Height) { return; }
            nfloat newYLoc = 0 - scrollView.ContentOffset.Y;
            FilterDisplay(scrollView.ContentOffset.Y > _headerViewContainer.Frame.Height + ScaleUtility.GetScaledHeight(36));
            frame.Y = newYLoc;
            _bgImageView.Frame = frame;
            _previousScrollOffset = _historyTableView.ContentOffset.Y;
            nfloat opac = _previousScrollOffset / _tableViewOffset;
            float absOpacity = Math.Abs((float)opac);
            AddViewWithOpacity(absOpacity);
        }

        private void OnResetBGRect()
        {
            _bgImageView.Frame = new CGRect(new CGPoint(0, 0), _bgImageView.Frame.Size);
            AddViewWithOpacity(0);
        }

        private void FilterDisplay(bool isHeader)
        {
            _lblNavTitle.Text = GetI18NValue(isHeader ? BillConstants.I18N_MyHistory : BillConstants.I18N_NavTitle);
            _viewFilter.Hidden = !isHeader;
        }

        private void AddViewWithOpacity(float opacity)
        {
            UIColor startColor = MyTNBColor.GradientPurpleDarkElement;
            UIColor endColor = MyTNBColor.GradientPurpleLightElement;
            CAGradientLayer gradientLayer = new CAGradientLayer
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

        private void DisplayBillPDF(string DetailedInfoNumber)
        {
            if (string.IsNullOrEmpty(DetailedInfoNumber) || string.IsNullOrWhiteSpace(DetailedInfoNumber))
            {
                return;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        NeedsUpdate = false;
                        DataManager.DataManager.SharedInstance.IsSameAccount = true;
                        UIStoryboard storyBoard = UIStoryboard.FromName("ViewBill", null);
                        ViewBillViewController viewController =
                            storyBoard.InstantiateViewController("ViewBillViewController") as ViewBillViewController;
                        if (viewController != null)
                        {
                            viewController.BillingNumber = DetailedInfoNumber;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void DisplayReceipt(string DetailedInfoNumber)
        {
            if (string.IsNullOrEmpty(DetailedInfoNumber) || string.IsNullOrWhiteSpace(DetailedInfoNumber))
            {
                return;
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        NeedsUpdate = false;
                        UIStoryboard storyBoard = UIStoryboard.FromName("Receipt", null);
                        ReceiptViewController viewController =
                            storyBoard.InstantiateViewController("ReceiptViewController") as ReceiptViewController;
                        if (viewController != null)
                        {
                            viewController.DetailedInfoNumber = DetailedInfoNumber;
                            UINavigationController navController = new UINavigationController(viewController);
                            navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                            PresentViewController(navController, true, null);
                        }
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void ShowFilterScreen()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            BillFilterViewController viewController =
                storyBoard.InstantiateViewController("BillFilterViewController") as BillFilterViewController;
            if (viewController != null)
            {
                NeedsUpdate = false;
                viewController.FilterIndex = FilterIndex;
                viewController.FilterTypes = FilterTypes;
                viewController.ApplyFilter = ApplyFilterWithIndex;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }
        #endregion

        #region Filter
        private List<string> GetHistoryFilterTypes(BillPayHistoriesDataModel billpayHistoryData)
        {
            List<string> filterKeys = new List<string>();
            List<string> filterTypes = new List<string>();

            if (billpayHistoryData != null && billpayHistoryData.BillPayFilterData != null && billpayHistoryData.BillPayFilterData.Count > 0)
            {
                FilterKeys = billpayHistoryData.BillPayFilterData.Select(x => x.Type).ToList();
                filterTypes = billpayHistoryData.BillPayFilterData.Select(x => x.Text).ToList();
            }
            return filterTypes;
        }

        private void ApplyFilterWithIndex(int index)
        {
            FilterIndex = index;
            NeedsUpdate = false;
            List<BillPayHistoryModel> historyList = _billHistory?.d?.data?.BillPayHistories.DeepClone() ?? new List<BillPayHistoryModel>();
            List<BillPayHistoryDataModel> dataToRemove = new List<BillPayHistoryDataModel>();
            List<BillPayHistoryModel> historyToRemove = new List<BillPayHistoryModel>();
            if (historyList.Count > 0)
            {
                if (index > 0)
                {
                    string filterKey = FilterKeys[index];
                    foreach (BillPayHistoryModel obj in historyList)
                    {
                        List<BillPayHistoryDataModel> historyData = obj.BillPayHistoryData;
                        foreach (BillPayHistoryDataModel data in historyData)
                        {
                            if (data.HistoryType != filterKey)
                            {
                                dataToRemove.Add(data);
                            }
                        }
                    }

                    if (dataToRemove.Count > 0)
                    {
                        foreach (BillPayHistoryDataModel objToRemove in dataToRemove)
                        {
                            foreach (BillPayHistoryModel obj in historyList)
                            {
                                List<BillPayHistoryDataModel> historyData = obj.BillPayHistoryData;
                                historyData.Remove(objToRemove);
                            }
                        }
                    }

                    foreach (BillPayHistoryModel obj in historyList)
                    {
                        List<BillPayHistoryDataModel> historyData = obj.BillPayHistoryData;
                        if (historyData.Count == 0)
                        {
                            historyToRemove.Add(obj);
                        }
                    }

                    if (historyToRemove.Count > 0)
                    {
                        foreach (BillPayHistoryModel objToRemove in historyToRemove)
                        {
                            historyList.Remove(objToRemove);
                        }
                    }
                }
            }

            _historyTableView.Source = new BillHistorySource(historyList, false)
            {
                OnTableViewScroll = OnTableViewScroll,
                GetI18NValue = GetI18NValue,
                OnSelectBill = DisplayBillPDF,
                OnSelectPayment = DisplayReceipt,
                OnShowFilter = ShowFilterScreen,
                IsFiltered = index > 0
            };
            _historyTableView.ReloadData();

            _imgFilter.Image = UIImage.FromBundle(index > 0 ? BillConstants.IMG_NavFiltered : BillConstants.IMG_NavUnfiltered);
        }
        #endregion

        #region Services
        private async Task<GetAccountsChargesResponseModel> GetAccountsCharges()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                accounts = new List<string> { DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty },
                isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount
            };
            GetAccountsChargesResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountsChargesResponseModel>(BillConstants.Service_GetAccountsCharges, request);
            _accountCharges = response;
            return response;
        }

        private async Task<GetAccountBillPayHistoryResponseModel> GetAccountBillPayHistory()
        {
            ServiceManager serviceManager = new ServiceManager();
            object request = new
            {
                serviceManager.usrInf,
                contractAccount = DataManager.DataManager.SharedInstance.SelectedAccount.accNum ?? string.Empty,
                isOwnedAccount = DataManager.DataManager.SharedInstance.SelectedAccount.IsOwnedAccount,
                accountType = DataManager.DataManager.SharedInstance.SelectedAccount.IsREAccount ? BillConstants.Param_RE : BillConstants.Param_UTIL
            };
            GetAccountBillPayHistoryResponseModel response = serviceManager.OnExecuteAPIV6<GetAccountBillPayHistoryResponseModel>(BillConstants.Service_GetAccountBillPayHistory, request);
            _billHistory = response;
            return response;
        }
        #endregion
    }
}
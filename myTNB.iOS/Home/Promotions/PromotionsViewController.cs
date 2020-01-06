using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Home.Promotions;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS;
using CoreAnimation;

namespace myTNB
{
    public partial class PromotionsViewController : CustomUIViewController
    {
        public PromotionsViewController(IntPtr handle) : base(handle) { }

        public bool IsDelegateNeeded;

        private UIImageView _imgViewNoPromotions, _bgImageView;
        private bool isPromoDetailScreen;
        private nfloat _navBarHeight;
        private UIView _navbarView;
        private UILabel _lblNavTitle, _lblDetails, _lblRefreshMsg;
        private CAGradientLayer _gradientLayer;
        private CustomUIButtonV2 _btnRefresh;

        public override void ViewDidLoad()
        {
            PageName = PromotionConstants.Pagename_Promotion;
            NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            Debug.WriteLine("PROMOTION DID LOAD");
            NotifCenterUtility.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            SetNavigationBar();
            promotionsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, View.Frame.Height - 49 - (DeviceHelper.IsIphoneXUpResolution() ? 88 : 64));
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> PROMOTIONS LanguageDidChange");
            base.LanguageDidChange(notification);
            if (_lblNavTitle != null)
            {
                _lblNavTitle.Text = GetI18NValue(PromotionConstants.I18N_Title);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Debug.WriteLine("PROMOTION WILL APPEAR");
            RemoveRefresh();
            RemoveNoPromotionView();
            if (!isPromoDetailScreen)
            {
                promotionsTableView.Hidden = true;
                promotionsTableView.Source = new PromotionsDataSource(this, new List<PromotionsModel>());
                promotionsTableView.ReloadData();

                if (DataManager.DataManager.SharedInstance.IsPromotionFirstLoad)
                {
                    NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (NetworkUtility.isReachable)
                            {
                                ActivityIndicator.Show();
                                SitecoreServices.Instance.LoadPromotions().ContinueWith(task =>
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        SetSubViews();
                                        ActivityIndicator.Hide();
                                    });
                                });
                            }
                            else
                            {
                                DisplayRefresh();
                            }
                        });
                    });
                }
                else
                {
                    SetSubViews();
                }
            }
            else
            {
                SetSubViews();
                isPromoDetailScreen = false;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        private void SetNavigationBar()
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

            UIView viewTitleBar = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(4)
                , _navbarView.Frame.Width, GetScaledHeight(24)));
            _lblNavTitle = new UILabel(new CGRect(0, 0, ViewWidth, GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                Text = GetI18NValue(PromotionConstants.I18N_Title),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };

            viewTitleBar.AddSubview(_lblNavTitle);
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
            _gradientLayer.Opacity = 1;
            _navbarView.Layer.InsertSublayer(_gradientLayer, 0);
            View.AddSubview(_navbarView);
        }

        private void DisplayRefresh()
        {
            _gradientLayer.Opacity = 0;
            _bgImageView = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.76875F))
            {
                Image = UIImage.FromBundle(Constants.IMG_BannerRefresh)
            };

            _lblRefreshMsg = new UILabel(new CGRect(GetScaledWidth(32)
                      , GetYLocationFromFrame(_bgImageView.Frame, 16), ViewWidth - GetScaledWidth(64), 1000))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = MyTNBColor.Grey,
                Text = GetCommonI18NValue(Constants.Common_RefreshMessage)
            };
            nfloat lblHeight = _lblRefreshMsg.GetLabelHeight(1000);
            _lblRefreshMsg.Frame = new CGRect(_lblRefreshMsg.Frame.Location, new CGSize(_lblRefreshMsg.Frame.Width, lblHeight));
            View.AddSubviews(new UIView[] { _bgImageView, _lblRefreshMsg });
            View.SendSubviewToBack(_bgImageView);

            _btnRefresh = new CustomUIButtonV2
            {
                Frame = new CGRect(GetScaledWidth(16), GetYLocationFromFrame(_lblRefreshMsg.Frame, GetScaledHeight(16))
                , BaseMarginedWidth, ScaleUtility.GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnRefresh.SetTitle(GetCommonI18NValue(Constants.Common_RefreshNow), UIControlState.Normal);
            _btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewWillAppear(true);
            }));
            View.AddSubview(_btnRefresh);
        }

        private void RemoveRefresh()
        {
            _gradientLayer.Opacity = 1;
            if (_bgImageView != null)
            {
                _bgImageView.RemoveFromSuperview();
            }
            if (_lblRefreshMsg != null)
            {
                _lblRefreshMsg.RemoveFromSuperview();
            }
            if (_btnRefresh != null)
            {
                _btnRefresh.RemoveFromSuperview();
            }
        }

        private void SetSubViews()
        {
            if (_imgViewNoPromotions != null)
            {
                _imgViewNoPromotions.RemoveFromSuperview();
            }
            if (_lblDetails != null)
            {
                _lblDetails.RemoveFromSuperview();
            }
            PromotionsEntity wsManager = new PromotionsEntity();
            List<PromotionsModel> promotionList = new List<PromotionsModel>();
            promotionList = wsManager.GetAllItemsV2();
            if (promotionList != null && promotionList.Count > 0)
            {
                promotionsTableView.ClearsContextBeforeDrawing = true;
                promotionsTableView.Source = new PromotionsDataSource(this, promotionList);
                promotionsTableView.ReloadData();
                promotionsTableView.Hidden = false;
                promotionsTableView.ShowsVerticalScrollIndicator = false;
                promotionsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            }
            else
            {
                promotionsTableView.Hidden = true;
                SetNoPromotionScreen();
            }
        }

        private void SetNoPromotionScreen()
        {
            _imgViewNoPromotions = new UIImageView(new CGRect((ViewWidth - GetScaledWidth(100)) / 2
                , GetScaledHeight(88) + _navbarView.Frame.GetMaxY()
                , GetScaledWidth(100), GetScaledHeight(96)))
            {
                Image = UIImage.FromBundle("IC-Empty-Promotion")
            };

            _lblDetails = new UILabel(new CGRect(GetScaledWidth(16), GetYLocationFromFrame(_imgViewNoPromotions.Frame, 24)
                , ViewWidth - GetScaledWidth(32), GetScaledHeight(40)))
            {
                Text = GetI18NValue(PromotionConstants.I18N_NoPromotions),
                TextColor = MyTNBColor.Grey,
                Font = TNBFont.MuseoSans_14_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Center
            };

            nfloat newTitleHeight = _lblDetails.GetLabelHeight(1000);
            _lblDetails.Frame = new CGRect(_lblDetails.Frame.Location, new CGSize(_lblDetails.Frame.Width, newTitleHeight));

            View.AddSubviews(new UIView[] { _imgViewNoPromotions, _lblDetails });
        }

        private void RemoveNoPromotionView()
        {
            if (_imgViewNoPromotions != null)
            {
                _imgViewNoPromotions.RemoveFromSuperview();
            }
            if (_lblDetails != null)
            {
                _lblDetails.RemoveFromSuperview();
            }
        }

        internal void OnPromotionItemSelect(PromotionsModel promotion)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("PromotionDetails", null);
            PromotionDetailsViewController viewController =
                storyBoard.InstantiateViewController("PromotionDetailsViewController") as PromotionDetailsViewController;
            if (viewController != null)
            {
                viewController.Promotion = promotion;
                viewController.OnDone = OnDone;
                UINavigationController navController = new UINavigationController(viewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        /// <summary>
        /// Handler for on done.
        /// </summary>
        public void OnDone()
        {
            isPromoDetailScreen = true;
        }
    }
}
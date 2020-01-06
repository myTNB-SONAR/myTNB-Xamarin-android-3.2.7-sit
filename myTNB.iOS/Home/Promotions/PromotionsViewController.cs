using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.Promotions;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class PromotionsViewController : CustomUIViewController
    {
        public PromotionsViewController(IntPtr handle) : base(handle) { }

        public bool IsDelegateNeeded;
        private UIImageView _imgViewNoPromotions;
        private UILabel _lblDetails;
        private TitleBarComponent _titleBarComponent;
        private UIView _headerView;
        private bool isPromoDetailScreen;

        public override void ViewDidLoad()
        {
            PageName = PromotionConstants.Pagename_Promotion;
            NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            Debug.WriteLine("PROMOTION DID LOAD");
            NotifCenterUtility.AddObserver((Foundation.NSString)"LanguageDidChange", LanguageDidChange);
            SetNavigationBar();
            promotionsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, View.Frame.Height - 49 - (DeviceHelper.IsIphoneXUpResolution() ? 88 : 64));
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> PROMOTIONS LanguageDidChange");
            base.LanguageDidChange(notification);
            _titleBarComponent?.SetTitle(GetI18NValue(PromotionConstants.I18N_Title));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //NavigationController.SetNavigationBarHidden(true, true);
            Debug.WriteLine("PROMOTION WILL APPEAR");
            if (!isPromoDetailScreen)
            {
                promotionsTableView.Hidden = true;
                promotionsTableView.Source = new PromotionsDataSource(this, new List<PromotionsModelV2>());
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
                                DisplayNoDataAlert();
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

        /// <summary>
        /// Sets the value for null end date.
        /// </summary>
        /// <returns>The value for null end date.</returns>
        /// <param name="promotions">Promotions.</param>
        private List<PromotionsModelV2> SetValueForNullEndDate(List<PromotionsModelV2> promotions)
        {
            List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
            foreach (PromotionsModelV2 promo in promotions)
            {
                if (string.IsNullOrEmpty(promo.PromoEndDate))
                {
                    DateTime nowDate = DateTime.Today.Date;
                    DateTime endDate = nowDate.AddDays(90);
                    promo.PromoEndDate = endDate.ToString("yyyyMMdd");
                }
                promotionList.Add(promo);
            }
            return promotionList;
        }

        private void SetNavigationBar()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            _headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(_headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(GetI18NValue(PromotionConstants.I18N_Title));
            _titleBarComponent.SetPrimaryVisibility(true);
            _headerView.AddSubview(titleBarView);
            View.AddSubview(_headerView);
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
            List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
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
                , GetScaledHeight(88) + _headerView.Frame.GetMaxY()
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

        internal void OnPromotionItemSelect(PromotionsModelV2 promotion)
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
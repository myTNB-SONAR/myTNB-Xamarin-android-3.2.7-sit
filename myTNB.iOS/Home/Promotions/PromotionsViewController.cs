using Foundation;
using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.Promotions;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using Newtonsoft.Json;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;
using System.Diagnostics;

namespace myTNB
{
    public partial class PromotionsViewController : CustomUIViewController
    {
        public PromotionsViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsDelegateNeeded = false;
        UIImageView imgViewNoPromotions;
        UILabel lblDetails;
        TitleBarComponent _titleBarComponent;

        string _imageSize = string.Empty;
        bool isPromoDetailScreen = false;

        public override void ViewDidLoad()
        {
            PageName = PromotionsConstants.Pagename;
            NavigationController.NavigationBarHidden = true;
            base.ViewDidLoad();
            Debug.WriteLine("PROMOTION DID LOAD");
            NotifCenterUtility.AddObserver((Foundation.NSString)"LanguageDidChange", LanguageDidChange);
            SetNavigationBar();
            promotionsTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 88 : 64
                , View.Frame.Width, View.Frame.Height - 49 - (DeviceHelper.IsIphoneXUpResolution() ? 88 : 64));
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> PROMOTIONS LanguageDidChange");
            _titleBarComponent?.SetTitle(GetI18NValue(PromotionsConstants.I18N_NavTitle));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //NavigationController.SetNavigationBarHidden(true, true);
            Debug.WriteLine("PROMOTION WILL APPEAR");
            if (!isPromoDetailScreen)
            {
                _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);
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
                                GetPromotions().ContinueWith(task =>
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
                                AlertHandler.DisplayNoDataAlert(this);
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

        Task GetPromotions()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
                string promotionTS = iService.GetPromotionsTimestampItem();
                PromotionsTimestampResponseModel promotionTimeStamp = JsonConvert.DeserializeObject<PromotionsTimestampResponseModel>(promotionTS);
                if (promotionTimeStamp != null && promotionTimeStamp.Status.Equals("Success")
                    && promotionTimeStamp.Data != null && promotionTimeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(promotionTimeStamp.Data[0].Timestamp)
                    && !string.IsNullOrWhiteSpace(promotionTimeStamp.Data[0].Timestamp))
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey("SiteCorePromotionTimeStamp");
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, "SiteCorePromotionTimeStamp");
                        sharedPreference.Synchronize();
                        isValidTimeStamp = true;
                    }
                    else
                    {
                        if (currentTS.Equals(promotionTimeStamp.Data[0].Timestamp))
                        {
                            isValidTimeStamp = false;
                        }
                        else
                        {
                            sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, "SiteCorePromotionTimeStamp");
                            sharedPreference.Synchronize();
                            isValidTimeStamp = true;
                        }
                    }
                }
                if (isValidTimeStamp)
                {
                    string promotionsItems = iService.GetPromotionsItem();
                    PromotionsV2ResponseModel promotionResponse = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(promotionsItems);
                    if (promotionResponse != null && promotionResponse.Status.Equals("Success")
                        && promotionResponse.Data != null && promotionResponse.Data.Count > 0)
                    {
                        PromotionsEntity wsManager = new PromotionsEntity();
                        PromotionsEntity.DeleteTable();
                        wsManager.CreateTable();
                        wsManager.InsertListOfItemsV2(SetValueForNullEndDate(promotionResponse.Data));
                    }
                }
            });
        }

        /// <summary>
        /// Sets the value for null end date.
        /// </summary>
        /// <returns>The value for null end date.</returns>
        /// <param name="promotions">Promotions.</param>
        private List<PromotionsModelV2> SetValueForNullEndDate(List<PromotionsModelV2> promotions)
        {
            List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
            foreach (var promo in promotions)
            {
                if (string.IsNullOrEmpty(promo.PromoEndDate))
                {
                    var nowDate = DateTime.Today.Date;
                    DateTime endDate = nowDate.AddDays(90);
                    promo.PromoEndDate = endDate.ToString("yyyyMMdd");
                }
                promotionList.Add(promo);
            }
            return promotionList;
        }

        internal void SetNavigationBar()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle(GetI18NValue(PromotionsConstants.I18N_NavTitle));
            _titleBarComponent.SetPrimaryVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        void SetSubViews()
        {
            if (imgViewNoPromotions != null)
            {
                imgViewNoPromotions.RemoveFromSuperview();
            }
            if (lblDetails != null)
            {
                lblDetails.RemoveFromSuperview();
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

        void SetNoPromotionScreen()
        {
            imgViewNoPromotions = new UIImageView(new CGRect(DeviceHelper.GetScaledSizeByWidth(26.6f)
                , DeviceHelper.GetScaledSizeByHeight(32.6f), DeviceHelper.GetScaledSizeByWidth(46.9f)
                , DeviceHelper.GetScaledSizeByHeight(26.4f)))
            {
                Image = UIImage.FromBundle(("IC-Empty-Promotion"))
            };

            lblDetails = new UILabel(new CGRect(44, DeviceHelper.GetScaledSizeByHeight(61.8f)
                , View.Frame.Width - 88, 32))
            {
                Text = "Promotion_NoPromotion".Translate(),
                TextColor = MyTNBColor.SilverChalice,
                Font = MyTNBFont.MuseoSans12,
                Lines = 2,
                TextAlignment = UITextAlignment.Center
            };

            View.AddSubviews(new UIView[] { imgViewNoPromotions, lblDetails });
        }

        internal void OnPromotionItemSelect(PromotionsModelV2 promotion)
        {
            //ActivityIndicator.Show();
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
            //ActivityIndicator.Hide();
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
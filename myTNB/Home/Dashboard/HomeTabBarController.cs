using Foundation;
using System;
using UIKit;
using myTNB.DataManager;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Services;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB
{
    public partial class HomeTabBarController : UITabBarController
    {
        public HomeTabBarController(IntPtr handle) : base(handle)
        {
        }

        string _imageSize = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine("HOME DID LOAD");
            TabBar.Translucent = false;
            TabBar.BackgroundColor = UIColor.White;

            ShouldSelectViewController += ShouldSelectTab;

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, HandleAppDidBecomeActive);

            if (!DataManager.DataManager.SharedInstance.IsPromotionFirstLoad)
            {
                UpdatePromotions();
                DataManager.DataManager.SharedInstance.IsPromotionFirstLoad = true;
            }

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("HOME WILL APPEAR");
            UITabBarItem[] vcs = TabBar.Items;
            vcs[1].Enabled = ServiceCall.HasAccountList();
            UpdatePromotionTabBarIcon();
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            PushNotificationHelper.HandlePushNotification();
        }

        /// <summary>
        /// Handles the app did become active.
        /// </summary>
        /// <param name="notification">Notification.</param>
        internal void HandleAppDidBecomeActive(NSNotification notification)
        {
            PushNotificationHelper.HandlePushNotification();
        }

        public override void ItemSelected(UITabBar tabbar, UITabBarItem item)
        {

        }

        public bool ShouldSelectTab(UITabBarController tabBarController, UIViewController viewController)
        {
#if false
            if (viewController is DashboardNavigationController && DataManager.DataManager.SharedInstance.GetAccountsCount() > 1)
            {
                var storyBoard = UIStoryboard.FromName("Dashboard", null);
                var vc = storyBoard.InstantiateViewController("DashboardHomeViewController") as DashboardHomeViewController;
                var nav = viewController as UINavigationController;
                nav.SetViewControllers(new UIViewController[] { vc }, false);
            }
#endif
            return true;
        }

        /// <summary>
        /// Shows the promotions modal.
        /// </summary>
        private void ShowPromotionsModal()
        {
            PromotionsEntity wsManager = new PromotionsEntity();
            var items = wsManager.GetAllItemsV2();

            if (items?.Count > 0)
            {
                CheckResetPromoShown(items);
                var filtered = items.FindAll(item => ShouldDisplayAppLaunch(item));

                if (filtered?.Count > 0)
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("PromotionDetails", null);
                    var viewController =
                        storyBoard.InstantiateViewController("PromotionsModalViewController") as PromotionsModalViewController;
                    viewController.Promotions = filtered;
                    viewController.OnModalDone = OnPromotionsModalDone;
                    var navController = new UINavigationController(viewController);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                    PresentViewController(navController, true, null);
                }
            }

        }

        /// <summary>
        /// Handler for when the promotions modal is done.
        /// </summary>
        public void OnPromotionsModalDone()
        {
            UpdatePromotionTabBarIcon();
        }

        /// <summary>
        /// Checks and resets as needed promo shown date.
        /// </summary>
        /// <param name="promotions">Promotions.</param>
        private void CheckResetPromoShown(List<PromotionsModelV2> promotions)
        {
            foreach (var promo in promotions)
            {
                var shownDate = GetShownDate(promo);
                if (shownDate != default(DateTime))
                {
                    var endDate = shownDate.AddDays(TNBGlobal.PromoOverlayDisplayIntervalDays);
                    var now = DateTime.Now.Date;
                    if (now >= endDate)
                    {
                        promo.PromoShownDate = string.Empty;
                        var entity = promo.ToEntity();
                        PromotionsEntity.UpdateItem(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if promo should be displayed on app launch.
        /// </summary>
        /// <returns><c>true</c>, if promo should display on app launch, <c>false</c> otherwise.</returns>
        /// <param name="promo">Promo.</param>
        private bool ShouldDisplayAppLaunch(PromotionsModelV2 promo)
        {
            bool res = false;
            if (promo == null)
            {
                return res;
            }

            if (promo.ShowAtAppLaunch && !promo.IsPromoExpired && IsPromoWithinCampaignPeriod(promo)
                && IsPromoOutsideDisplayInterval(promo))
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Checks if the promo outside display interval. Promo should not display within the display interval.
        /// </summary>
        /// <returns><c>true</c>, if promo outside display interval was ised, <c>false</c> otherwise.</returns>
        /// <param name="promo">Promo.</param>
        private bool IsPromoOutsideDisplayInterval(PromotionsModelV2 promo)
        {
            bool res = true;

            var shownDate = GetShownDate(promo);
            if (shownDate != default(DateTime))
            {
                var endDate = shownDate.AddDays(TNBGlobal.PromoOverlayDisplayIntervalDays);
                var now = DateTime.Now.Date;
                if (now >= shownDate && now < endDate)
                {
                    res = false;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the shown date.
        /// </summary>
        /// <returns>The shown date.</returns>
        /// <param name="promo">Promo.</param>
        private DateTime GetShownDate(PromotionsModelV2 promo)
        {
            if (!string.IsNullOrEmpty(promo.ID))
            {
                var entity = PromotionsEntity.GetItem(promo.ID);
                if (entity != null)
                {
                    var shownStr = entity.PromoShownDate;
                    if (!string.IsNullOrEmpty(shownStr))
                    {
                        return DateHelper.GetDateWithoutSeparator(shownStr);

                    }
                }
            }

            return default(DateTime);
        }

        /// <summary>
        /// Checks if the promo within campaign period.
        /// </summary>
        /// <returns><c>true</c>, if promo within campaign period was ised, <c>false</c> otherwise.</returns>
        /// <param name="promo">Promo.</param>
        private bool IsPromoWithinCampaignPeriod(PromotionsModelV2 promo)
        {
            //return true;
            var res = false;
            var now = DateTime.Today.Date;
            var startDate = DateHelper.GetDateWithoutSeparator(promo.PromoStartDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(promo.PromoEndDate))
            {
                endDate = DateHelper.GetDateWithoutSeparator(promo.PromoEndDate);
            }
            else
            {
                endDate = now.AddDays(90);
            }

            if (endDate != default(DateTime) && now >= startDate && now <= endDate)
            {
                res = true;
            }

            return res;
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

        bool HasUnreadPromotion(List<PromotionsModelV2> promotionList)
        {
            int index = promotionList.FindIndex(x => x.IsRead == false);
            Console.WriteLine("HasUnreadPromotion: " + (index > -1).ToString());
            return index > -1;
        }

        void UpdatePromotions()
        {
            _imageSize = DeviceHelper.GetImageSize((int)View.Frame.Width);

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        GetPromotions().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                UpdatePromotionTabBarIcon();

                                ShowPromotionsModal();
                            });
                        });
                    }
                });
            });
        }


        void UpdatePromotionTabBarIcon()
        {
            PromotionsEntity wsManager = new PromotionsEntity();
            List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
            promotionList = wsManager.GetAllItemsV2();
            if (promotionList != null && promotionList.Count > 0 && HasUnreadPromotion(promotionList))
            {
                TabBar.Items[2].SelectedImage = UIImage.FromBundle("Tab-Promotions-Unread-Active");
                TabBar.Items[2].Image = UIImage.FromBundle("Tab-Promotions-Unread-Inactive");
            }
            else
            {
                TabBar.Items[2].Image = UIImage.FromBundle("Tab-Promotions");
                TabBar.Items[2].SelectedImage = UIImage.FromBundle("Tab-Promotions");
            }
        }

        Task GetPromotions()
        {
            return Task.Factory.StartNew(() =>
            {

                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.DEFAULT_LANGUAGE);
                bool isValidTimeStamp = false;
#if true
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
#endif

                if (isValidTimeStamp)
                {
                    string promotionsItems = iService.GetPromotionsItem();
                    Console.WriteLine("debug: promo items: " + promotionsItems);
#if true
                    PromotionsV2ResponseModel promotionResponse = JsonConvert.DeserializeObject<PromotionsV2ResponseModel>(promotionsItems);
#else
                    PromotionsResponseModel promotionResponse = JsonConvert.DeserializeObject<PromotionsResponseModel>(promotionsItems);
#endif
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
    }
}
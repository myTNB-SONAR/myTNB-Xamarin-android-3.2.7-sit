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
using System.Diagnostics;
using myTNB.TabBar;

namespace myTNB
{
    public partial class HomeTabBarController : UITabBarController
    {
        public HomeTabBarController(IntPtr handle) : base(handle) { }

        private string _imageSize = string.Empty;
        private Dictionary<string, string> I18NDictionary;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("Tabbar");
            NotifCenterUtility.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NotifCenterUtility.AddObserver(UIApplication.DidBecomeActiveNotification, HandleAppDidBecomeActive);
            TabBar.Translucent = false;
            TabBar.BackgroundColor = UIColor.White;
            SetTabbarTitle();
            ShouldSelectViewController += ShouldSelectTab;
            if (!DataManager.DataManager.SharedInstance.IsPromotionFirstLoad)
            {
                UpdatePromotions();
                DataManager.DataManager.SharedInstance.IsPromotionFirstLoad = true;
            }
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> HOME TAB BAR LanguageDidChange");
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("Tabbar");
            SetTabbarTitle();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UITabBarItem[] tabbarItem = TabBar.Items;
            tabbarItem[1].Enabled = ServiceCall.HasAccountList();
            UpdatePromotionTabBarIcon();
            DataManager.DataManager.SharedInstance.IsPreloginFeedback = false;
            PushNotificationHelper.HandlePushNotification();
        }

        /// <summary>
        /// Handles the app did become active.
        /// </summary>
        /// <param name="notification">Notification.</param>
        private void HandleAppDidBecomeActive(NSNotification notification)
        {
            PushNotificationHelper.HandlePushNotification();
        }

        private void SetTabbarTitle()
        {
            if (TabBar == null || TabBar.Items == null || TabBar.Items.Length < 1)
            {
                return;
            }
            UITextAttributes normalSelected = new UITextAttributes();
            normalSelected.Font = TNBFont.MuseoSans_10_300;
            normalSelected.TextColor = MyTNBColor.GreyishBrown;

            UITextAttributes attrSelected = new UITextAttributes();
            attrSelected.Font = TNBFont.MuseoSans_10_500;
            attrSelected.TextColor = MyTNBColor.WaterBlue;

            UITabBarItem[] tabbarItem = TabBar.Items;
            tabbarItem[0].Title = GetI18NValue(TabbarConstants.Tab_Home);
            tabbarItem[1].Title = GetI18NValue(TabbarConstants.Tab_Bill);
            tabbarItem[2].Title = GetI18NValue(TabbarConstants.Tab_Promotion);
            tabbarItem[3].Title = GetI18NValue(TabbarConstants.Tab_Rewards);
            tabbarItem[4].Title = GetI18NValue(TabbarConstants.Tab_Profile);

            tabbarItem[0].SetTitleTextAttributes(normalSelected, UIControlState.Normal);
            tabbarItem[1].SetTitleTextAttributes(normalSelected, UIControlState.Normal);
            tabbarItem[2].SetTitleTextAttributes(normalSelected, UIControlState.Normal);
            tabbarItem[3].SetTitleTextAttributes(normalSelected, UIControlState.Normal);
            tabbarItem[4].SetTitleTextAttributes(normalSelected, UIControlState.Normal);

            tabbarItem[0].SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            tabbarItem[1].SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            tabbarItem[2].SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            tabbarItem[3].SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            tabbarItem[4].SetTitleTextAttributes(attrSelected, UIControlState.Selected);

            tabbarItem[0].Tag = 0;
            tabbarItem[1].Tag = 1;
            tabbarItem[2].Tag = 2;
            tabbarItem[3].Tag = 3;
            tabbarItem[4].Tag = 4;

            tabbarItem[3].Image = UIImage.FromBundle(ImageString(TabEnum.REWARDS, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            tabbarItem[3].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.REWARDS, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

            foreach (UITabBarItem item in tabbarItem)
            {
                UIImage imgUnselected = item.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                item.Image = imgUnselected;
                UIImage imgSelected = item.SelectedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                item.SelectedImage = imgSelected;
            }
        }

        private string GetI18NValue(string key)
        {
            if (I18NDictionary != null && I18NDictionary.ContainsKey(key))
            {
                return I18NDictionary[key];
            }
            return string.Empty;
        }

        public override void ItemSelected(UITabBar tabbar, UITabBarItem item)
        {
            if (!ShowNewIndicator("2"))
            {
                TabBar.Items[2].Image = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                TabBar.Items[2].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            }
            if (!ShowNewIndicator("3"))
            {
                TabBar.Items[3].Image = UIImage.FromBundle(ImageString(TabEnum.REWARDS, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                TabBar.Items[3].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.REWARDS, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            }

            if (tabbar.SelectedItem.Tag == 1)
            {
                UINavigationController navigationController = ChildViewControllers[1] as UINavigationController;
                BillViewController viewController = navigationController.ViewControllers[0] as BillViewController;
                viewController.NeedsUpdate = true;
            }
            else if (tabbar.SelectedItem.Tag == 2)
            {
                SetNewIndicator("2");
            }
            else if (tabbar.SelectedItem.Tag == 3)
            {
                SetNewIndicator("3");
            }
        }

        public bool ShouldSelectTab(UITabBarController tabBarController, UIViewController viewController)
        {
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
                    UIStoryboard storyBoard = UIStoryboard.FromName(TabbarConstants.Storyboard_Promotion, null);
                    var viewController =
                        storyBoard.InstantiateViewController(TabbarConstants.Controller_Promotion) as PromotionsModalViewController;
                    if (viewController != null)
                    {
                        viewController.Promotions = filtered;
                        viewController.OnModalDone = OnPromotionsModalDone;
                        UINavigationController navController = new UINavigationController(viewController);
                        navController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                        PresentViewController(navController, true, null);
                    }
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
            bool res = false || (promo != null && promo.ShowAtAppLaunch && !promo.IsPromoExpired && IsPromoWithinCampaignPeriod(promo)
                && IsPromoOutsideDisplayInterval(promo));
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
        public static List<PromotionsModelV2> SetValueForNullEndDate(List<PromotionsModelV2> promotions)
        {
            List<PromotionsModelV2> promotionList = new List<PromotionsModelV2>();
            foreach (var promo in promotions)
            {
                if (string.IsNullOrEmpty(promo.PromoEndDate))
                {
                    var nowDate = DateTime.Today.Date;
                    DateTime endDate = nowDate.AddDays(90);
                    promo.PromoEndDate = endDate.ToString(TabbarConstants.Format_Date);
                }
                promotionList.Add(promo);
            }
            return promotionList;
        }

        private bool HasUnreadPromotion(List<PromotionsModelV2> promotionList)
        {
            int index = promotionList.FindIndex(x => x.IsRead == false);
            return index > -1;
        }

        private void UpdatePromotions()
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

        private void UpdatePromotionTabBarIcon()
        {
            TabBar.Items[2].Image = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            TabBar.Items[2].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        }

        private Task GetPromotions()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS, _imageSize, TNBGlobal.SITECORE_URL, TNBGlobal.APP_LANGUAGE);
                bool isValidTimeStamp = false;
                string promotionTS = iService.GetPromotionsTimestampItem();
                PromotionsTimestampResponseModel promotionTimeStamp = JsonConvert.DeserializeObject<PromotionsTimestampResponseModel>(promotionTS);
                if (promotionTimeStamp != null && promotionTimeStamp.Status.Equals("Success")
                    && promotionTimeStamp.Data != null && promotionTimeStamp.Data[0] != null
                    && !string.IsNullOrEmpty(promotionTimeStamp.Data[0].Timestamp)
                    && !string.IsNullOrWhiteSpace(promotionTimeStamp.Data[0].Timestamp))
                {
                    var sharedPreference = NSUserDefaults.StandardUserDefaults;
                    string currentTS = sharedPreference.StringForKey(TabbarConstants.Sitecore_Timestamp);
                    if (string.IsNullOrEmpty(currentTS) || string.IsNullOrWhiteSpace(currentTS))
                    {
                        sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, TabbarConstants.Sitecore_Timestamp);
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
                            sharedPreference.SetString(promotionTimeStamp.Data[0].Timestamp, TabbarConstants.Sitecore_Timestamp);
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

        private string ImageString(TabEnum tabEnum, bool isSelected)
        {
            string imageStr;
            switch (tabEnum)
            {
                case TabEnum.WHATSNEW:

                    if (ShowNewIndicator("2"))
                    {
                        imageStr = isSelected ? TabbarConstants.Img_PromotionsSelected : TabbarConstants.Img_Promotions;
                        imageStr += "-New-" + TNBGlobal.APP_LANGUAGE.ToUpper();
                    }
                    else
                    {
                        PromotionsEntity wsManager = new PromotionsEntity();
                        List<PromotionsModelV2> promotionList = wsManager.GetAllItemsV2();
                        imageStr = promotionList != null && promotionList.Count > 0 && HasUnreadPromotion(promotionList) ?
                            isSelected ? TabbarConstants.Img_ActivePromotionsUnread : TabbarConstants.Img_InactivePromotionsUnread
                            : isSelected ? TabbarConstants.Img_PromotionsSelected : TabbarConstants.Img_Promotions;
                    }
                    break;
                case TabEnum.REWARDS:
                    if (ShowNewIndicator("3"))
                    {
                        imageStr = isSelected ? TabbarConstants.Img_RewardsSelected : TabbarConstants.Img_Rewards;
                        imageStr += "-New-" + TNBGlobal.APP_LANGUAGE.ToUpper();
                    }
                    else
                    {
                        imageStr = isSelected ? TabbarConstants.Img_RewardsSelected : TabbarConstants.Img_Rewards;
                    }
                    break;
                default:
                    imageStr = string.Empty;
                    break;
            }

            return imageStr;
        }

        private void SetNewIndicator(string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, "Tab Id - " + key);
            }
        }

        private bool ShowNewIndicator(string key)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                res = sharedPreference.BoolForKey("Tab Id - " + key);
            }
            return !res;
        }

        private enum TabEnum
        {
            None = 0,
            HOME,
            BILLS,
            WHATSNEW,
            REWARDS,
            PROFILE
        }
    }
}

using Foundation;
using System;
using UIKit;
using myTNB.DataManager;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.TabBar;
using System.Linq;
using myTNB.SitecoreCMS;

namespace myTNB
{
    public partial class HomeTabBarController : UITabBarController
    {
        public HomeTabBarController(IntPtr handle) : base(handle) { }

        private Dictionary<string, string> I18NDictionary;
        private UIColor _badgeColor = MyTNBColor.WaterBlue;
        private UIStringAttributes _badgeAttributes = new UIStringAttributes { Font = MyTNBFont.MuseoSans10_500 };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("Tabbar");
            NotifCenterUtility.AddObserver((NSString)"LanguageDidChange", LanguageDidChange);
            NotifCenterUtility.AddObserver((NSString)"WhatsNewFetchUpdate", WhatsNewFetchUpdate);
            NotifCenterUtility.AddObserver((NSString)"RewardsFetchUpdate", RewardsFetchUpdate);
            NotifCenterUtility.AddObserver(UIApplication.DidBecomeActiveNotification, HandleAppDidBecomeActive);
            TabBar.Translucent = false;
            TabBar.BackgroundColor = UIColor.White;
            ShouldSelectViewController += ShouldSelectTab;

            if (AppLaunchMasterCache.IsRewardsDisabled)
            {
                List<UIViewController> vcs = ViewControllers.ToList();
                vcs.RemoveAt(3);
                ViewControllers = vcs.ToArray();
            }
            else
            {
                FetchRewards();
            }
            SetTabbarTitle();
            FetchWhatsNew();
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> LanguageDidChange: HOME TAB BAR");
            I18NDictionary = LanguageManager.Instance.GetValuesByPage("Tabbar");
            SetTabbarTitle();
            if (!AppLaunchMasterCache.IsRewardsDisabled) { FetchRewards(); }
            FetchWhatsNew(true);
        }

        private void WhatsNewFetchUpdate(NSNotification notification)
        {
            if (!ShowNewIndicator("2"))
            {
                UpdateWhatsNewTabBarIcon();
            }
        }

        private void RewardsFetchUpdate(NSNotification notification)
        {
            if (!AppLaunchMasterCache.IsRewardsDisabled && !ShowNewIndicator("3"))
            {
                UpdateRewardsTabBarIcon();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UITabBarItem[] tabbarItem = TabBar.Items;
            tabbarItem[1].Enabled = ServiceCall.HasAccountList();
            UpdateWhatsNewTabBarIcon();
            UpdateRewardsTabBarIcon();
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

            UITabBarItem[] tabbarItems = TabBar.Items;
            tabbarItems[0].Title = GetI18NValue(TabbarConstants.Tab_Home);
            tabbarItems[1].Title = GetI18NValue(TabbarConstants.Tab_Bill);
            tabbarItems[2].Title = GetI18NValue(TabbarConstants.Tab_Promotion);
            tabbarItems[3].Title = GetI18NValue(AppLaunchMasterCache.IsRewardsDisabled ? TabbarConstants.Tab_Profile : TabbarConstants.Tab_Rewards);

            UpdateTabBar(tabbarItems);

            if (!AppLaunchMasterCache.IsRewardsDisabled)
            {
                tabbarItems[3].Image = UIImage.FromBundle(ImageString(TabEnum.REWARDS, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                tabbarItems[3].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.REWARDS, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                tabbarItems[4].Title = GetI18NValue(TabbarConstants.Tab_Profile);
            }
        }

        private void UpdateTabBar(UITabBarItem[] tabbarItems)
        {
            if (tabbarItems == null || tabbarItems.Length < 1)
            {
                return;
            }
            UITextAttributes normalSelected = new UITextAttributes();
            normalSelected.Font = TNBFont.MuseoSans_10_300;
            normalSelected.TextColor = MyTNBColor.GreyishBrown;

            UITextAttributes attrSelected = new UITextAttributes();
            attrSelected.Font = TNBFont.MuseoSans_10_500;
            attrSelected.TextColor = MyTNBColor.WaterBlue;

            for (int i = 0; i < tabbarItems.Length; i++)
            {
                UITabBarItem tab = tabbarItems[i];
                tab.Tag = i;
                tab.SetTitleTextAttributes(normalSelected, UIControlState.Normal);
                tab.SetTitleTextAttributes(attrSelected, UIControlState.Selected);

                UIImage imgUnselected = tab.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                tab.Image = imgUnselected;
                UIImage imgSelected = tab.SelectedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                tab.SelectedImage = imgSelected;
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
            int tabbarcount = tabbar != null && tabbar.Items != null ? tabbar.Items.Count() : 0;
            if (tabbarcount > 0 && tabbar.SelectedItem.Tag == tabbarcount - 1)
            {
                return;
            }

            InvokeOnMainThread(() =>
            {
                if (!ShowNewIndicator("2"))
                {
                    UpdateWhatsNewTabBarIcon();
                }
                if (!AppLaunchMasterCache.IsRewardsDisabled && !ShowNewIndicator("3"))
                {
                    UpdateRewardsTabBarIcon();
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
                else if (!AppLaunchMasterCache.IsRewardsDisabled && tabbar.SelectedItem.Tag == 3)
                {
                    SetNewIndicator("3");
                }

                if (tabbar.Tag == 0)
                {
                    this.SelectedIndex = 0;
                }
            });
        }

        public bool ShouldSelectTab(UITabBarController tabBarController, UIViewController viewController)
        {
            return true;
        }

        /// <summary>
        /// Handler for when the promotions modal is done.
        /// </summary>
        public void OnPromotionsModalDone()
        {
            UpdateWhatsNewTabBarIcon();
        }

        /// <summary>
        /// Checks and resets as needed promo shown date.
        /// </summary>
        /// <param name="promotions">Promotions.</param>
        private void CheckResetPromoShown(List<PromotionsModel> promotions)
        {
            foreach (PromotionsModel promo in promotions)
            {
                DateTime shownDate = GetShownDate(promo);
                if (shownDate != default(DateTime))
                {
                    DateTime endDate = shownDate.AddDays(TNBGlobal.PromoOverlayDisplayIntervalDays);
                    DateTime now = DateTime.Now.Date;
                    if (now >= endDate)
                    {
                        promo.PromoShownDate = string.Empty;
                        PromotionsEntity entity = promo.ToEntity();
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
        private bool ShouldDisplayAppLaunch(PromotionsModel promo)
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
        private bool IsPromoOutsideDisplayInterval(PromotionsModel promo)
        {
            bool res = true;

            DateTime shownDate = GetShownDate(promo);
            if (shownDate != default(DateTime))
            {
                DateTime endDate = shownDate.AddDays(TNBGlobal.PromoOverlayDisplayIntervalDays);
                DateTime now = DateTime.Now.Date;
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
        private DateTime GetShownDate(PromotionsModel promo)
        {
            if (!string.IsNullOrEmpty(promo.ID))
            {
                PromotionsEntity entity = PromotionsEntity.GetItem(promo.ID);
                if (entity != null)
                {
                    string shownStr = entity.PromoShownDate;
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
        private bool IsPromoWithinCampaignPeriod(PromotionsModel promo)
        {
            bool res = false;
            DateTime now = DateTime.Today.Date;
            DateTime startDate = DateHelper.GetDateWithoutSeparator(promo.PromoStartDate);
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
        public static List<PromotionsModel> SetValueForNullEndDate(List<PromotionsModel> promotions)
        {
            List<PromotionsModel> promotionList = new List<PromotionsModel>();
            foreach (PromotionsModel promo in promotions)
            {
                if (string.IsNullOrEmpty(promo.PromoEndDate))
                {
                    DateTime nowDate = DateTime.Today.Date;
                    DateTime endDate = nowDate.AddDays(90);
                    promo.PromoEndDate = endDate.ToString(TabbarConstants.Format_Date);
                }
                promotionList.Add(promo);
            }
            return promotionList;
        }

        private bool HasUnreadPromotion(List<PromotionsModel> promotionList)
        {
            int index = promotionList.FindIndex(x => x.IsRead == false);
            return index > -1;
        }

        private void FetchWhatsNew(bool isForceUpdate = false)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    if (DataManager.DataManager.SharedInstance.IsFromRewardsDeeplink)
                    {
                        InvokeOnMainThread(() =>
                        {
                            ActivityIndicator.Show();
                        });
                    }
                    InvokeInBackground(async () =>
                    {
                        DataManager.DataManager.SharedInstance.IsWhatsNewLoading = true;
                        await SitecoreServices.Instance.LoadWhatsNew(isForceUpdate);
                        NotifCenterUtility.PostNotificationName("OnReceiveWhatsNewNotification", new NSObject());
                        InvokeOnMainThread(() =>
                        {
                            if (DataManager.DataManager.SharedInstance.IsFromRewardsDeeplink)
                            {
                                ActivityIndicator.Hide();
                            }
                            UpdateWhatsNewTabBarIcon();
                            DataManager.DataManager.SharedInstance.IsWhatsNewLoading = false;
                            CheckForWhatsNewDeepLink();
                        });
                    });
                }
            });
        }

        private void UpdateWhatsNewTabBarIcon()
        {
            TabBar.Items[2].Image = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            TabBar.Items[2].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.WHATSNEW, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

            WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
            List<WhatsNewModel> whatsNewList = whatsNewEntity.GetAllItems();
            if (!ShowNewIndicator("2") && whatsNewList != null && whatsNewList.Count > 0)
            {
                int unreadCount = whatsNewList.Where(x => !x.IsRead && !x.Donot_Show_In_WhatsNew).Count();
                TabBar.Items[2].BadgeColor = _badgeColor;
                TabBar.Items[2].BadgeValue = unreadCount > 0 ? unreadCount.ToString() : null;
                TabBar.Items[2].SetBadgeTextAttributes(_badgeAttributes, UIControlState.Normal);
            }
            else
            {
                TabBar.Items[2].BadgeValue = null;
            }
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
                        imageStr = isSelected ? TabbarConstants.Img_PromotionsSelected : TabbarConstants.Img_Promotions;
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
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, "Tab Id - " + key);
            }
        }

        private bool ShowNewIndicator(string key)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
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

        private void FetchRewards()
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeInBackground(async () =>
                    {
                        DataManager.DataManager.SharedInstance.IsRewardsLoading = true;
                        await SitecoreServices.Instance.LoadRewards();
                        GetUserRewardsResponseModel userRewardsResponse = await RewardsServices.GetUserRewards();
                        NotifCenterUtility.PostNotificationName("OnReceivedRewardsNotification", new NSObject());
                        InvokeOnMainThread(() =>
                        {
                            UpdateRewardsTabBarIcon();
                            DataManager.DataManager.SharedInstance.IsRewardsLoading = false;
                            CheckForRewardDeepLink();
                        });
                    });
                }
            });
        }

        private void UpdateRewardsTabBarIcon()
        {
            if (AppLaunchMasterCache.IsRewardsDisabled)
            {
                return;
            }
            TabBar.Items[3].Image = UIImage.FromBundle(ImageString(TabEnum.REWARDS, false)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            TabBar.Items[3].SelectedImage = UIImage.FromBundle(ImageString(TabEnum.REWARDS, true)).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

            RewardsEntity rewardsEntity = new RewardsEntity();
            List<RewardsModel> rewardsList = rewardsEntity.GetAllItems();

            if (!ShowNewIndicator("3") && rewardsList != null && rewardsList.Count > 0)
            {
                int unreadCount = rewardsList.Where(x => !x.IsRead).Count();
                TabBar.Items[3].BadgeColor = _badgeColor;
                TabBar.Items[3].BadgeValue = unreadCount > 0 ? unreadCount.ToString() : null;
                TabBar.Items[3].SetBadgeTextAttributes(_badgeAttributes, UIControlState.Normal);
            }
            else
            {
                TabBar.Items[3].BadgeValue = null;
            }
        }

        private void CheckForRewardDeepLink()
        {
            if (!DataManager.DataManager.SharedInstance.IsRewardsLoading)
            {
                if (DataManager.DataManager.SharedInstance.IsFromRewardsDeeplink)
                {
                    if (RewardsCache.RewardIsAvailable)
                    {
                        RewardsServices.OpenRewardDetails(RewardsCache.DeeplinkRewardId, this);
                    }
                    else
                    {
                        AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_RewardsUnavailableTitle),
                            LanguageUtility.GetErrorI18NValue(Constants.Error_RewardsUnavailableMsg),
                            new Dictionary<string, Action> {
                        {LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), null}});
                    }
                    DataManager.DataManager.SharedInstance.IsFromRewardsDeeplink = false;
                }
            }
        }

        private void CheckForWhatsNewDeepLink()
        {
            if (!DataManager.DataManager.SharedInstance.IsWhatsNewLoading)
            {
                if (DataManager.DataManager.SharedInstance.IsFromWhatsNewDeeplink)
                {
                    if (WhatsNewCache.WhatsNewIsAvailable)
                    {
                        WhatsNewServices.OpenWhatsNewDetails(WhatsNewCache.DeeplinkWhatsNewId, this);
                    }
                    else
                    {
                        AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewUnavailableTitle),
                            LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewUnavailableMsg),
                            new Dictionary<string, Action> {
                        {LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), null}});
                    }
                    DataManager.DataManager.SharedInstance.IsFromWhatsNewDeeplink = false;
                }
            }
        }
    }
}
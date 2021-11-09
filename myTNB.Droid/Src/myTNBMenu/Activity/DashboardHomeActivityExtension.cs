using Android.Content;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Enquiry.GSL.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    internal static class DashboardHomeActivityExtension
    {
        internal static void DeeplinkValidation(this DashboardHomeActivity mainActivity)
        {
            switch (DeeplinkUtil.Instance.TargetScreen)
            {
                case Deeplink.ScreenEnum.Rewards:
                    DeeplinkRewardValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.WhatsNew:
                    DeeplinkWhatsNewValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.ApplicationListing:
                    DeeplinkAppListingValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.ApplicationDetails:
                    DeeplinkAppDetailsValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.OvervoltageClaimDetails:
                    DeeplinkOvervoltageFeedbackValidation(mainActivity);
                    break;
                default:
                    break;
            }
        }

        private static void DeeplinkRewardValidation(this DashboardHomeActivity mainActivity)
        {
            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
            if (!IsRewardsDisabled && mainActivity.bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
            {
                string rewardID = DeeplinkUtil.Instance.ScreenKey;
                if (!string.IsNullOrEmpty(rewardID))
                {
                    rewardID = "{" + rewardID + "}";
                    RewardsEntity wtManager = new RewardsEntity();
                    RewardsEntity item = wtManager.GetItem(rewardID);
                    mainActivity.mPresenter.OnStartRewardThread();
                }
            }
            else
            {
                mainActivity.IsRootTutorialShown = true;
                MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.RWDS_UNAVAILABLE_TITLE))
                    .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.RWDS_UNAVAILABLE_MSG))
                    .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.GOT_IT))
                    .SetCTAaction(() =>
                    {
                        mainActivity.IsRootTutorialShown = false;
                        if (DashboardHomeActivity.currentFragment.GetType() == typeof(HomeMenuFragment))
                        {
                            HomeMenuFragment fragment = (HomeMenuFragment)mainActivity.SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                            fragment.CallOnCheckShowHomeTutorial();
                        }
                    })
                    .Build().Show();
            }
        }

        private static void DeeplinkWhatsNewValidation(DashboardHomeActivity mainActivity)
        {
            string whatsNewID = DeeplinkUtil.Instance.ScreenKey;
            if (!string.IsNullOrEmpty(whatsNewID))
            {
                whatsNewID = "{" + whatsNewID + "}";
                WhatsNewEntity wtManager = new WhatsNewEntity();
                WhatsNewEntity item = wtManager.GetItem(whatsNewID);
                mainActivity.mPresenter.OnStartWhatsNewThread();
            }
        }

        private static async void DeeplinkAppListingValidation(DashboardHomeActivity mainActivity)
        {
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                mainActivity.ShowProgressDialog();
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
                mainActivity.HideProgressDialog();
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                AllApplicationsCache.Instance.Clear();
                AllApplicationsCache.Instance.Reset();
                Intent applicationLandingIntent = new Intent(mainActivity, typeof(ApplicationStatusLandingActivity));
                mainActivity.StartActivity(applicationLandingIntent);
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                     .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                     .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                errorPopup.Show();
            }
        }

        private static async void DeeplinkAppDetailsValidation(DashboardHomeActivity mainActivity)
        {
            mainActivity.ShowProgressDialog();
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                ApplicationDetailDisplay detailsResponse = await ApplicationStatusManager.Instance.GetApplicationDetail(ApplicationDetailsDeeplinkCache.Instance.SaveID
                    , ApplicationDetailsDeeplinkCache.Instance.ID
                    , ApplicationDetailsDeeplinkCache.Instance.Type
                    , ApplicationDetailsDeeplinkCache.Instance.System);

                if (detailsResponse.StatusDetail.IsSuccess)
                {
                    Intent applicationStatusDetailIntent = new Intent(mainActivity, typeof(ApplicationStatusDetailActivity));
                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(detailsResponse.Content));
                    mainActivity.StartActivity(applicationStatusDetailIntent);
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(detailsResponse.StatusDetail.Title)
                     .SetMessage(detailsResponse.StatusDetail.Message)
                     .SetCTALabel(detailsResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                    errorPopup.Show();
                }
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                     .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                     .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                errorPopup.Show();
            }
            mainActivity.HideProgressDialog();
        }

        private static void DeeplinkOvervoltageFeedbackValidation(DashboardHomeActivity mainActivity)
        {
            UserEntity user = UserEntity.GetActive();
            if (user.UserID == EnquiryDetailsDeeplinkCache.Instance.UserID)
            {
                //Overvoltage detail page
                ShowOverVoltageFeedback(mainActivity);
            }
        }

        internal static void ShowAddAccount(this DashboardHomeActivity mainActivity)
        {
            Intent linkAccount = new Intent(mainActivity, typeof(LinkAccountActivity));
            linkAccount.PutExtra("fromDashboard", true);
            mainActivity.StartActivity(linkAccount);
        }

        internal static void ShowGSLInfoScreen(this DashboardHomeActivity mainActivity)
        {
            Intent gslInfoIntent = new Intent(mainActivity, typeof(GSLRebateInfoActivity));
            mainActivity.StartActivity(gslInfoIntent);
        }

        internal static void ShowOverVoltageFeedback(this DashboardHomeActivity mainActivity)
        {
            Intent viewReceipt = new Intent(mainActivity, typeof(OverVoltageFeedbackDetailActivity));
            viewReceipt.PutExtra("ClaimId", EnquiryDetailsDeeplinkCache.Instance.ClaimID);
            mainActivity.StartActivity(viewReceipt);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP
{
    public class ApplicationStatusLandingPresenter : ApplicationStatusLandingContract.IPresenter
    {
        ApplicationStatusLandingContract.IView mView;
        private RewardServiceImpl mApi;
        private ISharedPreferences mPref;

        public ApplicationStatusLandingPresenter(ApplicationStatusLandingContract.IView view, ISharedPreferences pref   )
        {
            mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }
        public ApplicationStatusLandingPresenter(ApplicationStatusLandingContract.IView view)
        {
            mView = view;
            this.mApi = new RewardServiceImpl();
        }

        public void UpdateWhatsNewRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            WhatsNewEntity wtManager = new WhatsNewEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);
        }

        public void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateReadItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
        }

        private async Task OnUpdateReward(string itemID)
        {
            try
            {
                // Update api calling
                RewardsEntity wtManager = new RewardsEntity();
                RewardsEntity currentItem = wtManager.GetItem(itemID);

                UserInterface currentUsrInf = new UserInterface()
                {
                    eid = UserEntity.GetActive().Email,
                    sspuid = UserEntity.GetActive().UserID,
                    did = UserEntity.GetActive().DeviceId,
                    ft = FirebaseTokenEntity.GetLatest().FBToken,
                    lang = LanguageUtil.GetAppLanguage().ToUpper(),
                    sec_auth_k1 = Constants.APP_CONFIG.API_KEY_ID,
                    sec_auth_k2 = "",
                    ses_param1 = "",
                    ses_param2 = ""
                };

                string rewardId = currentItem.ID;
                rewardId = rewardId.Replace("{", "");
                rewardId = rewardId.Replace("}", "");

                AddUpdateRewardModel currentReward = new AddUpdateRewardModel()
                {
                    Email = UserEntity.GetActive().Email,
                    RewardId = rewardId,
                    Read = currentItem.Read,
                    ReadDate = !string.IsNullOrEmpty(currentItem.ReadDateTime) ? currentItem.ReadDateTime + " +00:00" : "",
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = !string.IsNullOrEmpty(currentItem.IsSavedDateTime) ? currentItem.IsSavedDateTime + " +00:00" : "",
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = !string.IsNullOrEmpty(currentItem.IsUsedDateTime) ? currentItem.IsUsedDateTime + " +00:00" : ""
                };

                AddUpdateRewardRequest request = new AddUpdateRewardRequest()
                {
                    usrInf = currentUsrInf,
                    reward = currentReward
                };

                AddUpdateRewardResponse response = await this.mApi.AddUpdateReward(request, new System.Threading.CancellationTokenSource().Token);

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
             List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingTitle"), // "Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingDescription"),//"Your submitted applications will automatically appear here so you can view their status. Use the filter to search through the list easily."
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchDescription"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = true
            });

            return newList;
            
        }
        public List<NewAppModel> OnGeneraNewAppTutorialEmptyList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.BottomLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingEmptyTitle"), //"Keep track of your applications.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingEmptyDescription"), //"Your submitted applications will automatically appear so you can view their status. Search and save applications submitted by others.",
                ItemCount = 0,
                DisplayMode = "Extra",
                IsButtonShow = false
            });
            newList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchTitle"), //"Search other application status.",
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "applicationStatusLandingSearchDescription"), // "Search and save applications submitted by others with your preferred reference number.",
                ItemCount = 0,
                DisplayMode = "",
                IsButtonShow = false
            });

            return newList;

        }
        
    }
}

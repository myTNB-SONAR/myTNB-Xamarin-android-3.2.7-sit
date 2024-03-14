using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.Content;
using Android.Icu.Text;
using Java.Util;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemPresenter : RewardItemContract.IRewardItemPresenter
    {
        RewardItemContract.IRewardItemView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        private RewardServiceImpl mApi;

        public RewardItemPresenter(RewardItemContract.IRewardItemView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }


        public List<RewardsModel> InitializeRewardList()
        {
            List<RewardsModel> list = new List<RewardsModel>();

            list.Add(new RewardsModel()
            {
                TitleOnListing = "",
                Image = ""
            });

            list.Add(new RewardsModel()
            {
                TitleOnListing = "",
                Image = ""
            });

            list.Add(new RewardsModel()
            {
                TitleOnListing = "",
                Image = ""
            });

            return list;
        }

        public List<RewardsModel> GetActiveRewardList()
        {
            List<RewardsModel> list = new List<RewardsModel>();

            if (mRewardsEntity == null)
            {
                mRewardsEntity = new RewardsEntity();
            }

            List<RewardsEntity> mList = mRewardsEntity.GetActiveItems();

            if (mList != null && mList.Count > 0)
            {
                foreach (RewardsEntity obj in mList)
                {
                    RewardsModel item = new RewardsModel();
                    item.ID = obj.ID;
                    item.Title = obj.Title;
                    item.Image = obj.Image;
                    item.ImageB64 = obj.ImageB64;
                    item.CategoryID = obj.CategoryID;
                    item.Description = obj.Description;
                    item.Read = obj.Read;
                    item.IsUsed = obj.IsUsed;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.PeriodLabel = obj.PeriodLabel;
                    item.LocationLabel = obj.LocationLabel;
                    item.TandCLabel = obj.TandCLabel;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.IsSaved = obj.IsSaved;
                    list.Add(item);
                }
            }

            return list;
        }

        public List<RewardsModel> GetActiveRewardList(string categoryID)
        {
            List<RewardsModel> list = new List<RewardsModel>();

            if (mRewardsEntity == null)
            {
                mRewardsEntity = new RewardsEntity();
            }

            List<RewardsEntity> mList = mRewardsEntity.GetActiveItemsByCategory(categoryID);

            if (mList != null && mList.Count > 0)
            {
                foreach (RewardsEntity obj in mList)
                {
                    RewardsModel item = new RewardsModel();
                    item.ID = obj.ID;
                    item.Title = obj.Title;
                    item.Image = obj.Image;
                    item.ImageB64 = obj.ImageB64;
                    item.CategoryID = obj.CategoryID;
                    item.Description = obj.Description;
                    item.Read = obj.Read;
                    item.IsUsed = obj.IsUsed;
                    item.TitleOnListing = obj.TitleOnListing;
                    item.PeriodLabel = obj.PeriodLabel;
                    item.LocationLabel = obj.LocationLabel;
                    item.TandCLabel = obj.TandCLabel;
                    item.StartDate = obj.StartDate;
                    item.EndDate = obj.EndDate;
                    item.IsSaved = obj.IsSaved;
                    list.Add(item);
                }
            }

            return list;
        }

        public void UpdateRewardSave(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            string formattedDate = currentDate.ToString(@"M/d/yyyy h:m:s tt", currCult);
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateIsSavedItem(itemID, flag, formattedDate);

            _ = OnUpdateReward(itemID);
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
    }
}

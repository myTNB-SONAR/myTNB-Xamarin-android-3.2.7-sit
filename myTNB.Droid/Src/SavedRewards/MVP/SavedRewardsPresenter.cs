using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SavedRewards.MVP
{
    public class SavedRewardsPresenter : SavedRewardsContract.ISavedRewardsPresenter
    {
        SavedRewardsContract.ISavedRewardsView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        private RewardServiceImpl mApi;

        public SavedRewardsPresenter(SavedRewardsContract.ISavedRewardsView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
            this.mApi = new RewardServiceImpl();
        }

        public List<RewardsModel> GetActiveSavedRewardList()
        {
            List<RewardsModel> list = new List<RewardsModel>();

            if (mRewardsEntity == null)
            {
                mRewardsEntity = new RewardsEntity();
            }

            List<RewardsEntity> mList = mRewardsEntity.GetActiveSavedItems();

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


        public void UpdateRewardRead(string itemID, bool flag)
        {
            DateTime currentDate = DateTime.UtcNow;
            RewardsEntity wtManager = new RewardsEntity();
            string formattedDate = currentDate.ToString();
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
                    lang = Constants.DEFAULT_LANG.ToUpper(),
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
                    ReadDate = currentItem.ReadDateTime,
                    Favourite = currentItem.IsSaved,
                    FavUpdatedDate = currentItem.IsSavedDateTime,
                    Redeemed = currentItem.IsUsed,
                    RedeemedDate = currentItem.IsUsedDateTime
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

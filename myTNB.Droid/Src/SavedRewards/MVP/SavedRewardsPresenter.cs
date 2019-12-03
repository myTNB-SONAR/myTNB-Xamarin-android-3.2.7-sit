using System.Collections.Generic;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.SavedRewards.MVP
{
    public class SavedRewardsPresenter : SavedRewardsContract.ISavedRewardsPresenter
    {
        SavedRewardsContract.ISavedRewardsView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        public SavedRewardsPresenter(SavedRewardsContract.ISavedRewardsView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
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

    }
}

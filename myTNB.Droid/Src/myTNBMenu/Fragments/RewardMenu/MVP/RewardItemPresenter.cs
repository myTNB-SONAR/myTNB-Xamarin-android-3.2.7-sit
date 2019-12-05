using System;
using System.Collections.Generic;
using Android.Content;
using Android.Icu.Text;
using Java.Util;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemPresenter : RewardItemContract.IRewardItemPresenter
    {
        RewardItemContract.IRewardItemView mView;

        private ISharedPreferences mPref;

        private RewardsEntity mRewardsEntity;

        public RewardItemPresenter(RewardItemContract.IRewardItemView view, ISharedPreferences pref)
        {
            this.mView = view;
            this.mPref = pref;
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
            DateTime currentDate = DateTime.Now.ToUniversalTime();
            RewardsEntity wtManager = new RewardsEntity();
            string formattedDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            if (!flag)
            {
                formattedDate = "";

            }
            wtManager.UpdateIsSavedItem(itemID, flag, formattedDate);
            // Update api calling
        }
    }
}

using System.Collections.Generic;
using Android.Content;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemPresenter : RewardItemContract.IRewardItemPresenter
    {
        RewardItemContract.IRewardItemView mView;

        private ISharedPreferences mPref;

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
                DisplayName = "",
                Image = ""
            });

            list.Add(new RewardsModel()
            {
                DisplayName = "",
                Image = ""
            });

            list.Add(new RewardsModel()
            {
                DisplayName = "",
                Image = ""
            });

            return list;
        }
    }
}

using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemContract
    {
        public interface IRewardItemView
        {

        }

        public interface IRewardItemPresenter
        {
            List<RewardsModel> InitializeRewardList();
        }
    }
}

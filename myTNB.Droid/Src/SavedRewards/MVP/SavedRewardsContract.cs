using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.SavedRewards.MVP
{
    public class SavedRewardsContract
    {
        public interface ISavedRewardsView
        {

        }

        public interface ISavedRewardsPresenter
        {
            List<RewardsModel> GetActiveSavedRewardList();
        }
    }
}

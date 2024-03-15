using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
    public class RewardItemContract
    {
        public interface IRewardItemView
        {

        }

        public interface IRewardItemPresenter
        {
            List<RewardsModel> InitializeRewardList();

            List<RewardsModel> GetActiveRewardList();

            List<RewardsModel> GetActiveRewardList(string categoryID);

            void UpdateRewardSave(string itemID, bool flag);

            void UpdateRewardRead(string itemID, bool flag);
        }
    }
}

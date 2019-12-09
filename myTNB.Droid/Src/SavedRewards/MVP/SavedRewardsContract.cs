﻿using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;

namespace myTNB_Android.Src.SavedRewards.MVP
{
    public class SavedRewardsContract
    {
        public interface ISavedRewardsView
        {
            void SetEmptyView();
        }

        public interface ISavedRewardsPresenter
        {
            List<RewardsModel> GetActiveSavedRewardList();

            void UpdateRewardRead(string itemID, bool flag);
        }
    }
}

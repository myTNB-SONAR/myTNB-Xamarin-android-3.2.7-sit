using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuContract
    {
		public interface IRewardMenuView
		{
            void OnSavedRewardsTimeStamp(string mSavedTimeStamp);

            void CheckRewardsTimeStamp();

            void OnSetResultTabView(List<RewardMenuModel> list);
        }

		public interface IRewardMenuPresenter
		{
			List<RewardMenuModel> InitializeRewardView();

            Task OnGetRewards();

            void OnCancelTask();

            void GetRewardsTimeStamp();

            Task OnGetRewardsTimeStamp();

            void CheckRewardsCache();

            Task OnGetUserRewardList();
        }
	}
}

using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB.Android.Src.NewAppTutorial.MVP;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuContract
    {
		public interface IRewardMenuView
		{
            void OnSavedRewardsTimeStamp(string mSavedTimeStamp);

            void CheckRewardsTimeStamp(string mTimeStemp);

            void OnSetResultTabView(List<RewardMenuModel> list);

            void SetEmptyView();

            bool CheckTabVisibility();

            void SetRefreshView(string buttonText, string messageText);

            void OnGetRewardTimestamp();
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

            List<NewAppModel> OnGeneraNewAppTutorialList();

            Task OnRecheckRewardsStatus();
        }
	}
}

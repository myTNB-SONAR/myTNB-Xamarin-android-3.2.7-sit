using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using myTNB_Android.Src.NewAppTutorial.MVP;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuContract
    {
		public interface IRewardMenuView
		{
            void OnSavedRewardsTimeStamp(string mSavedTimeStamp);

            void CheckRewardsTimeStamp();

            void OnSetResultTabView(List<RewardMenuModel> list);

            void SetEmptyView();

            bool CheckTabVisibility();

            void SetRefreshView(string buttonText, string messageText);
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
        }
	}
}

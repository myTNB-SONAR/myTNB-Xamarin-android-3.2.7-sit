using System.Collections.Generic;
using Android.Content;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using static myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuPresenter : RewardMenuContract.IRewardMenuPresenter
	{
        RewardMenuContract.IRewardMenuView mView;

		private ISharedPreferences mPref;

		public RewardMenuPresenter(RewardMenuContract.IRewardMenuView view, ISharedPreferences pref)
		{
			this.mView = view;
			this.mPref = pref;
		}


		public List<RewardMenuModel> InitializeRewardView()
        {
			List<RewardMenuModel> list = new List<RewardMenuModel>();

			list.Add(new RewardMenuModel()
			{
				TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            list.Add(new RewardMenuModel()
            {
                TabTitle = "",
                Fragment = new RewardItemFragment(),
                FragmentListMode = REWARDSITEMLISTMODE.INITIATE,
                FragmentSearchString = ""
            });

            return list;
		}
	}
}

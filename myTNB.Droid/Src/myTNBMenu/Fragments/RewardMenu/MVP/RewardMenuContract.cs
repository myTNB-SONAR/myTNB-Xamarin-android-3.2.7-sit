using System.Collections.Generic;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.MVP
{
	public class RewardMenuContract
    {
		public interface IRewardMenuView
		{

		}

		public interface IRewardMenuPresenter
		{
			List<RewardMenuModel> InitializeRewardView();
		}
	}
}

using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api
{
	public interface IAddUpdateRewardApi
	{
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v6/mytnbappws.asmx/AddUpdateRewards")]
		Task<AddUpdateRewardResponse> AddUpdateReward([Body] AddUpdateRewardRequest request, CancellationToken cancellationToken);
	}
}
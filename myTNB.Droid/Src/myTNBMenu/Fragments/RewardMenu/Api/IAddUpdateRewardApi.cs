using myTNB.Mobile.Business;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api
{
	public interface IAddUpdateRewardApi
	{
		[Headers("Content-Type:application/json; charset=utf-8")]
		[Post("/v7/mytnbws.asmx/UpdateRewards")]
		Task<AddUpdateRewardResponse> AddUpdateReward([Body] EncryptedRequest encryptedRequest, CancellationToken cancellationToken);
	}
}
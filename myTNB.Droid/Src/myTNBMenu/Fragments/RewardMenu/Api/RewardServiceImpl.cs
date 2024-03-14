using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Api
{
	public class RewardServiceImpl
    {
        IAddUpdateRewardApi addUpdateRewardApi = null;
        IGetUserRewardsApi getUserRewardsApi = null;

        public RewardServiceImpl()
		{
#if DEBUG
            var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            addUpdateRewardApi = RestService.For<IAddUpdateRewardApi>(httpClient);
            getUserRewardsApi = RestService.For<IGetUserRewardsApi>(httpClient);
#else
			addUpdateRewardApi = RestService.For<IAddUpdateRewardApi>(Constants.SERVER_URL.END_POINT);
            getUserRewardsApi = RestService.For<IGetUserRewardsApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<AddUpdateRewardResponse> AddUpdateReward(AddUpdateRewardRequest request, System.Threading.CancellationToken token)
		{
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return addUpdateRewardApi.AddUpdateReward(encryptedRequest, token);
		}

        public Task<GetUserRewardsResponse> GetUserRewards(GetUserRewardsRequest request, System.Threading.CancellationToken token)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return getUserRewardsApi.GetUserRewards(encryptedRequest, token);
        }
    }
}

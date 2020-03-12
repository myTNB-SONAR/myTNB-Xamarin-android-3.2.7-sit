using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api
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
			return addUpdateRewardApi.AddUpdateReward(request, token);
		}

        public Task<GetUserRewardsResponse> GetUserRewards(GetUserRewardsRequest request, System.Threading.CancellationToken token)
        {
            return getUserRewardsApi.GetUserRewards(request, token);
        }
    }
}

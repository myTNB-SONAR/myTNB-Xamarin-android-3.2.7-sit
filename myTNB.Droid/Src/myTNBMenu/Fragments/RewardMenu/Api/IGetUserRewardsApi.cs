using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Api
{
    public interface IGetUserRewardsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetUserRewards")]
        Task<GetUserRewardsResponse> GetUserRewards([Body] GetUserRewardsRequest request, CancellationToken cancellationToken);
    }
}
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Api
{
    public interface IGetIsSmrApplyAllowedApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetIsSmrApplyAllowed")]
        Task<GetIsSmrApplyAllowedResponse> GetIsSmrApplyAllowed([Body] GetIsSmrApplyAllowedRequest request, CancellationToken cancellationToken);
    }
}
using myTNB_Android.Src.FindUs.Request;
using myTNB_Android.Src.FindUs.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetLocationTypseApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetLocationTypes")]
        Task<GetLocationTypesResponse> GetLocationTypes([Body] GetLocationTypesRequest getLocationRequest, CancellationToken token);
    }
}
using myTNB_Android.Src.FindUs.Request;
using myTNB_Android.Src.FindUs.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetLocationsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetLocations")]
        Task<GetLocationsResponse> GetLocationsV5([Body] GetLocationsRequest getLocationRequest, CancellationToken token);
    }
}
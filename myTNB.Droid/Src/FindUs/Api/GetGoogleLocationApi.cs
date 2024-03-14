using myTNB.Android.Src.FindUs.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.FindUs.Api
{
    public interface GetGoogleLocationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/place/nearbysearch/json")]
        Task<GetGoogleLocationsResponse> GetLocationsFromGoogle([AliasAs("key")] string key, [AliasAs("location")] string location, [AliasAs("radius")] string radius, [AliasAs("keyword")] string keyword, [AliasAs("type")] string type, CancellationToken token);
    }
}
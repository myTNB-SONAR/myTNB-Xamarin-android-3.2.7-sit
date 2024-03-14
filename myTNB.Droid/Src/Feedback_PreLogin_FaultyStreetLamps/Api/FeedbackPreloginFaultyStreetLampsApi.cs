using myTNB.Android.Src.Feedback_PreLogin_FaultyStreetLamps.Models;
using Refit;
using System.Threading.Tasks;

namespace myTNB.Android.Src.Feedback_PreLogin_FaultyStreetLamps.Api
{
    public interface IFeedbackPreloginFaultyStreetLampsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/geocode/json")]
        Task<PreLoginFaultyStreetLampsReverseGeocodeResponse> ReverseGeocode([AliasAs("key")] string key, [AliasAs("latlng")] string latlng);
    }
}
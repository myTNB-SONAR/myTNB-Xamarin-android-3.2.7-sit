using myTNB.Android.Src.Feedback_Login_FaultyStreetLamps.Models;
using Refit;
using System.Threading.Tasks;

namespace myTNB.Android.Src.Feedback_Login_FaultyStreetLamps.Api
{
    public interface IFeedbackLoginFaultyStreetLampsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/geocode/json")]
        Task<LoginFaultyStreetLampsReverseGeocodeResponse> ReverseGeocode([AliasAs("key")] string key, [AliasAs("latlng")] string latlng);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using System.Threading.Tasks;
using myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Models;

namespace myTNB_Android.Src.Feedback_PreLogin_FaultyStreetLamps.Api
{
    public interface IFeedbackPreloginFaultyStreetLampsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/geocode/json")]
        Task<PreLoginFaultyStreetLampsReverseGeocodeResponse> ReverseGeocode([AliasAs("key")] string key , [AliasAs("latlng")] string latlng);
    }
}
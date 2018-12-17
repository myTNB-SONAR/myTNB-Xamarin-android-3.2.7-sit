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
using myTNB_Android.Src.FindUs.Response;
using myTNB_Android.Src.FindUs.Request;
using System.Threading;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetGoogleLocationApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/place/nearbysearch/json")]
        Task<GetGoogleLocationsResponse> GetLocationsFromGoogle([AliasAs("key")] string key, [AliasAs("location")] string location, [AliasAs("radius")] string radius, [AliasAs("keyword")] string keyword, [AliasAs("type")] string type, CancellationToken token);
    }
}
﻿using System;
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
using System.Threading;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetGoogleLocationDetailsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/place/details/json")]
        Task<GetGoogleLocationDetailsResponse> GetLocationDetailsFromGoogle([AliasAs("placeid")] string placeid, [AliasAs("key")] string key,  CancellationToken token);
    }
}
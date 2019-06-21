﻿using myTNB_Android.Src.FindUs.Response;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetGoogleLocationDetailsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Get("/maps/api/place/details/json")]
        Task<GetGoogleLocationDetailsResponse> GetLocationDetailsFromGoogle([AliasAs("placeid")] string placeid, [AliasAs("key")] string key, CancellationToken token);
    }
}
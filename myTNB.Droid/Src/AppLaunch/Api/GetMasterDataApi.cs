﻿using myTNB_Android.Src.AppLaunch.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Api
{
    public interface GetMasterDataApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAppLaunchMasterData")]
        Task<MasterDataResponse> GetAppLaunchMasterData([Body] MasterDataRequest getMasterDataRequest, CancellationToken token);
    }
}
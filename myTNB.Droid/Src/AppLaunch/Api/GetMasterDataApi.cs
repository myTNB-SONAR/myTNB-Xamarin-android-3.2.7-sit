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
using myTNB_Android.Src.AppLaunch.Models;
using System.Threading.Tasks;
using Refit;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using myTNB_Android.Src.Login.Requests;
using System.Threading;

namespace myTNB_Android.Src.Base.Api
{
    public interface GetMasterDataApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetAppLaunchMasterData")]
        Task<MasterDataResponse> GetAppLaunchMasterData([Body] MasterDataRequest getMasterDataRequest, CancellationToken token);
    }
}
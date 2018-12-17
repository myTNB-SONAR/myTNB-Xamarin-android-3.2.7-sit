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
using myTNB_Android.Src.LogoutRate.Models;
using System.Threading.Tasks;
using myTNB_Android.Src.LogoutRate.Request;
using System.Threading;

namespace myTNB_Android.Src.LogoutRate.Api
{
    public interface ILogoutApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/LogoutUser")]
        Task<LogoutResponse> LogoutUser([Body] LogoutRequest request, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/LogoutUser_V2")]
        Task<LogoutResponse> LogoutUserV2([Body] LogoutRequestV2 request, CancellationToken cancellationToken);

    }
}
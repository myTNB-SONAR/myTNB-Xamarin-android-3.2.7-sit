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
using myTNB_Android.Src.AppLaunch.Models;
using System.Threading.Tasks;
using System.Threading;
using myTNB_Android.Src.AppLaunch.Requests;

namespace myTNB_Android.Src.AppLaunch.Api
{
    public interface IWeblinksApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetWebLinks")]
        Task<WeblinkResponse> GetWebLinks([Body] WeblinkRequest request, CancellationToken token);

    }
}
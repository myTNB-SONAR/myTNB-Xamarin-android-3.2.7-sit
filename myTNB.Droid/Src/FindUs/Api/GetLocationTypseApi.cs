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
using myTNB_Android.Src.FindUs.Response;
using System.Threading.Tasks;
using myTNB_Android.Src.FindUs.Request;
using System.Threading;

namespace myTNB_Android.Src.FindUs.Api
{
    public interface GetLocationTypseApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetLocationTypes")]
        Task<GetLocationTypesResponse> GetLocationTypes([Body] GetLocationTypesRequest getLocationRequest, CancellationToken token);
    }
}
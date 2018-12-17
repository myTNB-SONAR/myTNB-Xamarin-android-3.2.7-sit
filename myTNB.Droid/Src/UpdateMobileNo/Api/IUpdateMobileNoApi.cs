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
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.UpdateMobileNo.Models;
using myTNB_Android.Src.UpdateMobileNo.Request;

namespace myTNB_Android.Src.UpdateMobileNo.Api
{
    public interface IUpdateMobileNoApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/UpdatePhoneNumber")]
        Task<UpdateMobileResponse> UpdatePhoneNumber([Body] UpdateMobileRequest request, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/UpdatePhoneNumber_v2")]
        Task<UpdateMobileResponse> UpdatePhoneNumberV2([Body] UpdateMobileV2Request request, CancellationToken cancellationToken);
    }
}
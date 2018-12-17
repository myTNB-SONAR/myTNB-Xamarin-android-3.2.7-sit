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
using myTNB_Android.Src.UpdateNickname.Request;
using System.Threading.Tasks;
using myTNB_Android.Src.UpdateNickname.Models;

namespace myTNB_Android.Src.UpdateNickname.Api
{
    public interface IUpdateAccountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/UpdateLinkedAccountNickName")]
        Task<UpdateLinkedAccountNickNameResponse> UpdateLinkedAccountNickName([Body] UpdateLinkedAccountNickNameRequest request, CancellationToken cancellationToken);

    }
}
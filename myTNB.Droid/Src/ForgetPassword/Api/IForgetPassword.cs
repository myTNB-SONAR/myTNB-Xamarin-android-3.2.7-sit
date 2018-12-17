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
using myTNB_Android.Src.ForgetPassword.Models;
using myTNB_Android.Src.ForgetPassword.Requests;
using System.Threading.Tasks;
using System.Threading;

namespace myTNB_Android.Src.ForgetPassword.Api
{
    public interface IForgetPassword
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SendResetPasswordCode")]
        Task<ForgetPasswordResponse> SendResetPasswordCode([Body] ForgetPasswordRequest userRequest, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/ResetPasswordWithToken")]
        Task<ForgetPasswordVerificationResponse> ResetPasswordWithToken([Body] ForgetPasswordVerificationCodeRequest request, CancellationToken cancellationToken);

    }
}
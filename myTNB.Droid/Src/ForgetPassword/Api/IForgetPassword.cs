using myTNB_Android.Src.ForgetPassword.Models;
using myTNB_Android.Src.ForgetPassword.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ForgetPassword.Api
{
    public interface IForgetPassword
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/ResetPasswordWithToken")]
        Task<ForgetPasswordVerificationResponse> ResetPasswordWithToken([Body] ForgetPasswordVerificationCodeRequest request, CancellationToken cancellationToken);

    }
}
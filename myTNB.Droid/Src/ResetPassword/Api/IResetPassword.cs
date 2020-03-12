using myTNB_Android.Src.ResetPassword.Model;
using myTNB_Android.Src.ResetPassword.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ResetPassword.Api
{
    public interface IResetPassword
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/ChangeNewPassword")]
        Task<ResetPasswordResponse> ChangeNewPassword([Body] ResetPasswordRequest request, CancellationToken cancellationToken);
    }
}
using myTNB_Android.Src.UpdatePassword.Models;
using myTNB_Android.Src.UpdatePassword.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.UpdatePassword.Api
{
    public interface IUpdatePasswordApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/ChangeNewPassword")]
        Task<UpdatePasswordResponse> ChangeNewPassword([Body] UpdatePasswordRequest request, CancellationToken cancellationToken);
    }
}
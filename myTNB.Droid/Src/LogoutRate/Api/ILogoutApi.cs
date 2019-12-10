using myTNB_Android.Src.LogoutRate.Models;
using myTNB_Android.Src.LogoutRate.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.LogoutRate.Api
{
    public interface ILogoutApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/LogoutUser")]
        Task<LogoutResponse> LogoutUser([Body] LogoutRequest request, CancellationToken cancellationToken);
    }
}
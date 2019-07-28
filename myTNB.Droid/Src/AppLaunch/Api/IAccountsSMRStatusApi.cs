using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Api
{
    public interface IAccountsSMRStatusApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsSMRStatus")]
        Task<AccountSMRStatusResponse> AccountsSMRStatusApi([Body] AccountsSMRStatusRequest request, CancellationToken token);
    }
}
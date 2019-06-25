using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Base.Api
{
    public interface GetPhoneVerifyStatusApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetPhoneVerifyStatus")]
        Task<PhoneVerifyStatusResponse> GetPhoneVerifyStatus([Body] GetPhoneVerifyStatusRequest getPhoneVerifyStatusRequest, CancellationToken token);
    }
}
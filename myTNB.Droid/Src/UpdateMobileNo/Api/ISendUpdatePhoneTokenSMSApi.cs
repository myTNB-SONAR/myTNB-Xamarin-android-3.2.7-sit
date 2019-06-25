using myTNB_Android.Src.UpdateMobileNo.Models;
using myTNB_Android.Src.UpdateMobileNo.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.UpdateMobileNo.Api
{
    public interface ISendUpdatePhoneTokenSMSApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SendUpdatePhoneTokenSMS")]
        Task<UpdatePhoneTokenSMSResponse> SendUpdatePhoneTokenSMS([Body] SendUpdatePhoneTokenSMSRequest request, CancellationToken cancellationToken);
    }
}
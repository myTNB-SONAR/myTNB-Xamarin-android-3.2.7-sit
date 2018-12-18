using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Model;
using Refit;

namespace myTNB.Mobile.Services
{
    [Headers("Content-Type:application/json; charset=utf-8")]
    public interface IApiService
    {
         [Post("/{urlPrefix}/GetPhoneVerifyStatus")]         Task<PhoneVerificationStatusResponse> GetPhoneVerifyStatus(string urlPrefix, [Body] PhoneVerificationStatusRequest getPhoneVerifyStatusRequest,
            CancellationToken cancelToken); 
    }
}

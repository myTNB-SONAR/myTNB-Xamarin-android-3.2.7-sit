using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Mobile.AWS.Services.Eligibility
{
    internal interface IEligibilityService
    {
        [Post("/Eligibility/api/v1/Eligibility/GetEligibility")]
        Task<HttpResponseMessage> PostEligibility([Body] EncryptedRequest encryptedRequest
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);
    }
}
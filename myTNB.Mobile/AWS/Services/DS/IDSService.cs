using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.AWS.Services.DS
{
    internal interface IDSService
    {
        [Get("/DigitalSignature/api/v1/EKYC/EKYCStatusByUserID/{userID}")]
        Task<HttpResponseMessage> GetEKYCStatus(string userID
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Get("/Identity/api/v1/Identity/UserIdentification/{userID}")]
        Task<HttpResponseMessage> GetEKYCIdentification(string userID
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/Eligibility/api/v1/Eligibility/GetEligibility")]
        Task<HttpResponseMessage> PostEligibilityByCriteria([Body] PostEligibilityByCriteriaRequest request
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);
    }
}
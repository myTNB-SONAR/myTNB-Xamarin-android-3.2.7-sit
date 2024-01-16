using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.MoreIcon;
using Refit;

namespace myTNB.Mobile.AWS.Services.Eligibility
{
    internal interface IEligibilityService
    {
        [Post("/Eligibility/api/v1/Eligibility/GetEligibility")]
        Task<HttpResponseMessage> PostEligibility([Body] PostEligibilityRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Post("/Eligibility/api/v1/Eligibility/GetMoreIconList")]
        Task<HttpResponseMessage> GetMoreIconList([Body] MoreIconRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Post("/Eligibility/api/v1/Eligibility/UpdateMoreIcon")]
        Task<HttpResponseMessage> UpdateMoreIconList([Body] UpdateMoreIconRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);
    }
}
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.AWS.Services.DBR
{
    internal interface IDBRService
    {
        [Get("/{type}/api/v1/eligibility/{userID}")]
        Task<HttpResponseMessage> GetEligibility(string userID
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string type = AWSConstants.Eligibility);

        [Get("/{environment}/api/v1/billrendering/{ca}")]
        Task<HttpResponseMessage> GetBillRendering(string ca
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);
    }
}
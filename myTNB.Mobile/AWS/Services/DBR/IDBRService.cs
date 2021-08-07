using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.DBR;
using Refit;

namespace myTNB.Mobile.AWS.Services.DBR
{
    internal interface IDBRService
    {
        [Get("/{environment}/api/v1/eligibility/{userID}")]
        Task<HttpResponseMessage> GetEligibility(string userID
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Get("/{environment}/api/v1/billrendering/{ca}")]
        Task<HttpResponseMessage> GetBillRendering(string ca
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/{environment}/api/v1/BillRendering/GetMultiple")]
        Task<HttpResponseMessage> PostMultiBillRendering(PostMultiBillRenderingRequest request
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = "KqNPPaCgl913pSLSHBgVT8NjJvTfTdYH6W0R1w78"//AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/{environment}/api/v1/Z_CS_SSP_GET_INSTL")]
        Task<HttpResponseMessage> PostInstallationDetails(PostInstallationDetailsRequest request
          , CancellationToken cancellationToken
          , [Header(AWSConstants.Headers.Authorization)] string accessToken
          , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
          , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = "KqNPPaCgl913pSLSHBgVT8NjJvTfTdYH6W0R1w78"//AWSConstants.XAPIKey
          , string environment = AWSConstants.Environment);

        [Post("/{environment}/api/v1/Z_CS_SSP_GET_INSTL/GetMultiple")]
        Task<HttpResponseMessage> PostMultiInstallationDetails(PostMultiInstallationDetailsRequest request
          , CancellationToken cancellationToken
          , [Header(AWSConstants.Headers.Authorization)] string accessToken
          , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
          , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = "KqNPPaCgl913pSLSHBgVT8NjJvTfTdYH6W0R1w78"//AWSConstants.XAPIKey
          , string environment = AWSConstants.Environment);
    }
}
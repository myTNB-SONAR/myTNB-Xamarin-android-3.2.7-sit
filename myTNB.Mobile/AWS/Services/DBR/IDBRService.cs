using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.DBR;
using Refit;

namespace myTNB.Mobile.AWS.Services.DBR
{
    internal interface IDBRService
    {
        [Get("/eligibility/{userID}")]
        Task<HttpResponseMessage> GetEligibility(string userID
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Get("/billrendering/{ca}")]
        Task<HttpResponseMessage> GetBillRendering(string ca
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/BillRendering/GetMultiple")]
        Task<HttpResponseMessage> PostMultiBillRendering(PostMultiBillRenderingRequest request
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/Z_CS_SSP_GET_INSTL")]
        Task<HttpResponseMessage> PostInstallationDetails(PostInstallationDetailsRequest request
          , CancellationToken cancellationToken
          , [Header(AWSConstants.Headers.Authorization)] string accessToken
          , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
          , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
          , string environment = AWSConstants.Environment);

        [Post("/Z_CS_SSP_GET_INSTL/GetMultiple")]
        Task<HttpResponseMessage> PostMultiInstallationDetails(PostMultiInstallationDetailsRequest request
          , CancellationToken cancellationToken
          , [Header(AWSConstants.Headers.Authorization)] string accessToken
          , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
          , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
          , string environment = AWSConstants.Environment);
    }
}
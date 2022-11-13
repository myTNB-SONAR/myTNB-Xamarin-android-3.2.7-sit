using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.DBR;
using myTNB.Mobile.AWS.Models.DBR.AutoOptIn;
using myTNB.Mobile.AWS.Models.DBR.GetBillRendering;
using Refit;

namespace myTNB.Mobile.AWS.Services.DBR
{
    internal interface IDBRService
    {
        [Post("/Eligibility/api/v1/Eligibility/GetEligibility")]
        Task<HttpResponseMessage> PostEligibility([Body] PostEligibilityRequest request
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
        Task<HttpResponseMessage> PostMultiBillRendering([Body] PostMultiBillRenderingRequest request
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Post("/BillRendering/BREligibilityIndicators")]
        Task<HttpResponseMessage> GetBillRenderingTenant([Body] GetBillRenderingTenantRequest tenantRequest
         , CancellationToken cancellationToken
         , [Header(AWSConstants.Headers.Authorization)] string accessToken
         , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
         , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
         , string environment = AWSConstants.Environment);

        [Post("/AutoOptInSchedule/GetAutoOptInCa")]
        Task<HttpResponseMessage> GetAutoOptInCaDBR([Body] GetAutoOptInCaRequest autoOptInCaRequest
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

        [Patch("/AutoOptInSchedule/UpdateAutoOptInCa")]
        Task<HttpResponseMessage> UpdateAutoOptInCaDBR([Body] GetAutoOptInCaRequest updateAutoOptInCaRequest
           , CancellationToken cancellationToken
           , [Header(AWSConstants.Headers.Authorization)] string accessToken
           , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);

    }
}

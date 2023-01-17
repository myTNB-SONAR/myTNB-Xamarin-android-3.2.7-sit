using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.DBR;
using Refit;

namespace myTNB.Mobile.AWS.Services.DBR
{
    internal interface IDBRService
    {
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
        Task<HttpResponseMessage> PostBREligibilityIndicators([Body] PostBREligibilityIndicatorsRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Post("/AutoOptInSchedule/GetAutoOptInCa")]
        Task<HttpResponseMessage> PostGetAutoOptInCa([Body] PostGetAutoOptInCaRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Patch("/AutoOptInSchedule/UpdateAutoOptInCa")]
        Task<HttpResponseMessage> PatchUpdateAutoOptInCa([Body] PatchUpdateAutoOptInCaRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);
    }
}
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.AWS.Services.ApplicationStatus
{
    public interface IApplicationStatusService
    {
        [Post("/myhome/nc-svc/api/v1/NC/DeleteDraft?referenceNo={referenceNo}")]
        Task<HttpResponseMessage> PostDeleteNCDraft(string referenceNo
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);

        [Post("/myhome/cot-svc/api/v1/COT/CancelDraft?referenceNo={referenceNo}")]
        Task<HttpResponseMessage> PostDeleteCOTDraft(string referenceNo
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);

        [Post("/myhome/coa-svc/api/v1/COA/DeleteDraft?referenceNo={referenceNo}")]
        Task<HttpResponseMessage> PostDeleteCOADraft(string referenceNo
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);

        [Post("/myhome/myhome-svc/api/v1/GetDraftApplication/GetDraftApplicationStatus")]
        Task<HttpResponseMessage> PostGetDraftApplications(CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);
    }
}
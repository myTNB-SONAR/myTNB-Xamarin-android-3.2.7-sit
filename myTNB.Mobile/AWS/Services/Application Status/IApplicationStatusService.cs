using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.ApplicationStatus.PostNCDraftApplications;
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

        [Post("/myhome/nc-svc/api/v1/NC/GetNCDraftApplications")]
        Task<HttpResponseMessage> PostGetNCDraftApplications([Body] PostGetNCDraftRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);
    }
}
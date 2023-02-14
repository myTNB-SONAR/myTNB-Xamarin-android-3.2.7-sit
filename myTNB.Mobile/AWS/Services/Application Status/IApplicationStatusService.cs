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
    }
}
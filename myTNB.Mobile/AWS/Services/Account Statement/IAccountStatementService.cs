using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.AccountStatement;
using Refit;

namespace myTNB.Mobile.AWS.Services.AccountStatement
{
    internal interface IAccountStatementService
    {
        [Post("/Account/api/v1/AccountDetails/AccountStatement")]
        Task<HttpResponseMessage> PostAccountStatement([Body] PostAccountStatementRequest request
            , CancellationToken cancellationToken
            , [Header(AWSConstants.Headers.Authorization)] string accessToken
            , [Header(AWSConstants.Headers.ViewInfo)] string viewInfo
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey);
    }
}
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models;
using Refit;

namespace myTNB.Mobile.AWS.Services.AccessToken
{
    internal interface IAccessTokenService
    {
        [Post("/Identity/GenerateAccessToken")]
        Task<HttpResponseMessage> GenerateAccessToken([Body] AccessTokenRequest request
            , CancellationToken cancelToken
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);

        [Post("/general/user-svc/api/v1/token/GetAccessToken")]
        Task<HttpResponseMessage> GetUserServiceAccessToken([Body] AccessTokenRequest request
            , CancellationToken cancelToken
            , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = "KqNPPaCgl913pSLSHBgVT8NjJvTfTdYH6W0R1w78"//AWSConstants.XAPIKey
            , string environment = AWSConstants.Environment);
    }
}
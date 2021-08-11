using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models;
using Refit;

namespace myTNB.Mobile.AWS.Services.AccessToken
{
    internal interface IAccessTokenService
    {
        [Post("/{environment}/api/v1/Identity/GenerateAccessToken")]
        Task<HttpResponseMessage> GenerateAccessToken([Body] AccessTokenRequest request
           , CancellationToken cancelToken
           , [Header(AWSConstants.Headers.XAPIKey)] string xAPIKey = AWSConstants.XAPIKey
           , string environment = AWSConstants.Environment);
    }
}
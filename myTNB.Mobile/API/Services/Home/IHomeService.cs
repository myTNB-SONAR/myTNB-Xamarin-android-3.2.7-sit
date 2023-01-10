using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.Home.PostServices;
using Refit;

namespace myTNB.Mobile.API.Services.Home
{
    public interface IHomeService
    {
        [Headers(new string[] { "Content-Type: application/json" })]
        [Post("/{urlPrefix}/GetServicesV4")]
        Task<HttpResponseMessage> PostServices([Body] PostServicesRequest request
            , CancellationToken cancellationToken
            , [Header(MobileConstants.Header_UserInfo)] string userInfo
            , [Header(MobileConstants.Header_Lang)] string lang
            , [Header(MobileConstants.Header_SecureKey)] string secureKey = MobileConstants.ApiKeyId
            , string urlPrefix = MobileConstants.ApiUrlPath);
    }
}
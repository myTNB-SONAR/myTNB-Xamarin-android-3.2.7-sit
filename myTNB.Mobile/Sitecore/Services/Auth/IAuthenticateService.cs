using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.Sitecore
{
    [Headers("Content-Type:application/json; charset=utf-8")]
    public interface IAuthenticateService
    {
        // Todo: Remove after test
        [Post("/{urlPrefix}/sitecore/api/ssc/auth/login")]
        Task<HttpResponseMessage> OnLogin([Body] LoginRequest request
            , CancellationToken cancelToken
            , string urlPrefix = MobileConstants.SitecoreURL);
    }
}
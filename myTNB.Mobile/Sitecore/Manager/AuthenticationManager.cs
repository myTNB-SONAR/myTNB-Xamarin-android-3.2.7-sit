using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Refit;

namespace myTNB.Mobile.Sitecore
{
    public sealed class AuthenticationManager
    {
        private static readonly Lazy<AuthenticationManager> lazy =
            new Lazy<AuthenticationManager>(() => new AuthenticationManager());

        public static AuthenticationManager Instance { get { return lazy.Value; } }
        public AuthenticationManager() { }

        #region Login
        public async Task<string> Login()
        {
            try
            {
                var service = RestService.For<IAuthenticateService>(Constants.SitecoreURL);
                var requestParameter = new LoginRequest
                {
                    Domain = Constants.SitecoreDomain,
                    Username = Constants.SitecoreUsername,
                    Password = Constants.SitecorePassword
                };

                try
                {
                    HttpResponseMessage response = await service.OnLogin(requestParameter, NetworkService.GetCancellationToken());
                    HttpResponseHeaders headers = response.Headers;
                    string cookie = string.Empty;
                    if (headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
                    {
                        cookie = values.First();
                    }
                    return cookie;
                }
                catch (ApiException apiEx)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG || MASTER
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG || MASTER
                Debug.WriteLine(e.Message);
#endif
            }
            return string.Empty;
        }
        #endregion
    }
}

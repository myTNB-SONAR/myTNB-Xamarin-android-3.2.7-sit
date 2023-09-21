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
                var service = RestService.For<IAuthenticateService>(MobileConstants.SitecoreURL);
                var requestParameter = new LoginRequest
                {
                    Domain = MobileConstants.SitecoreDomain,
                    Username = MobileConstants.SitecoreUsername,
                    Password = MobileConstants.SitecorePassword
                };

                try
                {
                    string cookie = string.Empty;
                    HttpResponseMessage response = await service.OnLogin(requestParameter, NetworkService.GetCancellationToken());
                    if (response != null && response.Headers != null)
                    {
                        HttpResponseHeaders headers = response.Headers;
                        if (headers != null
                            && headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
                        {
                            cookie = values.First();
                        }
                    }
                    Debug.WriteLine("[DEBUG] SiteCore Login cookie: " + cookie);
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

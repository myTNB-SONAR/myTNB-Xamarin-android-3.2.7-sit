using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Models.AccessToken;
using myTNB.Mobile.AWS.Services.AccessToken;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Mobile
{
    public sealed class AccessTokenManager
    {
        private static readonly Lazy<AccessTokenManager> lazy =
            new Lazy<AccessTokenManager>(() => new AccessTokenManager());

        public static AccessTokenManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public AccessTokenManager() { }

        /// <summary>
        /// Generates access token used to access services and in webview SSO
        /// Channel and Role Id are hard coded as it is used only in mobile App
        /// </summary>
        /// <param name="userID">myTNB account's user Id</param>
        /// <returns>Access Token</returns>
        public async Task<string> GenerateAccessToken(string userID)
        {
            try
            {
                IAccessTokenService service = RestService.For<IAccessTokenService>(AWSConstants.Domains.GenerateAccessToken);
                AccessTokenRequest request = new AccessTokenRequest
                {
                    Channel = AWSConstants.Channel,
                    UserId = userID.IsValid() ? userID.ToUpper() : string.Empty
                };

                HttpResponseMessage rawResponse = await service.GenerateAccessToken(request
                    , NetworkService.GetCancellationToken());
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    return string.Empty;
                }
                AccessTokenResponse response = await rawResponse.ParseAsync<AccessTokenResponse>();
                if (response != null && response.Content != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new AccessTokenResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] GenerateAccessToken: " + JsonConvert.SerializeObject(response));
                if (response.StatusDetail.IsSuccess)
                {
                    return response?.Content?.AccessToken ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GenerateAccessToken]Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GenerateAccessToken]General Exception: " + ex.Message);
#endif
            }
            return string.Empty;
        }
    }
}
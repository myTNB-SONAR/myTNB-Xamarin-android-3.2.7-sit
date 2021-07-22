using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS;
using myTNB.Mobile.AWS.Models;
using myTNB.Mobile.AWS.Services.DBR;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;

namespace myTNB.Mobile
{
    public sealed class EligibilityManager
    {
        private static readonly Lazy<EligibilityManager> lazy =
           new Lazy<EligibilityManager>(() => new EligibilityManager());

        public static EligibilityManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public EligibilityManager() { }

        private struct ConfigConstants
        {
            internal const string ServiceConfiguration = "ServiceConfiguration";
            internal const string CallInterval = "callInterval";
            internal const string Enabled = "enabled";
        }

        /// <summary>
        /// Get Eligible CAs
        /// Gte Eligible Features
        /// </summary>
        /// <param name="userID">myTNB Account User ID</param>
        /// <param name="accessToken">Generated Access Token</param>
        /// <returns></returns>
        public async Task<GetEligibilityResponse> GetEligibility(string userID
            , string accessToken)
        {
            userID = "0D1568D9-7770-4345-84BD-04C2C56A2069";
            accessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJVc2VySW5mbyI6IntcIkNoYW5uZWxcIjpudWxsLFwiVXNlcklkXCI6XCIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDBcIixcIlVzZXJOYW1lXCI6bnVsbCxcIlJvbGVJZHNcIjpudWxsfSIsIm5iZiI6MTYyNTYzMzQ1MywiZXhwIjoxNjI1NjM3MDUzLCJpYXQiOjE2MjU2MzM0NTMsImlzcyI6Im15VE5CIiwiYXVkIjoibXlUTkIgQXVkaWVuY2UifQ.jvPKeYrZqAvL9BOD25UpRFx1ld1-vp4FgUQLW6VFSW8-UlgWIyyY58mA7rtT7TX3m0GVPzJ6anEv6TYehiHGHQ";

            GetEligibilityResponse response = new GetEligibilityResponse();
            try
            {
                IDBRService service = RestService.For<IDBRService>(AWSConstants.Domains.GetDBREligibility);
                HttpResponseMessage rawResponse = await service.GetEligibility(userID
                   , NetworkService.GetCancellationToken()
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<GetEligibilityResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null
                        && response.StatusDetail != null
                        && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new GetEligibilityResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GetEligibility]Refit Exception: " + apiEx.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GetEligibility]General Exception: " + ex.Message);
#endif
            }

            response = new GetEligibilityResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.GetEligibility.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }

        /// <summary>
        /// Returns the API interval of specified service
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <returns>Value in Minutes</returns>
        public int GetAPICallInterval(string serviceName)
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(ConfigConstants.ServiceConfiguration, serviceName);
            if (config != null
                && config[ConfigConstants.CallInterval] is JToken enabledJToken
                && enabledJToken != null)
            {
                return enabledJToken.ToObject<int>();
            }
            return 0;
        }

        /// <summary>
        /// Checks if service is enabled based on language file
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <returns>isEnabled</returns>
        public bool IsEnabled(string serviceName)
        {
            JToken config = LanguageManager.Instance.GetServiceConfig(ConfigConstants.ServiceConfiguration, serviceName);
            if (config != null
                && config[ConfigConstants.Enabled] is JToken enabledJToken
                && enabledJToken != null)
            {
                return enabledJToken.ToObject<bool>();
            }
            return false;
        }

        /// <summary>
        /// Determines if Service should be called based on language config
        /// </summary>
        /// <param name="serviceName">Name of service to query from language file under ServiceConfiguration</param>
        /// <param name="saveTimeStamp">saved time stamp from device</param>
        /// <returns></returns>
        public bool ShouldCallApi(string serviceName
            , string saveTimeStamp)
        {
            if (IsEnabled(serviceName))
            {
                if (!saveTimeStamp.IsValid())
                {
                    return true;
                }
                DateTime timeStamp = DateTime.Parse(saveTimeStamp);
                int interval = GetAPICallInterval(serviceName);
                DateTime now = DateTime.Now;
                TimeSpan timeSpan = now - timeStamp;
                double minutes = timeSpan.TotalMinutes;
                return interval < minutes;
            }
            return false;
        }
    }
}
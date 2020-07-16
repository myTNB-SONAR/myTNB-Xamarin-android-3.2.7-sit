using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using RestSharp;

namespace myTNB
{
    public class BaseService
    {
        const string CONTENT_TYPE = "Content-Type";
        const string APPLICATION_JSON = "application/json";

        const int MaxRetryCount = 2;
        private int Timeout = 60000;
        private int RetryTimeout = 2000;
        private int _retryCount = 1;

        private bool _isPayment;
        private string _paymentURL = string.Empty;

        private readonly Dictionary<string, string> DomainDictionary = new Dictionary<string, string>
        {
            { "DEV", "https://mobiletestingws.tnb.com.my"}//"http://10.215.128.191:89",http://10.215.128.191:88
            , { "SIT", "https://mobiletestingws.tnb.com.my" }
            , { "PROD", "https://mytnbapp.tnb.com.my"}
        };

        private readonly Dictionary<string, string> EndPointDictionaryDev = new Dictionary<string, string>
        {
            {"V4", "/v5/my_billingssp.asmx/"}
            , {"V5", "/v5/my_billingssp.asmx/"}
            , {"V6", "/v6/mytnbappws.asmx/"}
        };

        private readonly Dictionary<string, string> EndPointDictionaryProd = new Dictionary<string, string>
        {
            {"V4", "/v5/my_billingssp.asmx/"}
            , {"V5", "/v5/my_billingssp.asmx/"}
            , {"V6", "/v6/mytnbappws.asmx/"}
        };

        private Dictionary<string, int> TimeOutDictionary = new Dictionary<string, int> {
            { "GetAppLaunchMasterData", 3000},
            { "GetPaymentReceipt", 30000}
        };

        private string GetURLEndpoint(APIVersion version)
        {
            string ver = version.ToString();
            if (TNBGlobal.IsProduction)
            {
                return EndPointDictionaryProd.ContainsKey(ver) ? EndPointDictionaryProd[ver] : string.Empty;
            }
            else
            {
                return EndPointDictionaryDev.ContainsKey(ver) ? EndPointDictionaryDev[ver] : string.Empty;
            }
        }

        /// <summary>
        /// Executes the webservice.
        /// </summary>
        /// <returns>The webservice.</returns>
        /// <param name="suffix">The name of the API.</param>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="version">Version of API to be used.</param>
        public RestResponse ExecuteWebservice(string suffix, object requestParams, APIVersion version, APIEnvironment env, bool isRetry = false)
        {
            //SIT Test
            //env = APIEnvironment.SIT;
            string domain = GetDomain(env);
            string url = domain + GetURLEndpoint(version) + suffix;

            _retryCount += isRetry ? 1 : 0;

            if (!string.IsNullOrEmpty(suffix) && !string.IsNullOrWhiteSpace(suffix)
                && TimeOutDictionary != null && TimeOutDictionary.Count > 0 && TimeOutDictionary.ContainsKey(suffix))
            {
                Timeout = TimeOutDictionary[suffix];
            }

            var client = new RestClient(url)
            {
                Timeout = isRetry ? RetryTimeout : Timeout
            };

            var request = new RestRequest
            {
                Method = Method.POST,
                Timeout = isRetry ? RetryTimeout : Timeout
            };

            request.AddHeader(CONTENT_TYPE, APPLICATION_JSON);
            request.AddJsonBody(requestParams);
            Debug.WriteLine("Service ------> " + suffix);

            RestResponse response = (RestResponse)client.Execute(request);

            WebException responseException = (WebException)response.ErrorException;
            if (responseException != null
                && responseException.Status == WebExceptionStatus.Timeout
                && suffix == "GetAppLaunchMasterData")
            {
                isRetry = true;
                if (isRetry && _retryCount <= MaxRetryCount)
                {
                    response = ExecuteWebservice(suffix, requestParams, version, env, isRetry);
                }
            }
            return response;
        }

        public string GetFormattedURL(string suffix, Dictionary<string, string> requestParams, APIVersion version, APIEnvironment env)
        {
            return GetURL(suffix, requestParams, version, env);
        }

        /// <summary>
        /// Gets the formatted URL for payment.
        /// </summary>
        /// <returns>The formatted URL.</returns>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="isPayment">If set to <c>true</c> is payment.</param>
        /// <param name="paymentURL">Payment URL.</param>
        public string GetFormattedURL(Dictionary<string, string> requestParams, bool isPayment, string paymentURL, APIEnvironment env)
        {
            _isPayment = isPayment;
            _paymentURL = paymentURL;
            return GetURL(string.Empty, requestParams, 0, env);
        }

        internal string GetURL(string suffix, Dictionary<string, string> requestParams, APIVersion version, APIEnvironment env)
        {
            string url = string.Empty;
            if (_isPayment)
            {
                url = _paymentURL + "?";
            }
            else
            {
                string domain = GetDomain(env);
                url = domain + GetURLEndpoint(version) + suffix + "?";
            }

            int index = 0;
            foreach (var item in requestParams)
            {
                string value = item.Value == null ? string.Empty : item.Value.Replace(" ", "%20");
                if (index == 0)
                {
                    url += item.Key + "=" + value;
                }
                else
                {
                    url += "&" + item.Key + "=" + value;
                }
                index++;
            }
            return url;
        }

        public string GetDomain(APIEnvironment environment)
        {
            string env = environment.ToString();
            return DomainDictionary.ContainsKey(env) ? DomainDictionary[env] : string.Empty;
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.Enum;
using RestSharp;

namespace myTNB
{
    public class BaseService
    {
        const string CONTENT_TYPE = "Content-Type";
        const string APPLICATION_JSON = "application/json";
        const int TIMEOUT = 60000;
        readonly int DOMAINTYPE = TNBGlobal.IsProduction ? 1 : 0; // Set to 1 for Prod
        bool _isPayment = false;
        string _paymentURL = string.Empty;
        readonly string[] _domain = new string[]{
            "https://mobiletestingws.tnb.com.my",
            "https://mytnbapp.tnb.com.my"
        };
        readonly string[] _endpointDevURL = new string[]{
            "/v4/my_billingssp.asmx/",
            "/v5/my_billingssp.asmx/"
        };
        readonly string[] _endpointProdURL = new string[]{
            "/v4/my_BillingSSP.asmx/",
            "/v5/my_BillingSSP.asmx/"
        };

        string GetURLEndpoint(APIVersion version)
        {
            if (TNBGlobal.IsProduction)
            {
                return version == APIVersion.V4 ? _endpointProdURL[0] : _endpointProdURL[1];
            }
            else
            {
                return version == APIVersion.V4 ? _endpointDevURL[0] : _endpointDevURL[1];
            }
        }

        /// <summary>
        /// Executes the webservice.
        /// </summary>
        /// <returns>The webservice.</returns>
        /// <param name="suffix">The name of the API.</param>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="version">Version of API to be used.</param>
        public RestResponse ExecuteWebservice(string suffix, object requestParams, APIVersion version)
        {
            string domain = _domain[DOMAINTYPE];
            string url = domain + GetURLEndpoint(version) + suffix;

            var client = new RestClient(url)
            {
                Timeout = TIMEOUT
            };

            var request = new RestRequest
            {
                Method = Method.POST,
                Timeout = TIMEOUT
            };
            request.AddHeader(CONTENT_TYPE, APPLICATION_JSON);
            request.AddJsonBody(requestParams);

            Debug.WriteLine("*****URL: " + url);
            Debug.WriteLine("*****PARAMETERS: " + requestParams);
            RestResponse response = (RestResponse)client.Execute(request);
            Debug.WriteLine("*****RESPONSE: " + response.Content.ToString());
            return response;
        }

        public string GetFormattedURL(string suffix, Dictionary<string, string> requestParams, APIVersion version)
        {
            return GetURL(suffix, requestParams, version);
        }

        /// <summary>
        /// Gets the formatted URL for payment.
        /// </summary>
        /// <returns>The formatted URL.</returns>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="isPayment">If set to <c>true</c> is payment.</param>
        /// <param name="paymentURL">Payment URL.</param>
        public string GetFormattedURL(Dictionary<string, string> requestParams, bool isPayment, string paymentURL)
        {
            _isPayment = isPayment;
            _paymentURL = paymentURL;
            return GetURL(string.Empty, requestParams, 0);
        }

        internal string GetURL(string suffix, Dictionary<string, string> requestParams, APIVersion version)
        {
            string url = string.Empty;
            if (_isPayment)
            {
                url = _paymentURL + "?";
            }
            else
            {
                string domain = _domain[DOMAINTYPE];
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
    }
}
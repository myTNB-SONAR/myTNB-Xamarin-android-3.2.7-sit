using RestSharp;
using Newtonsoft.Json;
using myTNB.Model;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System;

namespace myTNB
{
    public class ServiceManager
    {
        public BaseResponseModel BaseServiceCall(string suffix, object requestParams)
        {
            return OnExecuteAPI<BaseResponseModel>(suffix, requestParams);
        }

        public T OnExecuteAPI<T>(string suffix, object requestParams, APIVersion version = APIVersion.V5, bool isDev = false) where T : new()
        {
            T customClass = new T();
            try
            {
#if DEBUG
                isDev = true;
#endif
                BaseService baseService = new BaseService();
                APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
                env = isDev ? APIEnvironment.DEV : env;
                RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, version, env);
                return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? customClass
                    : JsonConvert.DeserializeObject<T>(rawResponse.Content);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Service Exception: {0} - {1} ", suffix, e.Message);
            }
            return customClass;
        }

        public T OnExecuteAPIV6<T>(string suffix, object requestParams) where T : new()
        {
            return OnExecuteAPI<T>(suffix, requestParams, APIVersion.V6);
        }

        public T OnExecuteAPIV2<T>(string suffix, object requestParams) where T : new()
        {
            BaseService baseService = new BaseService();
            APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5, env);
            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content) && !string.IsNullOrWhiteSpace(rawResponse.Content))
                {
                    JObject jsonData = JObject.Parse(rawResponse.Content);
                    JToken contentData = jsonData["d"];
                    if (contentData != null)
                    {
                        BaseModel responseBase = new BaseModel();
                        T model = contentData.ToObject<T>();
                        if (model != null)
                        {
                            return model;
                        }
                    }
                }
            }
            catch (JsonSerializationException jse)
            {
                Debug.WriteLine("JsonSerializationException: " + jse.Message);
            }
            catch (JsonReaderException jre)
            {
                Debug.WriteLine("JsonReaderException: " + jre.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new T();
        }

        /// <summary>
        /// Gets the PDF URL.
        /// </summary>
        /// <returns>The PDFS ervice URL.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public string GetPDFServiceURL(string suffix, Dictionary<string, string> requestParams)
        {
            BaseService baseService = new BaseService();
            APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
            return baseService.GetFormattedURL(suffix, requestParams, APIVersion.V5, env);
        }

        /// <summary>
        /// Gets the payment URL.
        /// </summary>
        /// <returns>The payment URL.</returns>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="paymentURL">Payment URL.</param>
        public string GetPaymentURL(Dictionary<string, string> requestParams, string paymentURL)
        {
            BaseService baseService = new BaseService();
            APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
            return baseService.GetFormattedURL(requestParams, true, paymentURL, env);
        }
    }
}
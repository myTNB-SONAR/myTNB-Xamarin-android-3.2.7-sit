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
        public BaseResponseModelV2 BaseServiceCallV6(string suffix, object requestParams)
        {
            return OnExecuteAPIV6<BaseResponseModelV2>(suffix, requestParams);
        }

        public T OnExecuteAPI<T>(string suffix, object requestParams, APIVersion version = APIVersion.V5) where T : new()
        {
            T customClass = new T();
            try
            {
                BaseService baseService = new BaseService();
                APIEnvironment env = TNBGlobal.IsProduction ? APIEnvironment.PROD : APIEnvironment.SIT;
#if DEBUG
                env = APIEnvironment.DEV;
#endif
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
#if DEBUG
            env = APIEnvironment.DEV;
#endif
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
#if DEBUG
            env = APIEnvironment.DEV;
#endif
            return baseService.GetFormattedURL(suffix, requestParams, APIVersion.V6, env);
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
#if DEBUG
            env = APIEnvironment.DEV;
#endif
            return baseService.GetFormattedURL(requestParams, true, paymentURL, env);
        }

        public object usrInf
        {
            get
            {
                string email = DataManager.DataManager.SharedInstance.User.Email ?? string.Empty;
                if (string.IsNullOrEmpty(email) && DataManager.DataManager.SharedInstance.UserEntity != null &&
                    DataManager.DataManager.SharedInstance.UserEntity.Count > 0 &&
                    DataManager.DataManager.SharedInstance.UserEntity[0] != null)
                {
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email ?? string.Empty;
                }
                string fcmToken = DataManager.DataManager.SharedInstance.FCMToken != null
                    ? DataManager.DataManager.SharedInstance.FCMToken : string.Empty;
                return new
                {
                    eid = email,
                    sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                    did = DataManager.DataManager.SharedInstance.UDID,
                    ft = fcmToken,
                    lang = TNBGlobal.APP_LANGUAGE,
                    sec_auth_k1 = TNBGlobal.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                };
            }
        }

        public object deviceInf
        {
            get
            {
                return new
                {
                    DeviceId = DataManager.DataManager.SharedInstance.UDID,
                    AppVersion = AppVersionHelper.GetAppShortVersion(),
                    OsType = TNBGlobal.DEVICE_PLATFORM_IOS,
                    OsVersion = DeviceHelper.GetOSVersion(),
                    DeviceDesc = TNBGlobal.APP_LANGUAGE
                };
            }
        }
    }
}
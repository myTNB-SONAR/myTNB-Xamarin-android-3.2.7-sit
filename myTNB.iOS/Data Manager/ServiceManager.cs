using RestSharp;
using Newtonsoft.Json;
using myTNB.Model;
using System.Collections.Generic;
using myTNB.Enum;
using myTNB.Model.AddMultipleSupplyAccountsToUserReg;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System;

namespace myTNB
{
    public class ServiceManager
    {
        /// <summary>
        /// Bases the service call.
        /// </summary>
        /// <returns>The service call.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public BaseResponseModel BaseServiceCall(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new BaseResponseModel()
                    : JsonConvert.DeserializeObject<BaseResponseModel>(rawResponse.Content);
        }

        public T OnExecuteAPI<T>(string suffix, object requestParams) where T : new()
        {
            T customClass = new T();
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                || string.IsNullOrWhiteSpace(rawResponse.Content)) ? customClass
                : JsonConvert.DeserializeObject<T>(rawResponse.Content);
        }

        /// <summary>
        /// Gets the account usage history for graph.
        /// </summary>
        /// <returns>The account usage history for graph.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public ChartModel GetAccountUsageHistoryForGraph(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<ChartModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new ChartModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException)
            { }
            catch (JsonReaderException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new ChartModel();
        }

        /// <summary>
        /// Gets the smart meter account data.
        /// </summary>
        /// <returns>The smart meter account data.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public SmartChartModel GetSmartMeterAccountData(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<SmartChartModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new SmartChartModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (JsonReaderException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new SmartChartModel();
        }

        /// <summary>
        /// Checks if the valid response.
        /// </summary>
        /// <returns><c>true</c>, if valid response was ised, <c>false</c> otherwise.</returns>
        /// <param name="jToken">J token.</param>
        public bool IsValidResponse(JToken jToken, ref BaseModel resp)
        {
            try
            {
                if (jToken != null)
                {
                    var model = jToken.ToObject<BaseModel>();
                    if (model != null)
                    {
                        resp = model;
                        return model.didSucceed;
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("General JSON parsing error: {0}", e.Message));
            }
            return false;
        }

        /// <summary>
        /// Gets the rate us questions.
        /// </summary>
        /// <returns>The rate us questions.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public FeedbackQuestionRequestModel GetRateUsQuestions(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<FeedbackQuestionRequestModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new FeedbackQuestionRequestModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new FeedbackQuestionRequestModel();
        }

        /// <summary>
        /// Gets the linked accounts summary info.
        /// </summary>
        /// <returns>The linked accounts summary info.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public AmountDueStatusResponseModel GetLinkedAccountsSummaryInfo(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<AmountDueStatusResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new AmountDueStatusResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new AmountDueStatusResponseModel();
        }

        /// <summary>
        /// Gets the app launch master data.
        /// </summary>
        /// <returns>The app launch master data.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public MasterDataResponseModel GetAppLaunchMasterData(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<MasterDataResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new MasterDataResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new MasterDataResponseModel();
        }

        /// <summary>
        /// Gets the phone verification status.
        /// </summary>
        /// <returns>The phone verification status.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public PhoneVerificationStatusResponseModel GetPhoneVerificationStatus(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<PhoneVerificationStatusResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new PhoneVerificationStatusResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Error in {0}. Message: {1}", suffix, e.Message));
            }
            return new PhoneVerificationStatusResponseModel();
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
            return baseService.GetFormattedURL(suffix, requestParams, APIVersion.V5);
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
            return baseService.GetFormattedURL(requestParams, true, paymentURL);
        }
    }
}
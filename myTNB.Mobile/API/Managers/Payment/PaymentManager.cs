using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.Payment.GetTaxInvoice;
using myTNB.Mobile.API.Models.Payment.PostApplicationPayment;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.Mobile.API.Services.Payment;
using myTNB.Mobile.Business;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;
using System.Diagnostics;

namespace myTNB.Mobile.API.Managers.Payment
{
    public sealed class PaymentManager
    {
        private static readonly Lazy<PaymentManager> lazy =
           new Lazy<PaymentManager>(() => new PaymentManager());

        public static PaymentManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public PaymentManager() { }

        #region ApplicationPayment
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userInfo"></param>
        /// <param name="customerName"></param>
        /// <param name="phoneNo"></param>
        /// <param name="osType"></param>
        /// <param name="registeredCardId"></param>
        /// <param name="paymentMode"></param>
        /// <param name="totalAmount"></param>
        /// <param name="applicationType"></param>
        /// <param name="searchTerm"></param>
        /// <param name="system"></param>
        /// <param name="applicationPaymentDetail"></param>
        /// <returns></returns>
        public async Task<T> ApplicationPayment<T>(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , string applicationType
            , string searchTerm
            , string system
            , string statusId
            , string statusCode
            , ApplicationPaymentDetail applicationPaymentDetail) where T : new()
        {
            T customClass = new T();
            try
            {
                if (totalAmount.IsValid() && totalAmount.Contains(","))
                {
                    totalAmount = totalAmount.Replace(",", string.Empty);
                }
                IPaymentService service = RestService.For<IPaymentService>(MobileConstants.ApiDomain);
                try
                {
                    PostApplicationPaymentRequest request = new PostApplicationPaymentRequest
                    {
                        UserInfo = userInfo,
                        CustomerName = customerName,
                        PhoneNo = phoneNo,
                        Platform = osType,
                        RegisteredCardId = registeredCardId,
                        PaymentMode = paymentMode,
                        TotalAmount = totalAmount,
                        ApplicationType = applicationType ?? string.Empty,
                        SearchTerm = searchTerm ?? string.Empty,
                        System = system ?? "myTNB",
                        ApplicationPaymentDetail = applicationPaymentDetail,
                        StatusId = statusId,
                        StatusCode = statusCode
                    };
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                    HttpResponseMessage rawResponse = await service.ApplicationPayment(encryptedRequest
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());

                    string response = await rawResponse.Content.ReadAsStringAsync();
                    if (response.IsValid())
                    {
                        customClass = JsonConvert.DeserializeObject<T>(response);
                    }
                    return customClass;
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][ApplicationPayment]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][ApplicationPayment]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            return customClass;
        }
        #endregion

        #region GetApplicationsPaidDetails
        public async Task<PostApplicationsPaidDetailsResponse> GetApplicationsPaidDetails(object userInfo
            , string refNumber
            , string statusId
            , string statusCode
            , string applicationType)
        {
            try
            {
                IPaymentService service = RestService.For<IPaymentService>(MobileConstants.ApiDomain);
                try
                {
                    PostApplicationsPaidDetailsRequest request = new PostApplicationsPaidDetailsRequest
                    {
                        UserInfo = userInfo,
                        ApplicationPayment = new ApplicationPayment
                        {
                            SRNumber = refNumber,
                            StatusId = statusId,
                            StatusCode = statusCode,
                            ApplicationType = applicationType
                        }
                    };
                    EncryptedRequest encryptedRequest = APISecurityManager.Instance.GetEncryptedRequest(request);
                    HttpResponseMessage rawResponse = await service.GetApplicationsPaidDetails(encryptedRequest
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString());
                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    PostApplicationsPaidDetailsResponse response = new PostApplicationsPaidDetailsResponse();
                    if (responseString.IsValid())
                    {
                        response = JsonConvert.DeserializeObject<PostApplicationsPaidDetailsResponse>(responseString);
                    }
                    return response;
                }
                catch (ApiException)
                {
#if MASTER
                    Debug.WriteLine("[DEBUG][GetApplicationsPaidDetails]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetApplicationsPaidDetails]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            return new PostApplicationsPaidDetailsResponse();
        }
        #endregion

        #region GetTaxInvoice
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srNumber">CA Number</param>
        /// <returns></returns>
        public async Task<byte[]> GetTaxInvoice(string srNumber)
        {
            GetTaxInvoiceResponse response;
            try
            {
                IPaymentService service = RestService.For<IPaymentService>(MobileConstants.ApiDomain);
                try
                {
                    HttpResponseMessage rawResponse = await service.GetTaxInvoice(srNumber
                        , AppInfoManager.Instance.GetUserInfo()
                        , NetworkService.GetCancellationToken()
                        , AppInfoManager.Instance.Language.ToString()
                        , AppInfoManager.Instance.Language.ToString());

                    string responseString = await rawResponse.Content.ReadAsStringAsync();
                    byte[] pdfByte = await rawResponse.Content.ReadAsByteArrayAsync();
                    return pdfByte;
                    /*
                    //Mark: Check for 404 First.
                    NotFoundModel notFoundModel = JsonConvert.DeserializeObject<NotFoundModel>(responseString);
                    if (notFoundModel != null && notFoundModel.Status.IsValid())
                    {
                        response = new GetTaxInvoiceResponse
                        {
                            Content = null,
                            StatusDetail = Constants.Service_GetTaxInvoice.GetStatusDetails(Constants.EMPTY)
                        };
                        return response;
                    }

                    response = JsonConvert.DeserializeObject<GetTaxInvoiceResponse>(responseString);
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        //Mark: Check for 0 Applications
                        if (response.Content == null)
                        {
                            response.StatusDetail.Code = Constants.EMPTY;
                        }
                        response.StatusDetail = Constants.Service_GetTaxInvoice.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response.StatusDetail = new StatusDetail();
                        response.StatusDetail = Constants.Service_GetTaxInvoice.GetStatusDetails(Constants.DEFAULT);
                    }
                    return response;*/
                }
                catch (ApiException apiEx)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetTaxInvoice]Refit Exception: " + apiEx.Message);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine("[DEBUG][GetTaxInvoice]General Exception: " + ex.Message);
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e.Message);
#endif
            }
            response = new GetTaxInvoiceResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = MobileConstants.Service_GetTaxInvoice.GetStatusDetails(MobileConstants.DEFAULT);
            return null;
        }
        #endregion

        #region Tax Invoice URL
        public string GetTaxInvoiceURL(string srNumber)
        {
            try
            {
                if (srNumber.IsValid())
                {
                    string urlFormat = "{0}/{1}/{2}?apiKeyID={3}&apiKey={4}&srNumber={5}&lang={6}";
                    string url = string.Format(urlFormat
                        , MobileConstants.ApiDomain
                        , MobileConstants.ApiUrlPath
                        , MobileConstants.Service_TaxInvoice
                        , MobileConstants.ApiKeyId
                        , MobileConstants.APIKey
                        , srNumber
                        , AppInfoManager.Instance.GetLanguage());
                    return url;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG][GetTaxInvoiceURL]General Exception: " + e.Message);
#endif
            }
            return string.Empty;
        }
        #endregion
    }
}
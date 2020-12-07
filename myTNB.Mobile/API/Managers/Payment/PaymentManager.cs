using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.Payment.GetTaxInvoice;
using myTNB.Mobile.API.Models.Payment.PostApplicationPayment;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.Mobile.API.Services.Payment;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;

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
        public async Task<T> ApplicationPayment<T>(object userInfo
            , string customerName
            , string phoneNo
            , string osType
            , string registeredCardId
            , string paymentMode
            , string totalAmount
            , ApplicationPaymentDetail applicationPaymentDetail) where T : new()
        {
            T customClass = new T();
            try
            {
                if (totalAmount.IsValid() && totalAmount.Contains(","))
                {
                    totalAmount = totalAmount.Replace(",", string.Empty);
                }
                IPaymentService service = RestService.For<IPaymentService>(Constants.ApiDomain);
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
                        ApplicationPaymentDetail = applicationPaymentDetail
                    };

                    HttpResponseMessage rawResponse = await service.ApplicationPayment(request
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
            , string srNumber)
        {
            try
            {
                IPaymentService service = RestService.For<IPaymentService>(Constants.ApiDomain);
                try
                {
                    PostApplicationsPaidDetailsRequest request = new PostApplicationsPaidDetailsRequest
                    {
                        UserInfo = userInfo,
                        SRNumber = srNumber
                    };

                    HttpResponseMessage rawResponse = await service.GetApplicationsPaidDetails(request
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
                catch (ApiException apiEx)
                {
#if DEBUG
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
        /// <param name="accountNumber">CA Number</param>
        /// <returns></returns>
        public async Task<byte[]> GetTaxInvoice(string srNumber)
        {
            GetTaxInvoiceResponse response;
            try
            {
                IPaymentService service = RestService.For<IPaymentService>(Constants.ApiDomain);
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
            response.StatusDetail = Constants.Service_GetTaxInvoice.GetStatusDetails(Constants.DEFAULT);
            return null;
        }
        #endregion

        #region Tax Invoice URL
        public string GetTaxInvoiceURL(string srNumber)
        {
            //Todo: Remove Stub
            srNumber = "4000009613";
            try
            {
                if (srNumber.IsValid())
                {
                    string urlFormat = "{0}/{1}/{2}?apiKeyID={3}&apiKey={4}&srNumber={5}&lang={6}";
                    string url = string.Format(urlFormat
                        , Constants.ApiDomain
                        , Constants.ApiUrlPath
                        , Constants.Service_TaxInvoice
                        , Constants.ApiKeyId
                        , Constants.APIKey
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
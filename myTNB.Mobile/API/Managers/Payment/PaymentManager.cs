using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.Payment.ApplicationPayment;
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
                        , NetworkService.GetCancellationToken());

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
    }
}
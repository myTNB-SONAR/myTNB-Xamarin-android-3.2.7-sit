using myTNB.Mobile.API.Models.ApplicationStatus;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Payment.ApplicationPayment
{
    public class PostApplicationPaymentRequest : BaseRequest
    {
        [JsonProperty("customerName")]
        public string CustomerName { set; get; }

        [JsonProperty("phoneNo")]
        public string PhoneNo { set; get; }

        [JsonProperty("platform")]
        public string Platform { set; get; }

        [JsonProperty("registeredCardId")]
        public string RegisteredCardId { set; get; }

        [JsonProperty("paymentMode")]
        public string PaymentMode { set; get; }

        [JsonProperty("totalAmount")]
        public string TotalAmount { set; get; }

        [JsonProperty("applicationPaymentDetail")]
        public ApplicationPaymentDetail ApplicationPaymentDetail { set; get; }
    }
}
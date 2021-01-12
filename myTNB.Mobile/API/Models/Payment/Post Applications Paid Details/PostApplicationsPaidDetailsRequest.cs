using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails
{
    public class PostApplicationsPaidDetailsRequest : BaseRequest
    {
        [JsonProperty("applicationPayment")]
        public ApplicationPayment ApplicationPayment { set; get; }
    }

    public class ApplicationPayment
    {
        [JsonProperty("srNumber")]
        public string SRNumber { set; get; }
        [JsonProperty("statusId")]
        public string StatusId { set; get; }
        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }
        [JsonProperty("applicationType")]
        public string ApplicationType { set; get; }
    }
}
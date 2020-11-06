using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails
{
    public class PostApplicationsPaidDetailsRequest : BaseRequest
    {
        [JsonProperty("srNumber")]
        public string SRNumber { set; get; }
    }
}
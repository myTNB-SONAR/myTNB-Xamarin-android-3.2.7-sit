using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.Base.Models
{
    public class PreLoginFeedback
    {
        [JsonProperty("ServiceReqNo")]
        public string FeedbackId { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }
    }
}
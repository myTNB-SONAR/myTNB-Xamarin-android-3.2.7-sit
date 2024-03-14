using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.Base.Request
{
    public class FeedbackRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("feedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("feedbackTypeId")]
        public string FeedbackTypeId { get; set; }

        [JsonProperty("accountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phoneNum")]
        public string PhoneNum { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("feedbackMesage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("stateId")]
        public string StateId { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("poleNum")]
        public string PoleNum { get; set; }

        [JsonProperty("images")]
        public List<AttachedImageRequest> Images { get; set; }

    }
}
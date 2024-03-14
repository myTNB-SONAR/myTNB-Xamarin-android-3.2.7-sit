using Newtonsoft.Json;

namespace myTNB.Android.Src.UpdateMobileNo.Models
{
    public class UpdatePhoneTokenSMSResponse
    {
        [JsonProperty("d")]
        public SendUpdatePhoneTokenSMSResult Data { get; set; }

        public class SendUpdatePhoneTokenSMSResult
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
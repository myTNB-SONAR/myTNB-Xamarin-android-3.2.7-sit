using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class PhoneVerifyStatusResponse
    {

        [JsonProperty("d")]
        public PhoneVerificationData verificationData { get; set; }

        public class PhoneVerificationData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public PhoneVerifyStatus Data { get; set; }
        }
    }
}
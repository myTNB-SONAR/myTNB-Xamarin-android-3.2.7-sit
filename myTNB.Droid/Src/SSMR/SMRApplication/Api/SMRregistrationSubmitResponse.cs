using System;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMR.SMRApplication.Api
{
    public class SMRregistrationSubmitResponse
    {
        [JsonProperty(PropertyName = "d")]
        public SMRSubmitResponseData Data { get; set; }

        public class SMRSubmitResponseData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public SMRSubmitResponseDetails AccountDetailsData { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }
        }

        public class SMRSubmitResponseDetails
        {
            [JsonProperty(PropertyName = "Status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "ServiceReqNo")]
            public string ServiceReqNo { get; set; }

            [JsonProperty(PropertyName = "ApplicationID")]
            public string ApplicationID { get; set; }

            [JsonProperty(PropertyName = "AppliedOn")]
            public string AppliedOn { get; set; }

            [JsonProperty(PropertyName = "StatusText")]
            public string StatusText { get; set; }
        }
    }
}

using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class BaseResponse<T>
    {
        [JsonProperty(PropertyName = "d")]
        public ResponseD Response { get; set; }

        public class ResponseD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public T Data { get; set; }

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
    }
}

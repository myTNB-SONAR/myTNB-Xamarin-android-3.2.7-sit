using System;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;
namespace myTNB_Android.Src.MyTNBService.Response
{
    public class NCAutoAddAccountsResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ResponseD Response { get; set; }

        public bool IsSuccessResponse()
        {
            bool IsSuccess = false;
            if (Response != null && Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }

        public class ResponseD
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

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

            [JsonProperty(PropertyName = "RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "IsPayEnabled")]
            public bool IsPayEnabled { get; set; }
        }
    }
}

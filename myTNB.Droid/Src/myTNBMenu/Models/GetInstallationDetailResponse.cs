using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class GetInstallationDetailResponse
    {
        [JsonProperty("d")]
        public InstallationDetailsData Data { get; set; }

        public class InstallationDetailsData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public AccountStatusData Data { get; set; }
        }

        public class AccountStatusData
        {
            [JsonProperty(PropertyName = "DisconnectionStatus")]
            public string DisconnectionStatus { get; set; }

            [JsonProperty(PropertyName = "AccountStatusModalTitle")]
            public string AccountStatusModalTitle { get; set; }

            [JsonProperty(PropertyName = "AccountStatusModalMessage")]
            public string AccountStatusModalMessage { get; set; }

            [JsonProperty(PropertyName = "AccountStatusModalBtnText")]
            public string AccountStatusModalBtnText { get; set; }

            [JsonProperty(PropertyName = "AccountStatusMessage")]
            public string AccountStatusMessage { get; set; }
        }
    }
}

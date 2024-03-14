using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class GetInstallationDetailsResponse
    {
        [JsonProperty("d")]
        public InstallationDetailsData Data { get; set; }

        public class InstallationDetailsData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }


            [JsonProperty(PropertyName = "RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty(PropertyName = "RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty(PropertyName = "RefreshBtnText")]
            public string RefreshBtnText { get; set; }

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

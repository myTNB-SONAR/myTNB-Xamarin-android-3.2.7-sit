using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class CheckPendingPaymentsResponse
    {

        [JsonProperty("d")]
        public CheckPendingPaymentsData Data { get; set; }

        public class CheckPendingPaymentsData
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

            [JsonProperty(PropertyName = "IsPayEnabled")]
            public bool IsPayEnabled { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<CheckPendingPayments> Data { get; set; }
        }

        public class CheckPendingPayments
        {
            [JsonProperty("contractAccountNumber")]
            public string ContractAccountNumber { get; set; }

            [JsonProperty("HasPendingPayment")]
            public bool HasPendingPayment { get; set; }

            [JsonProperty("PaymentAmount")]
            public string PaymentAmount { get; set; }

            [JsonProperty("MerchantTransactionId")]
            public string MerchantTransactionId { get; set; }
        }

    }
}
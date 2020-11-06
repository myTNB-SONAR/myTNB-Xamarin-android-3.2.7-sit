using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails
{
    public class PostApplicationsPaidDetailsResponse
    {
        [JsonProperty("d")]
        public PostApplicationsPaidDetailsModel D { set; get; }
    }

    public class PostApplicationsPaidDetailsModel
    {
        [JsonProperty("data")]
        public List<PostApplicationsPaidDetailsDataModel> Data { set; get; }
        [JsonProperty("status")]
        public string Status { set; get; } = string.Empty;
        [JsonProperty("isError")]
        public string IsError { set; get; } = string.Empty;
        [JsonProperty("message")]
        public string Message { set; get; } = string.Empty;
        [JsonProperty("ErrorCode")]
        public string ErrorCode { set; get; } = string.Empty;
        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { set; get; } = string.Empty;
        [JsonProperty("DisplayMessage")]
        public string DisplayMessage { set; get; } = string.Empty;
        [JsonProperty("DisplayType")]
        public string DisplayType { set; get; } = string.Empty;
        [JsonProperty("DisplayTitle")]
        public string DisplayTitle { set; get; } = string.Empty;
        [JsonProperty("RefreshTitle")]
        public string RefreshTitle { set; get; } = string.Empty;
        [JsonProperty("RefreshMessage")]
        public string RefreshMessage { set; get; } = string.Empty;
        [JsonProperty("RefreshBtnText")]
        public string RefreshBtnText { set; get; } = string.Empty;
        [JsonProperty("IsPayEnabled")]
        public bool IsPayEnabled { set; get; }
    }

    public class PostApplicationsPaidDetailsDataModel
    {
        [JsonProperty("SrNumber")]
        public string SRNumber { set; get; } = string.Empty;
        [JsonProperty("MerchantTransId")]
        public string MerchantTransID { set; get; } = string.Empty;
        [JsonProperty("PaymentDoneDate")]
        public DateTime? PaymentDoneDate { set; get; }
        [JsonProperty("Amount")]
        public double Amount { set; get; }
        [JsonProperty("AccNumber")]
        public string AccNumber { set; get; } = string.Empty;
        [JsonProperty("IsPaymentPending")]
        public bool IsPaymentPending { set; get; }
        [JsonProperty("AccountPayments")]
        public List<AccountPaymentsModel> AccountPayments { set; get; }
    }

    public class AccountPaymentsModel
    {
        [JsonProperty("PaymentType")]
        public string PaymentType { set; get; } = string.Empty;
        [JsonProperty("PaymentAmount")]
        public string PaymentAmount { set; get; } = string.Empty;
        [JsonProperty("RevenueCode")]
        public string RevenueCode { set; get; } = string.Empty;
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API
{
    public class AccountChargesResponse
    {
        [JsonProperty(PropertyName = "d")]
        public APIResponseResult Data { get; set; }

        public class APIResponseResult
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public AccountChargesResponseData ResponseData { get; set; }

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
        }

        public class AccountChargesResponseData
        {
            [JsonProperty(PropertyName = "AccountCharges")]
            public List<AccountCharge> AccountCharges { get; set; }

            [JsonProperty(PropertyName = "MandatoryChargesPopUpDetails")]
            public List<MandatoryChargesPopUpDetail> MandatoryChargesPopUpDetails { get; set; }
        }

        public class AccountCharge
        {
            [JsonProperty(PropertyName = "ContractAccount")]
            public string ContractAccount { get; set; }

            [JsonProperty(PropertyName = "CurrentCharges")]
            public float CurrentCharges { get; set; }

            [JsonProperty(PropertyName = "OutstandingCharges")]
            public float OutstandingCharges { get; set; }

            [JsonProperty(PropertyName = "AmountDue")]
            public float AmountDue { get; set; }

            [JsonProperty(PropertyName = "DueDate")]
            public string DueDate { get; set; }

            [JsonProperty(PropertyName = "BillDate")]
            public string BillDate { get; set; }

            [JsonProperty(PropertyName = "IncrementREDueDateByDays")]
            public string IncrementREDueDateByDays { get; set; }

            [JsonProperty(PropertyName = "MandatoryCharges")]
            public MandatoryCharge MandatoryCharges { get; set; }
        }

        public class MandatoryCharge
        {
            [JsonProperty(PropertyName = "TotalAmount")]
            public float TotalAmount { get; set; }

            [JsonProperty(PropertyName = "Charges")]
            public List<Charge> Charges { get; set; }
        }

        public class Charge
        {
            [JsonProperty(PropertyName = "Key")]
            public string Key { get; set; }

            [JsonProperty(PropertyName = "Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Amount")]
            public float Amount { get; set; }
        }

        public class MandatoryChargesPopUpDetail
        {
            [JsonProperty(PropertyName = "Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Description")]
            public string Description { get; set; }

            [JsonProperty(PropertyName = "CTA")]
            public string CTA { get; set; }

            [JsonProperty(PropertyName = "Type")]
            public string Type { get; set; }
        }
    }
}

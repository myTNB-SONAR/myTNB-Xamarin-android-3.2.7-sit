using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class AccountChargesResponse : BaseResponse<AccountChargesResponse.AccountChargesResponseData>
    {
        public AccountChargesResponseData GetData()
        {
            return Response.Data;
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
            public double CurrentCharges { get; set; }

            [JsonProperty(PropertyName = "OutstandingCharges")]
            public double OutstandingCharges { get; set; }

            [JsonProperty(PropertyName = "AmountDue")]
            public double AmountDue { get; set; }

            [JsonProperty(PropertyName = "ActualCurrentCharges")]
            public double ActualCurrentCharges { get; set; }

            [JsonProperty(PropertyName = "DueDate")]
            public string DueDate { get; set; }

            [JsonProperty(PropertyName = "BillDate")]
            public string BillDate { get; set; }

            [JsonProperty(PropertyName = "IncrementREDueDateByDays")]
            public string IncrementREDueDateByDays { get; set; }

            [JsonProperty(PropertyName = "ShowEppToolTip")]
            public bool ShowEppToolTip { get; set; }

            [JsonProperty(PropertyName = "MandatoryCharges")]
            public MandatoryCharge MandatoryCharges { get; set; }
        }

        public class MandatoryCharge
        {
            [JsonProperty(PropertyName = "TotalAmount")]
            public double TotalAmount { get; set; }

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
            public double Amount { get; set; }
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

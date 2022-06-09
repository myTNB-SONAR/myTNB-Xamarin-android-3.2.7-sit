using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class AccountDetails
    {

        [JsonProperty("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("accName")]
        public string AccountName { get; set; }

        [JsonProperty("accICNo")]
        public string AccICNo { get; set; }

        [JsonProperty("accICNoNew")]
        public string AccICNoNew { get; set; }

        [JsonProperty("accComNo")]
        public string AccComno { get; set; }

        [JsonProperty("amDeposit")]
        public double AmtDeposit { get; set; }

        [JsonProperty("amCurrentChg")]
        public double AmtCurrentChg { get; set; }

        [JsonProperty("amOutstandingChg")]
        public double AmtOutstandingChg { get; set; }

        [JsonProperty("amPayableChg")]
        public double AmtPayableChg { get; set; }

        [JsonProperty("amLastPay")]
        public double AmtLastPay { get; set; }

        [JsonProperty("dateBill")]
        public string DateBill { get; set; }

        [JsonProperty("datePymtDue")]
        public string DatePaymentDue { get; set; }

        [JsonProperty("dateLastPay")]
        public string DateLastPay { get; set; }

        [JsonProperty("sttSupply")]
        public string SttSupply { get; set; }

        [JsonProperty("addStreet")]
        public string AddStreet { get; set; }

        [JsonProperty("addArea")]
        public string AddArea { get; set; }

        [JsonProperty("addTown")]
        public string AddTown { get; set; }

        [JsonProperty("addState")]
        public string AddState { get; set; }

        [JsonProperty("stnName")]
        public string StnName { get; set; }

        [JsonProperty("stnAddStreet")]
        public string StnAddStreet { get; set; }

        [JsonProperty("stnAddArea")]
        public string StnAddArea { get; set; }

        [JsonProperty("stnAddTown")]
        public string StnAddTown { get; set; }

        [JsonProperty("stnAddState")]
        public string StnAddState { get; set; }

        [JsonProperty("amCustBal")]
        public double AmtCustBal { get; set; }

        [JsonProperty("ItemizedBillings")]
        public List<ItemizedBillingDetails> ItemizedBilling { get; set; }   

        [JsonProperty("OpenChargesTotal")]
        public double OpenChargesTotal { get; set; }     

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("isError")]
        public bool isError { get; set; }

        [JsonProperty("WhatIsThisLink")]
        public string WhatIsThisLink { get; set; }

        [JsonProperty("WhatIsThisTitle")]
        public string WhatIsThisTitle { get; set; }

        [JsonProperty("WhatIsThisMessage")]
        public string WhatIsThisMessage { get; set; }

        [JsonProperty("WhatIsThisButtonText")]
        public string WhatIsThisButtonText { get; set; }

        [JsonProperty("smartMeterCode")]
        public string SmartMeterCode { get; set; }

        [JsonProperty("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

    }
}
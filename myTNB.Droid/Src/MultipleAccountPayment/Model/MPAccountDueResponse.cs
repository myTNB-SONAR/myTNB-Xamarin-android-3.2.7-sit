using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.MultipleAccountPayment.Model
{
    public class MPAccountDueResponse
    {

        [JsonProperty(PropertyName = "d")]
        [AliasAs("d")]
        public AccountDueAmountResponse accountDueAmountResponse { get; set; }

        public class AccountDueAmountResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            [AliasAs("status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "MandatoryChargesTitle")]
            [AliasAs("MandatoryChargesTitle")]
            public string MandatoryChargesTitle { get; set; }

            [JsonProperty(PropertyName = "MandatoryChargesMessage")]
            [AliasAs("MandatoryChargesMessage")]
            public string MandatoryChargesMessage { get; set; }

            [JsonProperty(PropertyName = "MandatoryChargesPriButtonText")]
            [AliasAs("MandatoryChargesPriButtonText")]
            public string MandatoryChargesPriButtonText { get; set; }

            [JsonProperty(PropertyName = "MandatoryChargesSecButtonText")]
            [AliasAs("MandatoryChargesSecButtonText")]
            public string MandatoryChargesSecButtonText { get; set; }

            [JsonProperty(PropertyName = "isError")]
            [AliasAs("isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            [AliasAs("message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            [AliasAs("data")]
            public List<Account> accounts { get; set; }
        }

        public class Account
        {
            [JsonProperty(PropertyName = "amountDue")]
            [AliasAs("amountDue")]
            public double amountDue { get; set; }

            [JsonProperty(PropertyName = "billDueDate")]
            [AliasAs("billDueDate")]
            public string billDueDate { get; set; }

            [JsonProperty(PropertyName = "accNum")]
            [AliasAs("accNum")]
            public string accNum { get; set; }

            [JsonProperty(PropertyName = "OpenChargesTotal")]
            [AliasAs("OpenChargesTotal")]
            public double OpenChargesTotal { get; set; } 
        }
    }
}
using myTNB.AndroidApp.Src.AddAccount.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class AccountDueAmount
    {
        [JsonProperty("AccountAmountDue")]
        public AccountDueAmountData AmountDueData { get; set; }
    }

    public class AccountDueAmountData
    {
        [JsonProperty("amountDue")]
        public double AmountDue { get; set; }

        [JsonProperty("billDueDate")]
        public string BillDueDate { get; set; }

        [JsonProperty("IncrementREDueDateByDays")]
        public string IncrementREDueDateByDays { get; set; }

        [JsonProperty("ItemizedBillings")]
        public List<ItemizedBillingDetails> ItemizedBilling { get; set; }

        [JsonProperty("OpenChargesTotal")]
        public double OpenChargesTotal { get; set; }

        [JsonProperty("WhyThisAmountLink")]
        public string WhyThisAmountLink { get; set; }

        [JsonProperty("WhyThisAmountTitle")]
        public string WhyThisAmountTitle { get; set; }

        [JsonProperty("WhyThisAmountMessage")]
        public string WhyThisAmountMessage { get; set; }

        [JsonProperty("WhyThisAmountPriButtonText")]
        public string WhyThisAmountPriButtonText { get; set; }

        [JsonProperty("WhyThisAmountSecButtonText")]
        public string WhyThisAmountSecButtonText { get; set; }

        [JsonProperty("ShowEppToolTip")]
        public bool ShowEppToolTip { get; set; }
    }
}
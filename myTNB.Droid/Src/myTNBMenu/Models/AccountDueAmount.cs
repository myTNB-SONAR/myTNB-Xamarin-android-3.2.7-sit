using myTNB_Android.Src.AddAccount.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class AccountDueAmount
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
    }
}
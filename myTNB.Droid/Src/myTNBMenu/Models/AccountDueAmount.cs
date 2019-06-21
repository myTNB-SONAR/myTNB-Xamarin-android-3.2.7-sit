using Newtonsoft.Json;

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
    }
}
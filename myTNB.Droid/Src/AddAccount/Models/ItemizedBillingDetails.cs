using Newtonsoft.Json;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class ItemizedBillingDetails
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
using Newtonsoft.Json;

namespace myTNB.Android.Src.AddAccount.Models
{
    public class ItemizedBillingDetails
    {
        [JsonProperty("NonConsumpChargeName")]
        public string NonConsumpChargeName { get; set; }

        [JsonProperty("NonConsumpChargeValue")]
        public double NonConsumpChargeValue { get; set; }
    }
}
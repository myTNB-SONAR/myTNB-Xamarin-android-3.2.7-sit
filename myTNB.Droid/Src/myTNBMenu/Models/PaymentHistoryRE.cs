using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class PaymentHistoryRE
    {

        //[JsonProperty("__type")]
        //[AliasAs("__type")]
        //public string Type { get; set; }

        [JsonProperty("Amount")]
        [AliasAs("Amount")]
        public double Amount { get; set; }

        [JsonProperty("BillConsumption")]
        [AliasAs("BillConsumption")]
        public string BillConsumption { get; set; }

        [JsonProperty("DocumentDate")]
        [AliasAs("DocumentDate")]
        public string DocumentDate { get; set; }

        [JsonProperty("DocumentNo")]
        [AliasAs("DocumentNo")]
        public string DocumentNo { get; set; }

        [JsonProperty("OutgoingPaymentType")]
        [AliasAs("OutgoingPaymentType")]
        public string OutgoingPaymentType { get; set; }

        [JsonProperty("PaidDate")]
        [AliasAs("PaidDate")]
        public string PaidDate { get; set; }

        [JsonProperty("Status")]
        [AliasAs("Status")]
        public string Status { get; set; }

    }
}
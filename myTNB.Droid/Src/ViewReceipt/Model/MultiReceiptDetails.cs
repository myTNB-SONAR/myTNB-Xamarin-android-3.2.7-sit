using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ViewReceipt.Model
{
    public class MultiReceiptDetails
    {
        [JsonProperty(PropertyName = "referenceNum")]
        [AliasAs("referenceNum")]
        public string referenceNum { get; set; }

        [JsonProperty(PropertyName = "accountNum")]
        [AliasAs("accountNum")]
        public string accountNum { get; set; }

        [JsonProperty(PropertyName = "customerName")]
        [AliasAs("customerName")]
        public string customerName { get; set; }

        [JsonProperty(PropertyName = "customerEmail")]
        [AliasAs("customerEmail")]
        public string customerEmail { get; set; }

        [JsonProperty(PropertyName = "customerPhone")]
        [AliasAs("customerPhone")]
        public string customerPhone { get; set; }

        [JsonProperty(PropertyName = "payAmt")]
        [AliasAs("payAmt")]
        public string payAmt { get; set; }

        [JsonProperty(PropertyName = "payMethod")]
        [AliasAs("payMethod")]
        public string payMethod { get; set; }

        [JsonProperty(PropertyName = "payTransID")]
        [AliasAs("payTransID")]
        public string payTransID { get; set; }

        [JsonProperty(PropertyName = "paySellerNum")]
        [AliasAs("paySellerNum")]
        public string paySellerNum { get; set; }

        [JsonProperty(PropertyName = "payTransStatus")]
        [AliasAs("payTransStatus")]
        public string payTransStatus { get; set; }

        [JsonProperty(PropertyName = "payTransDate")]
        [AliasAs("payTransDate")]
        public string payTransDate { get; set; }

        [JsonProperty(PropertyName = "merchantID")]
        [AliasAs("merchantID")]
        public string merchantID { get; set; }

        [JsonProperty(PropertyName = "accMultiPay")]
        [AliasAs("accMultiPay")]
        public List<AccMultiPay> accMultiPay { get; set; }
    }

    public class AccMultiPay
    {
        [JsonProperty(PropertyName = "accountNum")]
        [AliasAs("accountNum")]
        public string accountNum { get; set; }

        [JsonProperty(PropertyName = "itmAmt")]
        [AliasAs("itmAmt")]
        public string itmAmt { get; set; }

        [JsonProperty(PropertyName = "AccountOwnerName")]
        [AliasAs("AccountOwnerName")]
        public string accountOwnerName { get; set; }
    }
}
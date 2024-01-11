using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ViewBill.Model
{
    public class GetBillMaskingRequest
    {
        [JsonProperty(PropertyName = "apiKeyID")]
        public string apiKeyID { get; set; }

        [JsonProperty(PropertyName = "contractAccount")]
        public string contractAccount { get; set; }

        [JsonProperty(PropertyName = "billingNo")]
        public string billingNo { get; set; }

        [JsonProperty(PropertyName = "isOwnerBill")]
        public bool isOwnerBill { get; set; }

        [JsonProperty(PropertyName = "lang")]
        public string lang { get; set; }
    }
}


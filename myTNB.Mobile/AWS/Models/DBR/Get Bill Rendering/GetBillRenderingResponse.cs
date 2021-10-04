using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class GetBillRenderingResponse : BaseStatus
    {
        [JsonProperty("contractAccountNumber")]
        public string ContractAccountNumber { set; get; }
        [JsonProperty("digitalBillEligibility")]
        public string DigitalBillEligibility { set; get; }
        [JsonProperty("digitalBillStatus")]
        public string DigitalBillStatus { set; get; }
        [JsonProperty("ownerBillRenderingMethod")]
        public string OwnerBillRenderingMethod { set; get; }
        [JsonProperty("ownerBillingEmail")]
        public string OwnerBillingEmail { set; get; }
        [JsonProperty("bcRecord")]
        public List<BCRecordModel> BCRecordModels { set; get; }
    }

    public class BCRecordModel
    {
        [JsonProperty("bcbpNumber")]
        public string BCBPNumber { set; get; }
        [JsonProperty("bcbpFirstName")]
        public string BCBPFirstName { set; get; }
        [JsonProperty("bcbpLastName")]
        public string BCBPLastName { set; get; }
        [JsonProperty("bcRenderingMethod")]
        public string BCRenderingMethod { set; get; }
        [JsonProperty("bcBillingEmail")]
        public string BCBillingEmail { set; get; }
    }
}
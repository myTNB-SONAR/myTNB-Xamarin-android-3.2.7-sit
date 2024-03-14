using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.SSMRTerminate.MVP
{
    public class SSMRTerminationReasonModel
    {
        [JsonProperty(PropertyName = "ReasonId")]
        [AliasAs("ReasonId")]
        public string ReasonId { get; set; }

        [JsonProperty(PropertyName = "ReasonName")]
        [AliasAs("ReasonName")]
        public string ReasonName { get; set; }

        [JsonProperty(PropertyName = "ReasonIcon")]
        [AliasAs("ReasonIcon")]
        public string ReasonIcon { get; set; }

        [JsonProperty(PropertyName = "ReasonCTA")]
        [AliasAs("ReasonCTA")]
        public string ReasonCTA { get; set; }

        [JsonProperty(PropertyName = "ReasonDescription")]
        [AliasAs("ReasonDescription")]
        public string ReasonDescription { get; set; }

        [JsonProperty(PropertyName = "OrderId")]
        [AliasAs("OrderId")]
        public string OrderId { get; set; }
    }
}
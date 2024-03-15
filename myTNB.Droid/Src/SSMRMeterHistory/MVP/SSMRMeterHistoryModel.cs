using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.SSMRMeterHistory.MVP
{
    public class SSMRMeterHistoryModel
    {
        [JsonProperty(PropertyName = "ReadingDate")]
        [AliasAs("ReadingDate")]
        public string ReadingDate { get; set; }

        [JsonProperty(PropertyName = "ReadingType")]
        [AliasAs("ReadingType")]
        public string ReadingType { get; set; }

        [JsonProperty(PropertyName = "ReadingReason")]
        [AliasAs("ReadingReason")]
        public string ReadingReason { get; set; }

        [JsonProperty(PropertyName = "ReadingValue")]
        [AliasAs("ReadingValue")]
        public string ReadingValue { get; set; }

        [JsonProperty(PropertyName = "Consumption")]
        [AliasAs("Consumption")]
        public string Consumption { get; set; }

        [JsonProperty(PropertyName = "ReadingTypeCode")]
        [AliasAs("ReadingTypeCode")]
        public string ReadingTypeCode { get; set; }

        [JsonProperty(PropertyName = "ReadingForMonth")]
        [AliasAs("ReadingForMonth")]
        public string ReadingForMonth { get; set; }
    }
}
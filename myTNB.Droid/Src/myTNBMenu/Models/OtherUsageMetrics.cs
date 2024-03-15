using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class OtherUsageMetrics
    {
        [JsonProperty("ElectricUsage")]
        public string ElectricUsage { get; set; }

        [JsonProperty("CO2Emission")]
        public string CO2Emission { get; set; }

        [JsonProperty("ElectricCost")]
        public string ElectricCost { get; set; }
    }
}
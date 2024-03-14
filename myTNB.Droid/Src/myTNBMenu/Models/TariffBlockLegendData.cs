using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class TariffBlocksLegendData
    {
        [JsonProperty(PropertyName = "BlockId")]
        [AliasAs("BlockId")]
        public string BlockId { get; set; }

        [JsonProperty(PropertyName = "BlockRange")]
        [AliasAs("BlockRange")]
        public string BlockRange { get; set; }

        [JsonProperty(PropertyName = "BlockPrice")]
        [AliasAs("BlockPrice")]
        public string BlockPrice { get; set; }

        [JsonProperty(PropertyName = "RGB")]
        [AliasAs("RGB")]
        public ColorData Color { get; set; }

        public class ColorData
        {
            [JsonProperty(PropertyName = "R")]
            [AliasAs("R")]
            public int RedColor { get; set; }

            [JsonProperty(PropertyName = "G")]
            [AliasAs("G")]
            public int GreenColor { get; set; }

            [JsonProperty(PropertyName = "B")]
            [AliasAs("B")]
            public int BlueData { get; set; }
        }
    }
}

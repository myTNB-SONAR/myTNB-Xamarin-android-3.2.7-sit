using System;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SSMR.SMRApplication.MVP
{
    public class SSMRWalkthroughData
    {

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("ID")]
        public string ID { get; set; }
    }
}

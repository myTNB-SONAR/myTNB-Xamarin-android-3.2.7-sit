using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class StatusDetail
    {
        [JsonProperty("code")]
        public string Code { set; get; }
#if DEBUG
        [JsonProperty("title")]
        public string Title { set; get; }

        [JsonProperty("description")]
        public string Description { set; get; }

        [JsonProperty("displayMode")]
        public string DisplayMode { set; get; }

        [JsonProperty("ctaText")]
        public string CTAText { set; get; }
#endif
    }
}
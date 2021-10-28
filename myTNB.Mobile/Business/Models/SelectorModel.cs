using Newtonsoft.Json;

namespace myTNB
{
    public class SelectorModel
    {
        [JsonProperty("id")]
        public string ID { set; get; }
        [JsonProperty("key")]
        public string Key { set; get; }
        [JsonProperty("value")]
        public string Value { set; get; }
        [JsonProperty("description")]
        public string Description { set; get; }
    }

    public class PopupSelectorModel
    {
        [JsonProperty("Title")]
        public string Title { set; get; } = string.Empty;
        [JsonProperty("Description")]
        public string Description { set; get; } = string.Empty;
        [JsonProperty("CTA")]
        public string CTA { set; get; } = string.Empty;
        [JsonProperty("Type")]
        public string Type { set; get; } = string.Empty;
    }

    public class RebateTypeModel
    {
        [JsonProperty("key")]
        public string Key { set; get; }
        [JsonProperty("description")]
        public string Description { set; get; }
        [JsonProperty("needsIncident")]
        public bool NeedsIncident { set; get; }
    }
}
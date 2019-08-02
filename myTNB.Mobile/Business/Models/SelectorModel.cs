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
        public string Title { set; get; }
        [JsonProperty("Description")]
        public string Description { set; get; }
        [JsonProperty("CTA")]
        public string CTA { set; get; }
        [JsonProperty("Type")]
        public string Type { set; get; }
    }
}
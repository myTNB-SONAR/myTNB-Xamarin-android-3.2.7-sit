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
}
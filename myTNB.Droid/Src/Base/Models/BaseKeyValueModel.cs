using Newtonsoft.Json;

namespace myTNB.Android.Src.Base.Models
{
    public class BaseKeyValueModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}

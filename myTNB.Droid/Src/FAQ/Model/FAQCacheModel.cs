using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.FAQ.Model
{
    public class FAQCacheModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("data")]
        public List<FAQCacheList> Data { get; set; }
    }

    public class FAQCacheList
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }
    }
}
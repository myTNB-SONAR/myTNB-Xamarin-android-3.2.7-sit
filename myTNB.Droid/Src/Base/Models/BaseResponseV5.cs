using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.Base.Models
{
    public abstract class BaseResponseV5<T>
    {
        [JsonProperty("__type")]
        [AliasAs("__type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        [AliasAs("status")]
        public string status { get; set; }

        [JsonProperty(PropertyName = "isError")]
        [AliasAs("isError")]
        public bool isError { get; set; }

        [JsonProperty(PropertyName = "message")]
        [AliasAs("message")]
        public string message { get; set; }

        [JsonProperty(PropertyName = "data")]
        [AliasAs("data")]
        public List<T> data { get; set; }
    }
}

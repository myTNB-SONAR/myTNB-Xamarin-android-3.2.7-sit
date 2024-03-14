using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class WeblinkResponse
    {
        [JsonProperty("d")]
        public WeblinkResponseData Data { get; set; }

        public class WeblinkResponseData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("isError")]
            public bool IsError { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("data")]
            public List<Weblink> Data { get; set; }
        }
    }
}
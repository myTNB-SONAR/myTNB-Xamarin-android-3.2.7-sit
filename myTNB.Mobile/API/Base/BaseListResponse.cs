using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Base
{
    public class BaseListResponse<T>
    {
        [JsonProperty("content")]
        public List<T> Content { get; set; }

        [JsonProperty("statusDetail")]
        public StatusDetail StatusDetail { set; get; }
    }
}
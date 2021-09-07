using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Base
{
    public class BaseListResponse<T> : BaseStatus
    {
        [JsonProperty("content")]
        public List<T> Content { get; set; }
    }
}
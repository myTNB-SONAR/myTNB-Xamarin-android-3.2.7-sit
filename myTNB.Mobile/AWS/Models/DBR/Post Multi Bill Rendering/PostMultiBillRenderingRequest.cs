using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostMultiBillRenderingRequest
    {
        [JsonProperty("caNos")]
        public List<string> CANumbers { set; get; }
    }
}
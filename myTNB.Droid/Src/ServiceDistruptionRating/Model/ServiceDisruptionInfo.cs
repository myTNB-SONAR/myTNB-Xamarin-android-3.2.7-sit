using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB_Android.Src.ServiceDistruptionRating.Model
{
    public class ServiceDisruptionInfo
    {
        [JsonProperty(PropertyName = "email")]
        [AliasAs("email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "sdEventId")]
        [AliasAs("sdEventId")]
        public string sdEventId { get; set; }

        [JsonProperty(PropertyName = "subscriptionStatus")]
        [AliasAs("subscriptionStatus")]
        public bool subscriptionStatus { get; set; }
    }
}
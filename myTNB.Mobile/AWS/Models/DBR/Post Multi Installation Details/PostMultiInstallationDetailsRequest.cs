using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostMultiInstallationDetailsRequest
    {
        [JsonProperty("CaNos")]
        public List<string> CANumbers { set; get; }
    }
}
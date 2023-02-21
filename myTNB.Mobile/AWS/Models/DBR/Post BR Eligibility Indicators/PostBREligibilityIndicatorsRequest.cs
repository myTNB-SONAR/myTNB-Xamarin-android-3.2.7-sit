using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostBREligibilityIndicatorsRequest
    {
        [JsonProperty("CaNos")]
        public List<string> CaNos { set; get; }
        [JsonProperty("userID")]
        public string UserID { set; get; }
    }
}

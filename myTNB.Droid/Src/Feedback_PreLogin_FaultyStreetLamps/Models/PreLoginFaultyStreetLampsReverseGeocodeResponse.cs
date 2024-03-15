using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.Feedback_PreLogin_FaultyStreetLamps.Models
{
    public class PreLoginFaultyStreetLampsReverseGeocodeResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public List<ResultData> Result { get; set; }

        public class ResultData
        {
            [JsonProperty("formatted_address", Required = Required.AllowNull)]
            public string FormattedAddress { get; set; }
        }
    }
}
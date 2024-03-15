using myTNB.AndroidApp.Src.FindUs.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.FindUs.Response
{
    public class GetGoogleLocationsResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<GoogleApiResult> results { get; set; }

    }
}
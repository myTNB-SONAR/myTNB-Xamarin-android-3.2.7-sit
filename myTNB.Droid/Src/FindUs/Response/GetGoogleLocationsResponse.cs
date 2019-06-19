using myTNB_Android.Src.FindUs.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.FindUs.Response
{
    public class GetGoogleLocationsResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<GoogleApiResult> results { get; set; }

    }
}
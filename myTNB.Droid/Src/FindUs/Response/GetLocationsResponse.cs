using myTNB.AndroidApp.Src.FindUs.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.FindUs.Response
{
    public class GetLocationsResponse
    {
        [JsonProperty(PropertyName = "d")]
        public LocationsResponse D { get; set; }

        public class LocationsResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public List<LocationData> LocationList { get; set; }
        }

    }
}
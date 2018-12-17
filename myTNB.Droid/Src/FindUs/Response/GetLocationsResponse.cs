using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using myTNB_Android.Src.FindUs.Models;

namespace myTNB_Android.Src.FindUs.Response
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
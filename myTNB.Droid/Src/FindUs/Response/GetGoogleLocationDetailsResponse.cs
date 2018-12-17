﻿using System;
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

namespace myTNB_Android.Src.FindUs.Response
{
    public class GetGoogleLocationDetailsResponse
    {

        [JsonProperty(PropertyName = "result")]
        public GoogleLocationDetails result { get; set; }      
        
    }

    public class GoogleLocationDetails
    {
        [JsonProperty(PropertyName = "icon")]
        public string icon { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "international_phone_number")]
        public string international_phone_number { get; set; }

        [JsonProperty(PropertyName = "formatted_phone_number")]
        public string formatted_phone_number { get; set; }

        [JsonProperty(PropertyName = "formatted_address")]
        public string formatted_address { get; set; }
    }
}
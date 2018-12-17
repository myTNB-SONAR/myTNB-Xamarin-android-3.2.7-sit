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
    public class GetGoogleLocationsResponse
    {
        [JsonProperty(PropertyName = "results")]
        public List<GoogleApiResult> results { get; set; }

    }
}
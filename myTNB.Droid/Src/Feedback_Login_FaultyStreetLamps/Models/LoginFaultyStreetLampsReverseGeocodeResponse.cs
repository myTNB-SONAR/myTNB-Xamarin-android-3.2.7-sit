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

namespace myTNB_Android.Src.Feedback_Login_FaultyStreetLamps.Models
{
    public class LoginFaultyStreetLampsReverseGeocodeResponse
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
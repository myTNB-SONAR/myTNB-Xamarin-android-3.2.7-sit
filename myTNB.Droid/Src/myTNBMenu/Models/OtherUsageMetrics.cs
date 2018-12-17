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

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class OtherUsageMetrics
    {
        [JsonProperty("ElectricUsage")]
        public string ElectricUsage { get; set; }

        [JsonProperty("CO2Emission")]
        public string CO2Emission { get; set; }

        [JsonProperty("ElectricCost")]
        public string ElectricCost { get; set; }
    }
}
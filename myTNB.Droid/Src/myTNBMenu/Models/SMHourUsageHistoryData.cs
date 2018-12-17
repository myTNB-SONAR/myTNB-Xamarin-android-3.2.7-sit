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
using Refit;
using Newtonsoft.Json.Linq;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class SMHourUsageHistoryData
    {
        [JsonProperty(PropertyName = "OtherUsageMetrics")]
        [AliasAs("OtherUsageMetrics")]
        public OtherUsageMetricsData OtherUsageMetrics { get; set; }
        
        [JsonProperty(PropertyName = "ByHour")]
        [AliasAs("ByHour")]
        public List<ByHourData> ByHour { get; set; }
       
        public class OtherUsageMetricsData
        {
            [JsonProperty(PropertyName = "ElectricUsage")]
            [AliasAs("ElectricUsage")]
            public string ElectricUsage { get; set; }

            [JsonProperty(PropertyName = "CO2Emission")]
            [AliasAs("CO2Emission")]
            public string CO2Emission { get; set; }

            [JsonProperty(PropertyName = "ElectricCost")]
            [AliasAs("ElectricCost")]
            public string ElectricCost { get; set; }
        }


        public class ByHourData
        {
            [JsonProperty(PropertyName = "Date")]
            [AliasAs("Date")]
            public string Date { get; set; }

            [JsonProperty(PropertyName = "Hour")]
            [AliasAs("Hour")]
            public string Hour { get; set; }

            [JsonProperty(PropertyName = "Consumption")]
            [AliasAs("Consumption")]
            public string Consumption { get; set; }

            [JsonProperty(PropertyName = "Amount")]
            [AliasAs("Amount")]
            public string Amount { get; set; }

        }
    }

    
}
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class SMUsageHistoryData
    {

        [JsonProperty(PropertyName = "IsCO2Disabled")]
        [AliasAs("IsCO2Disabled")]
        public bool IsCO2Disabled { get; set; }

        [JsonProperty(PropertyName = "OtherUsageMetrics")]
        [AliasAs("OtherUsageMetrics")]
        public OtherUsageMetricsData OtherUsageMetrics { get; set; }

        [JsonProperty(PropertyName = "ByMonth")]
        [AliasAs("ByMonth")]
        public List<ByMonthData> ByMonth { get; set; }

        [JsonProperty(PropertyName = "ByDay")]
        [AliasAs("ByDay")]
        public List<ByDayData> ByDay { get; set; }

        [JsonProperty(PropertyName = "ToolTips")]
        [AliasAs("ToolTips")]
        public List<SmartMeterToolTips> ToolTips { get; set; }

        public class OtherUsageMetricsData
        {
            [JsonProperty(PropertyName = "StatsByCost")]
            [AliasAs("StatsByCost")]
            public StatsByCost StatsByCost { get; set; }

            [JsonProperty(PropertyName = "StatsByUsage")]
            [AliasAs("StatsByUsage")]
            public StatsByUsage StatsByUsage { get; set; }

            [JsonProperty(PropertyName = "StatsByCo2")]
            [AliasAs("StatsByCo2")]
            public List<StatsByCo2> StatsByCo2 { get; set; }

            [JsonProperty(PropertyName = "CurrentCycleStartDate")]
            [AliasAs("CurrentCycleStartDate")]
            public string CurrentCycleStartDate { get; set; }
        }

        public class StatsByCost
        {
            [JsonProperty(PropertyName = "CurrentCharges")]
            [AliasAs("CurrentCharges")]
            public string CurrentCharges { get; set; }

            [JsonProperty(PropertyName = "AsOf")]
            [AliasAs("AsOf")]
            public String AsOf { get; set; }

            [JsonProperty(PropertyName = "ProjectedCost")]
            [AliasAs("ProjectedCost")]
            public string ProjectedCost { get; set; }
        }

        public class StatsByUsage
        {
            [JsonProperty(PropertyName = "CurrentUsageKWH")]
            [AliasAs("CurrentUsageKWH")]
            public string CurrentUsageKWH { get; set; }

            [JsonProperty(PropertyName = "AsOf")]
            [AliasAs("AsOf")]
            public string AsOf { get; set; }

            [JsonProperty(PropertyName = "UsageComparedToPrevious")]
            [AliasAs("UsageComparedToPrevious")]
            public string UsageComparedToPrevious { get; set; }
        }

        public class StatsByCo2
        {
            [JsonProperty(PropertyName = "ItemName")]
            [AliasAs("ItemName")]
            public string ItemName { get; set; }

            [JsonProperty(PropertyName = "ItemUnit")]
            [AliasAs("ItemUnit")]
            public string ItemUnit { get; set; }

            [JsonProperty(PropertyName = "Quantity")]
            [AliasAs("Quantity")]
            public string Quantity { get; set; }

            [JsonProperty(PropertyName = "AsOf")]
            [AliasAs("AsOf")]
            public string AsOf { get; set; }
        }

        public class ByMonthData
        {
            [JsonProperty(PropertyName = "Range")]
            [AliasAs("Range")]
            public string Range { get; set; }

            [JsonProperty(PropertyName = "Months")]
            [AliasAs("Months")]
            public List<MonthData> Months { get; set; }

            public class MonthData
            {
                [JsonProperty(PropertyName = "Date")]
                [AliasAs("Date")]
                public string Date { get; set; }

                [JsonProperty(PropertyName = "Year")]
                [AliasAs("Year")]
                public string Year { get; set; }

                [JsonProperty(PropertyName = "Month")]
                [AliasAs("Month")]
                public string Month { get; set; }

                [JsonProperty(PropertyName = "Day")]
                [AliasAs("Day")]
                public string Day { get; set; }

                [JsonProperty(PropertyName = "Amount")]
                [AliasAs("Amount")]
                public string Amount { get; set; }

                [JsonProperty(PropertyName = "Consumption")]
                [AliasAs("Consumption")]
                public string Consumption { get; set; }

                [JsonProperty(PropertyName = "CO2")]
                [AliasAs("CO2")]
                public string CO2 { get; set; }
            }

        }

        public class ByDayData
        {
            [JsonProperty(PropertyName = "Range")]
            [AliasAs("Range")]
            public string Range { get; set; }

            [JsonProperty(PropertyName = "CurrentCycle")]
            [AliasAs("CurrentCycle")]
            public String CurrentCycle { get; set; }

            [JsonProperty(PropertyName = "Days")]
            [AliasAs("Days")]
            public List<DayData> Days { get; set; }

            [JsonProperty(PropertyName = "Index")]
            [AliasAs("Index")]
            public string Index { get; set; }


            public class DayData
            {
                [JsonProperty(PropertyName = "Date")]
                [AliasAs("Date")]
                public string Date { get; set; }

                [JsonProperty(PropertyName = "Month")]
                [AliasAs("Month")]
                public string Month { get; set; }

                [JsonProperty(PropertyName = "Day")]
                [AliasAs("Day")]
                public string Day { get; set; }

                [JsonProperty(PropertyName = "Amount")]
                [AliasAs("Amount")]
                public string Amount { get; set; }

                [JsonProperty(PropertyName = "Consumption")]
                [AliasAs("Consumption")]
                public string Consumption { get; set; }

                [JsonProperty(PropertyName = "CO2")]
                [AliasAs("CO2")]
                public string CO2 { get; set; }
            }
        }

        public class SmartMeterToolTips
        {
            [JsonProperty(PropertyName = "Type")]
            [AliasAs("Type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "Title")]
            [AliasAs("Title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "Message")]
            [AliasAs("Message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "SMLink")]
            [AliasAs("SMLink")]
            public string SMLink { get; set; }

            [JsonProperty(PropertyName = "SMBtnText")]
            [AliasAs("SMBtnText")]
            public string SMBtnText { get; set; }
        }
    }


}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.myTNBMenu.Models
{
    public class UsageHistoryData
    {
        [JsonProperty(PropertyName = "ByMonth")]
        [AliasAs("ByMonth")]
        public ByMonthData ByMonth { get; set; }

        [JsonProperty(PropertyName = "ByDay")]
        [AliasAs("ByDay")]
        public List<ByDayData> ByDay { get; set; }

        [JsonProperty(PropertyName = "TariffBlocksLegend")]
        [AliasAs("TariffBlocksLegend")]
        public List<TariffBlocksLegendData> TariffBlocksLegend { get; set; }

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

                [JsonProperty(PropertyName = "AmountTotal")]
                [AliasAs("AmountTotal")]
                public double AmountTotal { get; set; }

                [JsonProperty(PropertyName = "UsageTotal")]
                [AliasAs("UsageTotal")]
                public double UsageTotal { get; set; }

                [JsonProperty(PropertyName = "Currency")]
                [AliasAs("Currency")]
                public string Currency { get; set; }

                [JsonProperty(PropertyName = "UsageUnit")]
                [AliasAs("UsageUnit")]
                public string UsageUnit { get; set; }

                [JsonProperty(PropertyName = "IsEstimatedReading")]
                [AliasAs("IsEstimatedReading")]
                public string IsEstimatedReading { get; set; }

                [JsonProperty(PropertyName = "DPCIndicator")]
                [AliasAs("DPCIndicator")]
                public bool DPCIndicator { get; set; }

                [JsonProperty(PropertyName = "DPCIndicatorUsageMessage")]
                [AliasAs("DPCIndicatorUsageMessage")]
                public string DPCIndicatorUsageMessage { get; set; }

                [JsonProperty(PropertyName = "DPCIndicatorTariffMessage")]
                [AliasAs("DPCIndicatorTariffMessage")]
                public string DPCIndicatorTariffMessage { get; set; }

                [JsonProperty(PropertyName = "DPCIndicatorRMMessage")]
                [AliasAs("DPCIndicatorRMMessage")]
                public string DPCIndicatorRMMessage { get; set; }

                [JsonProperty(PropertyName = "tariffBlocks")]
                [AliasAs("tariffBlocks")]
                public List<TariffBlock> TariffBlocksList { get; set; }

                [JsonProperty(PropertyName = "isCurrentlyUnavailable")]
                [AliasAs("isCurrentlyUnavailable")]
                public bool isCurrentlyUnavailable { get; set; }
            }

            public class TariffBlock
            {
                [JsonProperty(PropertyName = "BlockId")]
                [AliasAs("BlockId")]
                public string BlockId { get; set; }

                [JsonProperty(PropertyName = "Amount")]
                [AliasAs("Amount")]
                public double Amount { get; set; }

                [JsonProperty(PropertyName = "Usage")]
                [AliasAs("Usage")]
                public double Usage { get; set; }

                [JsonProperty(PropertyName = "BlockPrice")]
                [AliasAs("BlockPrice")]
                public string BlockPrice { get; set; }
            }

            class DoubleDataAmountConverter : JsonConverter
            {
                public override bool CanConvert(Type objectType)
                {
                    return (objectType == typeof(Double) || objectType == typeof(String));
                }

                public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                {
                    double parseAmount = 0.00;
                    JToken jtoken = JToken.Load(reader);
                    if (jtoken.Type == JTokenType.String)
                    {
                        string val = jtoken.ToObject<String>();

                        if (double.TryParse(val, out parseAmount))
                        {
                            parseAmount = Double.Parse(val);
                        }
                        else
                        {
                            return parseAmount;
                        }
                    }

                    return parseAmount;
                }

                public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                {
                    serializer.Serialize(writer, value);
                }
            }
        }

        public class ByDayData
        {
            [JsonProperty(PropertyName = "Range")]
            [AliasAs("Range")]
            public string Range { get; set; }

            [JsonProperty(PropertyName = "Days")]
            [AliasAs("Days")]
            public List<DayData> Days { get; set; }


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
                public double Amount { get; set; }
            }
        }
    }


}
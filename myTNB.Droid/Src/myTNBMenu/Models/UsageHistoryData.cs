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
    public class UsageHistoryData
    {
        [JsonProperty(PropertyName = "ByMonth")]
        [AliasAs("ByMonth")]
        public ByMonthData ByMonth { get; set; }

        [JsonProperty(PropertyName = "ByDay")]
        [AliasAs("ByDay")]
        public List<ByDayData> ByDay { get; set; }

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
                [JsonProperty(PropertyName = "Month")]
                [AliasAs("Month")]
                public string Month { get; set; }
                [JsonProperty(PropertyName = "Amount")]
                //[JsonConverter(typeof(DoubleDataAmountConverter))]
                [AliasAs("Amount")]
                public double Amount { get; set; }
                [JsonProperty(PropertyName = "Usage")]
                [AliasAs("Usage")]
                public double Usage { get; set; }
                [JsonProperty(PropertyName = "IsEstimatedReading")]
                [AliasAs("IsEstimatedReading")]
                public string IsEstimatedReading { get; set; }
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
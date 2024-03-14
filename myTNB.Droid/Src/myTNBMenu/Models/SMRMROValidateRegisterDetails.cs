using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Models
{
    public class SMRMROValidateRegisterDetails
    {
        [JsonProperty(PropertyName = "RegisterNumber")]
        [AliasAs("RegisterNumber")]
        public string RegisterNumber { get; set; }

        [JsonProperty(PropertyName = "MroID")]
        [AliasAs("MroID")]
        public string MroID { get; set; }

        [JsonProperty(PropertyName = "PrevMrDate")]
        [AliasAs("PrevMrDate")]
        public string PrevMrDate { get; set; }

        [JsonProperty(PropertyName = "SchMrDate")]
        [AliasAs("SchMrDate")]
        public string SchMrDate { get; set; }

        [JsonProperty(PropertyName = "PrevMeterReading")]
        [AliasAs("PrevMeterReading")]
        public string PrevMeterReading { get; set; }

        [JsonProperty(PropertyName = "ReadingUnit")]
        [AliasAs("ReadingUnit")]
        public string ReadingUnit { get; set; }

        [JsonProperty(PropertyName = "ReadingUnitDisplayTitle")]
        [AliasAs("ReadingUnitDisplayTitle")]
        public string ReadingUnitDisplayTitle { get; set; }
    }
}

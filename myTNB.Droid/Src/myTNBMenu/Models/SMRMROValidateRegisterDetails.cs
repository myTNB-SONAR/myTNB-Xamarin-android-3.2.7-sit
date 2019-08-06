﻿using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Models
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
    }
}

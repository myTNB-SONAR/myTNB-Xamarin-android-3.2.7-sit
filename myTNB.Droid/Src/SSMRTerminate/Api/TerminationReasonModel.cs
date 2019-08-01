﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.SSMRTerminate.Api
{
    public class TerminationReasonModel
    {
        [JsonProperty(PropertyName = "ReasonId")]
        public string ReasonId { get; set; }

        [JsonProperty(PropertyName = "ReasonName")]
        public string ReasonName { get; set; }

        [JsonProperty(PropertyName = "ReasonIcon")]
        public string ReasonIcon { get; set; }

        [JsonProperty(PropertyName = "ReasonCTA")]
        public string ReasonCTA { get; set; }

        [JsonProperty(PropertyName = "ReasonDescription")]
        public string ReasonDescription { get; set; }

        [JsonProperty(PropertyName = "OrderId")]
        public string OrderId { get; set; }
    }
}

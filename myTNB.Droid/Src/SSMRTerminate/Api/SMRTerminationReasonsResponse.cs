using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMRTerminate.Api
{
    public class SMRTerminationReasonsResponse
    {
        [JsonProperty(PropertyName = "d")]
        public TerminationReasonsResponse Response { get; set; }

        public class TerminationReasonsResponse
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public TerminationReasons TerminationReasons { get; set; }

            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }

            [JsonProperty(PropertyName = "DisplayTitle")]
            public string DisplayTitle { get; set; }
        }

        public class TerminationReasons
        {
            [JsonProperty(PropertyName = "reasons")]
            public List<TerminationReasonModel> ReasonList { get; set; }
        }
    }
}

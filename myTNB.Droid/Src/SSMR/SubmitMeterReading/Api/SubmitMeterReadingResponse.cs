using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Api
{
    public class SubmitMeterReadingResponse
    {
        [JsonProperty(PropertyName = "d")]
        public SMRSubmitResponseData Data { get; set; }

        public class SMRSubmitResponseData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public SMRSubmitResponseDetails ResponseDetailsData { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

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

        public class SMRSubmitResponseDetails
        {
            [JsonProperty(PropertyName = "ContractAccount")]
            public string ContractAccount { get; set; }

            [JsonProperty(PropertyName = "SubmitSMRMeterReadingsResp")]
            public List<SubmitSMRMeterReadingsResp> SubmitSMRMeterReadingsResp { get; set; }
        }

        public class SubmitSMRMeterReadingsResp
        {
            [JsonProperty(PropertyName = "MessageID")]
            public string MessageID { get; set; }

            [JsonProperty(PropertyName = "Message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "RegisterNumber")]
            public string RegisterNumber { get; set; }

            [JsonProperty(PropertyName = "ReadingUnit")]
            public string ReadingUnit { get; set; }

            [JsonProperty(PropertyName = "IsSuccess")]
            public bool IsSuccess { get; set; }
            
        }
    }
}

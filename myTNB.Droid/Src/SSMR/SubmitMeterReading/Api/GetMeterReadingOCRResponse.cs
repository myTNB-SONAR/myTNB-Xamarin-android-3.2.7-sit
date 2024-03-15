using System;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Api
{
    public class GetMeterReadingOCRResponse
    {
        [JsonProperty(PropertyName = "d")]
        public GetMeterReadingOCRResponseData Data { get; set; }

        public class GetMeterReadingOCRResponseData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public GetMeterReadingOCRResponseDetails ResponseDetailsData { get; set; }

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

        public class GetMeterReadingOCRResponseDetails
        {
            [JsonProperty(PropertyName = "RequestReadingUnit")]
            public string RequestReadingUnit { get; set; }

            [JsonProperty(PropertyName = "ImageId")]
            public string ImageId { get; set; }

            [JsonProperty(PropertyName = "OCRValue")]
            public string OCRValue { get; set; }

            [JsonProperty(PropertyName = "OCRUnit")]
            public string OCRUnit { get; set; }

            [JsonProperty(PropertyName = "IsSuccess")]
            public string IsSuccess { get; set; }
        }
    }
}

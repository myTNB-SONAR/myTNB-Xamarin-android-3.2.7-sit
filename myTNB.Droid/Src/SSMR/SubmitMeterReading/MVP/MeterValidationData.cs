using System;
namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterValidationData
    {
        public string messageId { get; set; }
        public string message { get; set; }
        public string registerNumber { get; set; }
        public string meterReadingUnit { get; set; }
        public bool isSuccess { get; set; }
    }
}

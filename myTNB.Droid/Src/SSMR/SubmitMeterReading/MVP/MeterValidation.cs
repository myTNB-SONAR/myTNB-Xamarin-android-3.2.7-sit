using System;
namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterValidation
    {
        public MeterValidation()
        {
        }
        public string mroID { get; set; }
        public string registerNumber { get; set; }
        public string meterId { get; set; }
        public string readingResult { get; set; }
        public bool validated { get; set; }
    }
}

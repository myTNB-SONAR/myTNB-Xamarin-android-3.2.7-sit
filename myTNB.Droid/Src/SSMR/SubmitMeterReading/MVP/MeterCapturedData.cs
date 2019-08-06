using System;
namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterCapturedData
    {
        public MeterCapturedData()
        {
        }
		public string meterId { get; set; }
		public bool isSelected { get; set; }
		public bool hasImage { get; set; }
	}
}

using System;
using Android.Graphics;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterImageModel
    {
        public string RequestReadingUnit { set; get; }

        public string ImageId { set; get; }

        public string ImageSize { set; get; }

        public Bitmap ImageData { set; get; }
    }
}

using System;
using System.Collections.Generic;
using Android.Graphics;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterTakePhotoContract
    {
        public interface IView
        {

        }

        public interface IPresenter
        {
            void GetMeterReadingOCRValue(string contractNumber);
            List<MeterImageModel> GetMeterImages();
            void AddMeterImage(string readingUnit, string imageId, Bitmap imageBitmap);
        }
    }
}

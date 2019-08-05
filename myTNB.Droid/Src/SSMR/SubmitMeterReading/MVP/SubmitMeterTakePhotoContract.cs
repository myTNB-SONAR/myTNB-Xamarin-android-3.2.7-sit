using System;
using System.Collections.Generic;
using Android.Graphics;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterTakePhotoContract
    {
        public interface IView
        {
            void ShowOCRLoading();
            void ShowMeterReadingPage(string resultResponse);
        }

        public interface IPresenter
        {
            void InitializeModelList();
            void GetMeterReadingOCRValue(string contractNumber);
            List<MeterImageModel> GetMeterImages();
            void AddMeterImage(string readingUnit, string imageId, Bitmap imageBitmap);
            void AddMeterImageAt(int imagePosition, string readingUnit, string imageId, Bitmap imageBitmap);
            void RemoveMeterImageAt(int position);
        }
    }
}

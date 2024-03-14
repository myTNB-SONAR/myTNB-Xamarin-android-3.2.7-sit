using System;
using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterTakePhotoContract
    {
        public interface IView
        {
            void ShowOCRLoading();
            void ShowMeterReadingPage(string resultResponse);
            void ShowMeterReadingPageWithError();
        }

        public interface IPresenter
        {
            void InitializeModelList();
            void InitializeModelList(int count);
            void GetMeterReadingOCRValue(string contractNumber);
            List<MeterImageModel> GetMeterImages();
            void AddMeterImage(string readingUnit, string imageId, Bitmap imageBitmap);
            void AddMeterImageAt(int imagePosition, string readingUnit, string imageId, Bitmap imageBitmap);
            void RemoveMeterImageAt(int position);
            void SetMeterImageList(List<MeterImageModel> meterImageDataList);
        }
    }
}

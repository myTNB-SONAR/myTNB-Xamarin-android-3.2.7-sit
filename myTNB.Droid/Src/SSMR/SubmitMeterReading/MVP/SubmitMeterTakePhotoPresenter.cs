using System;
using System.Collections.Generic;
using Android.Graphics;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRValueRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterTakePhotoPresenter : SubmitMeterTakePhotoContract.IPresenter
    {
        SubmitMeterTakePhotoContract.IView mView;
        SubmitMeterReadingApi api;
        List<MeterImageModel> meterImageList;

        public SubmitMeterTakePhotoPresenter(SubmitMeterTakePhotoContract.IView view)
        {
            mView = view;
            api = new SubmitMeterReadingImpl();
            meterImageList = new List<MeterImageModel>();
        }

        public async void GetMeterReadingOCRValue(string contractAccount)
        {
            GetMeterReadingOCRValueRequest request;
            GetMeterReadingOCRResponse response;
            foreach (MeterImageModel meterImageModel in meterImageList)
            {
                MeterImage meterImage = new MeterImage();
                meterImage.RequestReadingUnit = meterImageModel.RequestReadingUnit;
                meterImage.ImageId = meterImageModel.ImageId;
                meterImage.ImageSize = meterImageModel.ImageData.ByteCount.ToString();
                meterImage.ImageData = Utils.ImageUtils.GetBase64FromBitmap(meterImageModel.ImageData);

                request = new GetMeterReadingOCRValueRequest(contractAccount, meterImage);
                //response = await api.GetMeterReadingOCRValue(request);
            }

        }

        public List<MeterImageModel> GetMeterImages()
        {
            return meterImageList;
        }

        public void AddMeterImage(string readingUnit, string imageId, Bitmap imageData)
        {
            MeterImageModel meterImage = new MeterImageModel();
            meterImage.RequestReadingUnit = readingUnit;
            meterImage.ImageId = imageId;
            meterImage.ImageSize = imageData.ByteCount.ToString();
            meterImage.ImageData = imageData;
            meterImageList.Add(meterImage);
        }
    }
}

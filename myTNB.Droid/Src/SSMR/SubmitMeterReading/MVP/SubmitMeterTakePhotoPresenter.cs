using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using Newtonsoft.Json;
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
            mView.ShowOCRLoading();
            List<Task<GetMeterReadingOCRResponse>> ocrSubmitTasks = new List<Task<GetMeterReadingOCRResponse>>();
            foreach (MeterImageModel meterImageModel in meterImageList)
            {
                if (meterImageModel.ImageData != null)
                {
                    MeterImage meterImage = new MeterImage();
                    meterImage.RequestReadingUnit = meterImageModel.RequestReadingUnit;
                    meterImage.ImageId = meterImageModel.ImageId;
                    meterImage.ImageSize = meterImageModel.ImageData.ByteCount.ToString();
                    meterImage.ImageData = Utils.ImageUtils.GetBase64FromBitmap(meterImageModel.ImageData);

                    request = new GetMeterReadingOCRValueRequest(contractAccount, meterImage);
                    ocrSubmitTasks.Add(api.GetMeterReadingOCRValue(request));
                }
            }

            var results = await Task.WhenAll(ocrSubmitTasks);
            //GetMeterReadingOCRResponse responseData = results[0];
            string resultResponse = JsonConvert.SerializeObject(results);
            mView.ShowMeterReadingPage(resultResponse);
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

        public void RemoveMeterImageAt(int position)
        {
            MeterImageModel meterImage = meterImageList[position];
            meterImage.ImageData = null;
        }

        public void InitializeModelList()
        {
            MeterImageModel model;
            for (int i=0; i < 3; i++)
            {
                model = new MeterImageModel();
                meterImageList.Add(model);
            }
        }

        public void AddMeterImageAt(int imagePosition, string readingUnit, string imageId, Bitmap imageBitmap)
        {
            meterImageList[imagePosition].RequestReadingUnit = readingUnit;
            meterImageList[imagePosition].ImageId = imageId;
            meterImageList[imagePosition].ImageSize = imageBitmap.ByteCount.ToString();
            meterImageList[imagePosition].ImageData = imageBitmap;
        }
    }
}

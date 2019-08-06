﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRValueRequest;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingPresenter : SubmitMeterReadingContract.IPresenter
    {
        SubmitMeterReadingContract.IView mView;
        SubmitMeterReadingImpl api;
        public SubmitMeterReadingPresenter(SubmitMeterReadingContract.IView view)
        {
            this.mView = view;
            api = new SubmitMeterReadingImpl();
        }

        public async void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList)
        {
            SubmitMeterReadingRequest request = new SubmitMeterReadingRequest(contractAccountValue, isOwnedAccountValue, meterReadingList);
            SubmitMeterReadingResponse response = await api.SubmitSMRMeetingReading(request);
            if (response.Data == null)
            {

            }
            else
            {

            }
        }

        
        public void EvaluateOCRReadingResponse(string jsonResponseList)
        {
            List<GetMeterReadingOCRResponse> ocrResponseList = JsonConvert.DeserializeObject<List<GetMeterReadingOCRResponse>>(jsonResponseList);
            List<GetMeterReadingOCRResponseDetails> smrRegisterDetailList = new List<GetMeterReadingOCRResponseDetails>();
            GetMeterReadingOCRResponseDetails details;
            foreach (GetMeterReadingOCRResponse ocrReadingResponse in ocrResponseList)
            {
                if (ocrReadingResponse.Data.ErrorCode == "7200" && ocrReadingResponse.Data.ResponseDetailsData != null)
                {
                    details = new GetMeterReadingOCRResponseDetails();
                    details.IsSuccess = ocrReadingResponse.Data.ResponseDetailsData.IsSuccess;
                    details.OCRUnit = ocrReadingResponse.Data.ResponseDetailsData.OCRUnit;
                    details.OCRValue = ocrReadingResponse.Data.ResponseDetailsData.OCRValue;
                    smrRegisterDetailList.Add(details);
                }
                else
                {
                    details = new GetMeterReadingOCRResponseDetails();
                    details.IsSuccess = "false";
                    details.OCRUnit = ocrReadingResponse.Data.ResponseDetailsData.OCRUnit;
                    details.OCRValue = ocrReadingResponse.Data.ResponseDetailsData.OCRValue;
                    smrRegisterDetailList.Add(details);
                }
            }
            mView.UpdateCurrentMeterReading(smrRegisterDetailList);
        }

        public async Task OnGetThreePhaseData()
        {
            try
            {
                this.mView.ShowProgressDialog();
                SSMRMeterReadingThreePhaseScreensEntity entity = new SSMRMeterReadingThreePhaseScreensEntity();
                List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
                List<SSMRMeterReadingThreePhaseScreensEntity> dbList = entity.GetAllItems();
                if (dbList.Count > 0)
                {
                    foreach (SSMRMeterReadingThreePhaseScreensEntity model in dbList)
                    {
                        SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                        dataModel.Image = model.Image;
                        dataModel.Title = model.Title;
                        dataModel.Description = model.Description;
                        Bitmap imageBitmap = null;
                        imageBitmap = await GetOnboardingPhoto(model.Image);
                        if (imageBitmap != null)
                        {
                            dataModel.ImageBitmap = imageBitmap;
                        }
                        items.Add(dataModel);
                    }
                }
                else
                {
                    items.AddRange(OnGetLocalThreePhaseData());
                }
                this.mView.OnUpdateThreePhaseTooltipData(items);
            }
            catch (Exception e)
            {
                List<SSMRMeterReadingModel> localItems = new List<SSMRMeterReadingModel>();
                localItems.AddRange(OnGetLocalThreePhaseData());
                this.mView.OnUpdateThreePhaseTooltipData(localItems);
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task OnGetOnePhaseData()
        {
            try
            {
                this.mView.ShowProgressDialog();
                SSMRMeterReadingScreensEntity entity = new SSMRMeterReadingScreensEntity();
                List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
                List<SSMRMeterReadingScreensEntity> dbList = entity.GetAllItems();
                if (dbList.Count > 0)
                {
                    foreach (SSMRMeterReadingScreensEntity model in dbList)
                    {
                        SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                        dataModel.Image = model.Image;
                        dataModel.Title = model.Title;
                        dataModel.Description = model.Description;
                        Bitmap imageBitmap = null;
                        imageBitmap = await GetOnboardingPhoto(model.Image);
                        if (imageBitmap != null)
                        {
                            dataModel.ImageBitmap = imageBitmap;
                        }
                        items.Add(dataModel);
                    }
                }
                else
                {
                    items.AddRange(OnGetLocalOnePhaseData());
                }
                this.mView.OnUpdateOnePhaseTooltipData(items);
            }
            catch (Exception e)
            {
                List<SSMRMeterReadingModel> localItems = new List<SSMRMeterReadingModel>();
                localItems.AddRange(OnGetLocalOnePhaseData());
                this.mView.OnUpdateOnePhaseTooltipData(localItems);
                Utility.LoggingNonFatalError(e);
            }
        }

        private async Task<Bitmap> GetOnboardingPhoto(string imageUrl)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap imageBitmap = null;
            try
            {
                await Task.Run(() =>
                {
                    imageBitmap = ImageUtils.GetImageBitmapFromUrl(imageUrl);
                }, cts.Token);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return imageBitmap;
        }

        public List<SSMRMeterReadingModel> OnGetLocalThreePhaseData()
        {
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
            for (int i = 0; i < 3; i++)
            {
                SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                if (i == 0)
                {
                    dataModel.Image = "tooltip_bg_1";
                    dataModel.Title = "Alright, what do I need to read?";
                    dataModel.Description = "You'll need to read 3 reading values (kWh, kVARh, kW). Your meter will automatically flash one after the other.";
                }
                else if (i == 1)
                {
                    dataModel.Image = "tooltip_bg_2";
                    dataModel.Title = "But wait, how do I read my meter?";
                    dataModel.Description = "You can enter each reading manually or just snap/upload a photo, and we’ll do the reading for you.";
                }
                else
                {
                    dataModel.Image = "tooltip_bg_3";
                    dataModel.Title = "How do I enter these values?";
                    dataModel.Description = "Enter the numbers according to its unit in the input. You’ll see your previous month's reading as a reference.";
                }
                items.Add(dataModel);
            }
            return items;
        }

        private List<SSMRMeterReadingModel> OnGetLocalOnePhaseData()
        {
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
            for (int i = 0; i < 2; i++)
            {
                SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                if (i == 0)
                {
                    dataModel.Image = "tooltip_bg_2";
                    dataModel.Title = "Alright, what do I need to read?";
                    dataModel.Description = "Your meter will display the kWh reading by default. Enter the reading manually or just snap/upload a photo, and we’ll do the reading for you.";
                }
                else if (i == 1)
                {
                    dataModel.Image = "tooltip_bg_3";
                    dataModel.Title = "How do I enter the value?";
                    dataModel.Description = "For manual reading, enter the kWh numbers in the input. You’ll see your previous month's reading as a reference.";
                }
                items.Add(dataModel);
            }
            return items;
        }
    }
}

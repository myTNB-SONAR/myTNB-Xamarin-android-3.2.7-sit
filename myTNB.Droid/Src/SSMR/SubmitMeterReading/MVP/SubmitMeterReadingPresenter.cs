using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using myTNB.Mobile.Business;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.NewAppTutorial.MVP;
using myTNB.Android.Src.SSMR.SSMRBase.MVP;
using myTNB.Android.Src.SSMR.SubmitMeterReading.Api;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;
using static myTNB.Android.Src.AppLaunch.Models.MasterDataResponse;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRValueRequest;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingPresenter : SubmitMeterReadingContract.IPresenter
    {
        SubmitMeterReadingContract.IView mView;
        SubmitMeterReadingImpl api;
        List<MeterReading> meterReadingList = new List<MeterReading>();
        public SubmitMeterReadingPresenter(SubmitMeterReadingContract.IView view)
        {
            this.mView = view;
            api = new SubmitMeterReadingImpl();
        }

        public async void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList)
        {
            try
            {
                SubmitMeterReadingRequest request = new SubmitMeterReadingRequest(contractAccountValue, isOwnedAccountValue, meterReadingList);
                //Mock - START
                //SubmitMeterReadingResponse mockResponse = new SubmitMeterReadingResponse();
                //List<SubmitSMRMeterReadingsResp> SubmitSMRMeterReadingsRespList = new List<SubmitSMRMeterReadingsResp>();
                //SubmitSMRMeterReadingsResp resp = new SubmitSMRMeterReadingsResp();
                //resp.MessageID = "24";
                //resp.ReadingUnit = "KWH";
                //resp.Message = "Your meter reading could not be validated. Please try again.";
                //resp.IsSuccess = false;
                //SubmitSMRMeterReadingsRespList.Add(resp);

                //resp = new SubmitSMRMeterReadingsResp();
                //resp.MessageID = "24";
                //resp.ReadingUnit = "KW";
                //resp.Message = "Your meter reading could not be validated. Please try again.";
                //resp.IsSuccess = true;
                //SubmitSMRMeterReadingsRespList.Add(resp);

                //resp = new SubmitSMRMeterReadingsResp();
                //resp.MessageID = "24";
                //resp.ReadingUnit = "KVAR";
                //resp.Message = "Your meter reading could not be validated. Please try again.";
                //resp.IsSuccess = false;
                //SubmitSMRMeterReadingsRespList.Add(resp);
                //mockResponse.Data = new SMRSubmitResponseData();
                //mockResponse.Data.ErrorCode = "7100";
                //mockResponse.Data.DisplayTitle = "Reading Submitted Test";
                //mockResponse.Data.DisplayMessage = "Thank you for your meter reading submission. We will notify you when your meter reading has been validated.";
                //mockResponse.Data.ResponseDetailsData = new SMRSubmitResponseDetails();
                //mockResponse.Data.ResponseDetailsData.SubmitSMRMeterReadingsResp = SubmitSMRMeterReadingsRespList;
                ////Mock - END
                EncryptedRequest encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
                SubmitMeterReadingResponse response = await api.SubmitSMRMeetingReading(encryptedRequest);
                if (response.Data != null && response.Data.ErrorCode == "7200")
                {
                    this.mView.OnRequestSuccessful(response.Data);
                }
                else if (response.Data != null && response.Data.ErrorCode == "7100")
                {
                    List<MeterValidationData> meterValidationDataList = new List<MeterValidationData>();
                    if (response.Data.ResponseDetailsData != null)
                    {
                        foreach (SubmitSMRMeterReadingsResp meterReadingResp in response.Data.ResponseDetailsData.SubmitSMRMeterReadingsResp)
                        {
                            MeterValidationData validationData = new MeterValidationData();
                            validationData.messageId = meterReadingResp.MessageID;
                            validationData.message = meterReadingResp.Message;
                            validationData.registerNumber = meterReadingResp.RegisterNumber;
                            validationData.isSuccess = meterReadingResp.IsSuccess;
                            validationData.meterReadingUnit = meterReadingResp.ReadingUnit;
                            meterValidationDataList.Add(validationData);
                        }
                    }
                    this.mView.ShowMeterCardValidationError(meterValidationDataList);
                }
                else
                {
                    this.mView.OnRequestFailed(response.Data);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public void EvaluateOCRReadingResponse(string jsonResponseList)
        {
            List<GetMeterReadingOCRResponse> ocrResponseList = JsonConvert.DeserializeObject<List<GetMeterReadingOCRResponse>>(jsonResponseList);
            List<GetMeterReadingOCRResponseDetails> smrRegisterDetailList = new List<GetMeterReadingOCRResponseDetails>();
            GetMeterReadingOCRResponseDetails details;
            string errorMessage = "";
            foreach (GetMeterReadingOCRResponse ocrReadingResponse in ocrResponseList)
            {
                if (ocrReadingResponse.Data.ErrorCode == "7200" && ocrReadingResponse.Data.ResponseDetailsData != null)
                {
                    details = new GetMeterReadingOCRResponseDetails();
                    details.IsSuccess = ocrReadingResponse.Data.ResponseDetailsData.IsSuccess;
                    details.OCRUnit = ocrReadingResponse.Data.ResponseDetailsData.OCRUnit;
                    details.OCRValue = ocrReadingResponse.Data.ResponseDetailsData.OCRValue;
                    smrRegisterDetailList.Add(details);
                    mView.UpdateCurrentMeterReading(smrRegisterDetailList);
                }
                else
                {
                    details = new GetMeterReadingOCRResponseDetails();
                    details.IsSuccess = "false";
                    details.OCRUnit = ocrReadingResponse.Data.ResponseDetailsData.OCRUnit;
                    details.OCRValue = ocrReadingResponse.Data.ResponseDetailsData.OCRValue;
                    smrRegisterDetailList.Add(details);
                    mView.UpdateCurrentMeterReading(smrRegisterDetailList);
                    errorMessage = ocrReadingResponse.Data.ErrorMessage;
                }
            }
            this.mView.ShowMeterReadingOCRError(errorMessage);
        }

        public void AddMeterReading(string MroID, string RegisterNumber, string MeterReadingResult)
        {
            MeterReading newMeterReading = new MeterReading();
            newMeterReading.MroID = MroID;
            newMeterReading.RegisterNumber = RegisterNumber;
            newMeterReading.MeterReadingResult = MeterReadingResult;
            meterReadingList.Add(newMeterReading);
        }

        public async Task OnGetThreePhaseData()
        {
            try
            {
                this.mView.ShowProgressDialog();
                bool isOCRDisabled = false;
                bool smrAccountOCRDown = SMRPopUpUtils.OnGetIsOCRDownFlag();
                if (MyTNBAccountManagement.GetInstance().IsOCRDown() || smrAccountOCRDown)
                {
                    isOCRDisabled = true;
                }

                if (isOCRDisabled)
                {
                    SSMRMeterReadingThreePhaseScreensOCROffEntity entity = new SSMRMeterReadingThreePhaseScreensOCROffEntity();
                    List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
                    List<SSMRMeterReadingThreePhaseScreensOCROffEntity> dbList = entity.GetAllItems();
                    if (dbList.Count > 0)
                    {
                        foreach (SSMRMeterReadingThreePhaseScreensOCROffEntity model in dbList)
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
                        this.mView.OnUpdateThreePhaseTooltipData(items);
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                    }
                }
                else
                {
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
                        this.mView.OnUpdateThreePhaseTooltipData(items);
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                    }
                }
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }

        public async Task OnGetOnePhaseData()
        {
            try
            {
                this.mView.ShowProgressDialog();
                bool isOCRDisabled = false;
                bool smrAccountOCRDown = SMRPopUpUtils.OnGetIsOCRDownFlag();
                if (MyTNBAccountManagement.GetInstance().IsOCRDown() || smrAccountOCRDown)
                {
                    isOCRDisabled = true;
                }

                if (isOCRDisabled)
                {
                    SSMRMeterReadingScreensOCROffEntity entity = new SSMRMeterReadingScreensOCROffEntity();
                    List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
                    List<SSMRMeterReadingScreensOCROffEntity> dbList = entity.GetAllItems();
                    if (dbList.Count > 0)
                    {
                        foreach (SSMRMeterReadingScreensOCROffEntity model in dbList)
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

                        this.mView.OnUpdateOnePhaseTooltipData(items);
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                    }
                }
                else
                {
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

                        this.mView.OnUpdateOnePhaseTooltipData(items);
                    }
                    else
                    {
                        this.mView.HideProgressDialog();
                    }
                }
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
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

        public List<MeterReadingModel> GetDummyData()
        {
            List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList = new List<SMRMROValidateRegisterDetails>();
            SMRMROValidateRegisterDetails sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "001";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            sMRMROValidateRegisterDetails.ReadingUnit = "KWH";
            sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle = "kWh";
            sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            sMRMROValidateRegisterDetails.RegisterNumber = "002";
            sMRMROValidateRegisterDetails.MroID = "0000002432432";
            sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            sMRMROValidateRegisterDetails.ReadingUnit = "KW";
            sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle = "kW";
            sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            //sMRMROValidateRegisterDetails = new SMRMROValidateRegisterDetails();
            //sMRMROValidateRegisterDetails.RegisterNumber = "003";
            //sMRMROValidateRegisterDetails.MroID = "0000002432432";
            //sMRMROValidateRegisterDetails.PrevMrDate = "2-8-2019";
            //sMRMROValidateRegisterDetails.SchMrDate = "2-8-2019";
            //sMRMROValidateRegisterDetails.PrevMeterReading = "1234567";
            //sMRMROValidateRegisterDetails.ReadingUnit = "KVAR";
            //sMRMROValidateRegisterDetails.ReadingUnitDisplayTitle = "kVARh";
            //sMRMROValidateRegisterDetailsList.Add(sMRMROValidateRegisterDetails);

            return GetMeterReadingModelList(sMRMROValidateRegisterDetailsList);
        }

        public List<MeterReadingModel> GetMeterReadingModelList(List<SMRMROValidateRegisterDetails> sMRMROValidateRegisterDetailsList)
        {
            List<MeterReadingModel> meterReadingModelList = new List<MeterReadingModel>();
            MeterReadingModel meterReadingModel;
            sMRMROValidateRegisterDetailsList.ForEach(validatedRegister =>
            {
                meterReadingModel = new MeterReadingModel();
                meterReadingModel.mroID = validatedRegister.MroID;
                meterReadingModel.registerNumber = validatedRegister.RegisterNumber;
                meterReadingModel.previousMeterReadingValue = validatedRegister.PrevMeterReading;
                meterReadingModel.currentMeterReadingValue = "";
                meterReadingModel.isValidated = false;
                meterReadingModel.meterReadingUnit = validatedRegister.ReadingUnit;
                meterReadingModel.meterReadingUnitDisplay = validatedRegister.ReadingUnitDisplayTitle;
                meterReadingModelList.Add(meterReadingModel);
            });
            return meterReadingModelList;
        }

        public List<NewAppModel> OnGeneraNewAppTutorialList()
        {
            List<NewAppModel> newList = new List<NewAppModel>();

            bool isOCRDown = false;

            bool smrAccountOCRDown = SMRPopUpUtils.OnGetIsOCRDownFlag();
            if (MyTNBAccountManagement.GetInstance().IsOCRDown() || smrAccountOCRDown)
            {
                isOCRDown = true;
            }

            newList.Add(new NewAppModel()
            {
                ContentShowPosition = isOCRDown ? ContentType.BottomLeft : ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("SSMRSubmitMeterReading", "tutorialReadMeterTitle"),
                ContentMessage = Utility.GetLocalizedLabel("SSMRSubmitMeterReading", "tutorialReadMeterDesc"),
                ItemCount = 0,
                DisplayMode = isOCRDown ? "DOWN" : "UP",
                IsButtonShow = false
            });

            return newList;
        }
    }
}

using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SSMRBase.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using Newtonsoft.Json;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRValueRequest;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
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
            SubmitMeterReadingRequest request = new SubmitMeterReadingRequest(contractAccountValue, isOwnedAccountValue, meterReadingList);
            //SubmitMeterReadingResponse response = await api.SubmitSMRMeetingReading(request);
            //STUB - START
            SubmitMeterReadingResponse response = new SubmitMeterReadingResponse();
            SMRSubmitResponseData data = new SMRSubmitResponseData();
            data.ErrorCode = "7201";
            data.DisplayTitle = "Reading Submitted";
            data.DisplayMessage = "Thank you for your meter reading submission. We will notify you when your meter reading has been validated.";
            response.Data = data;
            //STUB - END

            if(response.Data != null && response.Data.ErrorCode == "7200")
            {
                this.mView.OnRequestSuccessful(response.Data);
            }
            else
            {
                this.mView.OnRequestFailed(response.Data);
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
    }
}

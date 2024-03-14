using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.NewAppTutorial.MVP;
using myTNB.Android.Src.SSMR.SSMRBase.MVP;
using myTNB.Android.Src.SSMR.SubmitMeterReading.Api;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;
using static myTNB.Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingContract
    {
        public interface IView
        {
            void UpdateCurrentMeterReading(List<GetMeterReadingOCRResponseDetails> ocrMeterReadingList);
            void ShowMeterReadingOCRError(string errorMessage);

            void OnRequestSuccessful(SMRSubmitResponseData response);
            void OnRequestFailed(SMRSubmitResponseData response);
            void ShowMeterCardValidationError(List<MeterValidationData> validationDataList);

            void OnUpdateThreePhaseTooltipData(List<SSMRMeterReadingModel> list);

            void OnUpdateOnePhaseTooltipData(List<SSMRMeterReadingModel> list);

            void ShowProgressDialog();

            void HideProgressDialog();

            void OnUpdateSubmitMeterButton();

            void ClearMeterCardValidationError(METER_READING_TYPE mType);
        }

        public interface IPresenter
        {
            void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList);

            Task OnGetOnePhaseData();

            Task OnGetThreePhaseData();

            List<NewAppModel> OnGeneraNewAppTutorialList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SSMRBase.MVP;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingResponse;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingContract
    {
        public interface IView
        {
            void UpdateCurrentMeterReading(List<GetMeterReadingOCRResponseDetails> ocrMeterReadingList);
            void ShowMeterReadingOCRError(string errorMessage);

            void OnRequestSuccessful(SMRSubmitResponseData response);
            void OnRequestFailed(SMRSubmitResponseData response);

            void OnUpdateThreePhaseTooltipData(List<SSMRMeterReadingModel> list);

            void OnUpdateOnePhaseTooltipData(List<SSMRMeterReadingModel> list);

            void ShowProgressDialog();

            void HideProgressDialog();
        }

        public interface IPresenter
        {
            void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList);

            Task OnGetOnePhaseData();

            Task OnGetThreePhaseData();
        }
    }
}

using System;
using System.Collections.Generic;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.GetMeterReadingOCRResponse;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingContract
    {
        public interface IView
        {
            void UpdateCurrentMeterReading(List<GetMeterReadingOCRResponseDetails> ocrMeterReadingList);
        }

        public interface IPresenter
        {
            void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList);
        }
    }
}

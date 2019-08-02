using System;
using System.Collections.Generic;
using static myTNB_Android.Src.SSMR.SubmitMeterReading.Api.SubmitMeterReadingRequest;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class SubmitMeterReadingContract
    {
        public interface IView
        {

        }

        public interface IPresenter
        {
            void SubmitMeterReading(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList);
        }
    }
}

using System;
using System.Collections.Generic;
using myTNB_Android.Src.SSMR.SubmitMeterReading.Api;
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
    }
}

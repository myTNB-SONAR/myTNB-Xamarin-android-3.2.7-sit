using System;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.Api;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.Api
{
    public class SubmitMeterReadingRequest : BaseRequest
    {
        public string contractAccount;
        public string isOwnedAccount;
        public List<MeterReading> meterReadings;
        public SubmitMeterReadingRequest(string contractAccountValue, bool isOwnedAccountValue, List<MeterReading> meterReadingList)
        {
            contractAccount = contractAccountValue;
            isOwnedAccount = isOwnedAccountValue ? "true" : "false";
            meterReadings = meterReadingList;
        }

        public class MeterReading
        {
            public string MroID { set; get; }
            public string RegisterNumber { set; get; }
            public string MeterReadingResult { set; get; }
            public string Channel { set; get; }
            public string MeterReadingDate { set; get; }
            public string MeterReadingTime { set; get; }
        }
    }
}

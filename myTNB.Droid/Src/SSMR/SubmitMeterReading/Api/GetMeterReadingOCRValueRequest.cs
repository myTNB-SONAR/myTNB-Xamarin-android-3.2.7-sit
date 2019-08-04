using myTNB_Android.Src.SSMR.SMRApplication.Api;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Api
{
    public class GetMeterReadingOCRValueRequest : BaseRequest
    {
        public string contractAccount;
        public MeterImage meterImage;
        public GetMeterReadingOCRValueRequest(string contractAccountValue, MeterImage meterImageValue)
        {
            contractAccount = contractAccountValue;
            meterImage = meterImageValue;
        }

        public class MeterImage
        {
            public string RequestReadingUnit { set; get; }

            public string ImageId { set; get; }

            public string ImageSize { set; get; }

            public string ImageData { set; get; }
        }
    }
}

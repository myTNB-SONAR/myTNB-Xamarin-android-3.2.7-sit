using myTNB.Android.Src.SSMR.SMRApplication.Api;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.Api
{
    public class GetMeterReadingOCRValueRequest : BaseRequest
    {
        public string contractAccount = "";
        public MeterImage meterImage = new MeterImage();
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

namespace myTNB.Model
{
    public class GetOCRReadingResponseModel
    {
        public GetOCRReadingDataModel d { set; get; }
    }

    public class GetOCRReadingDataModel : BaseModelV2
    {
        public GetOCRReadingModel data { set; get; }
    }

    public class GetOCRReadingModel
    {
        public string RequestReadingUnit { set; get; }
        public string ImageId { set; get; }
        public string OCRValue { set; get; }
        public string OCRUnit { set; get; }
        public bool IsSuccess { set; get; } = true;
    }
}
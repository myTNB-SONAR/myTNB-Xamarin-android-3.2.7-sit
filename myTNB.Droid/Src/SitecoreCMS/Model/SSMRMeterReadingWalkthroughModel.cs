using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class SSMRMeterReadingTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<SSMRMeterReadingTimeStamp> Data { set; get; }
    }

    public class SSMRMeterReadingResponseModel
    {
        public string Status { set; get; }
        public List<SSMRMeterReadingModel> Data { set; get; }
    }

    public class SSMRMeterReadingModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ImageBitmap { set; get; }

        public string ID { set; get; }
    }

    public class SSMRMeterReadingTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
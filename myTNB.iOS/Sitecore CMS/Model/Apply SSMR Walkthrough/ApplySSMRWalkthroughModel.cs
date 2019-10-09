using System.Collections.Generic;
using Foundation;

namespace myTNB.SitecoreCMS.Model
{
    public class ApplySSMRTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<ApplySSMRTimeStamp> Data { set; get; }
    }

    public class ApplySSMRResponseModel
    {
        public string Status { set; get; }
        public List<ApplySSMRModel> Data { set; get; }
    }

    public class ApplySSMRModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public NSData NSDataImage { set; get; }

        public string ID { set; get; }
    }

    public class ApplySSMRTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
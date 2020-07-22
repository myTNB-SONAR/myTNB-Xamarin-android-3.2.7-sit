using System.Collections.Generic;
using Android.Graphics;


namespace myTNB.SitecoreCMS.Model
{


    public class HowDoesCopyOfIdentificationTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<HowDoesCopyOfIdentificationTimeStamp> Data { set; get; }
    }

    public class HowDoesCopyOfIdentificationpResponseModel
    {
        public string Status { set; get; }
        public List<HowDoesCopyOfIdentificationModel> Data { set; get; }
    }

    public class HowDoesCopyOfIdentificationModel
    {

        public string Title { set; get; }
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string ImageBase64 { set; get; }
        public string Image { set; get; }
        public string ID { get; set; }
    }

    public class HowDoesCopyOfIdentificationTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
using System.Collections.Generic;
using Android.Graphics;


namespace myTNB.SitecoreCMS.Model
{
   
    public class HowDoesProofOfConsentTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<HowDoesProofOfConsentimeStamp> Data { set; get; }
    }

    public class HowDoesProofOfConsentResponseModel
    {
        public string Status { set; get; }
        public List<HowDoesProofOfConsentModel> Data { set; get; }
    }

    public class HowDoesProofOfConsentModel
    {

        public string Title { set; get; }
        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string ImageBase64 { set; get; }
        public string Image { set; get; }
        public string ID { get; set; }
    }

    public class HowDoesProofOfConsentimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
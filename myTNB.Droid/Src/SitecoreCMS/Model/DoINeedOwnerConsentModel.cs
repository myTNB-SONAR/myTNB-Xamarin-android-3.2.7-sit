using System.Collections.Generic;
using Android.Graphics;


namespace myTNB.SitecoreCMS.Model
{


    public class DoINeedOwnerConsentTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<DoINeedOwnerConsentTimeStamp> Data { set; get; }
    }

    public class DoINeedOwnerConsentOwnerResponseModel
    {
        public string Status { set; get; }
        public List<DoINeedOwnerConsentModel> Data { set; get; }
    }

    public class DoINeedOwnerConsentModel
    {

        public string PopUpTitle { set; get; }
        public string PopUpBody { set; get; }
        public string ID { get; set; }
    }

    public class DoINeedOwnerConsentTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
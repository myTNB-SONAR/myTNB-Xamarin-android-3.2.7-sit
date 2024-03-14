using System.Collections.Generic;
using Android.Graphics;
using myTNB.Mobile;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SitecoreCMS.Model
{

    public class FloatingButtonTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<FloatingButtonTimeStamp> Data { set; get; }
    }

    public class FloatingButtonResponseModel
    {
        public string Status { set; get; }
        public List<FloatingButtonModel> Data { set; get; }
    }

    public class FloatingButtonModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ImageB64 { set; get; }
        public string StartDateTime { set; get; }
        public string EndDateTime { set; get; }
        public string ShowForSeconds { set; get; }

        public Bitmap ImageBitmap { set; get; }

        public string ID { set; get; }
    }

    public class FloatingButtonTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

    public enum Module
    {
        WEB,
        DBR,
        BR,
        EB,
        SD,
        TNG
    }

}


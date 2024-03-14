using System.Collections.Generic;
using Android.Graphics;
using myTNB.Mobile;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.SitecoreCMS.Model
{

    public class FloatingButtonMarketingTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<FloatingButtonMarketingTimeStamp> Data { set; get; }
    }

    public class FloatingButtonMarketingResponseModel
    {
        public string Status { set; get; }
        public List<FloatingButtonMarketingModel> Data { set; get; }
    }

    public class FloatingButtonMarketingModel
    {
        public string Title { set; get; }
        public string ButtonTitle { set; get; }
        public string Description { set; get; }
        public string Description_Images { set; get; }
        public List<FBMarketingDetailImageDBModel> Description_Images_List { set; get; }
        public string Infographic_FullView_URL { set; get; }
        public string Infographic_FullView_URL_ImageB64 { set; get; }
        //public string Image { set; get; }
        //public string ImageB64 { set; get; }

        //public Bitmap ImageBitmap { set; get; }

        public string ID { set; get; }
    }

    public class FBMarketingDetailImageModel
    {
        public string ExtractedImageTag { set; get; }
        public string ExtractedImageUrl { set; get; }
        public Bitmap ExtractedImageBitmap { set; get; }
    }

    public class FBMarketingDetailImageDBModel
    {
        public string ExtractedImageTag { set; get; }
        public string ExtractedImageUrl { set; get; }
        public string ExtractedImageB64 { set; get; }
    }

    public class FloatingButtonMarketingTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

   
}


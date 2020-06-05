using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.SitecoreCMS.Model
{
    /// <summary>
    /// /syahmi sto add this function
    /// </summary>
    public class EppToolTipTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<EppToolTipTimeStamp> Data { set; get; }
    }

    public class EppToolTipResponseModel
    {
        public string Status { set; get; }
        public List<EppToolTipModel> Data { set; get; }
    }

    public class EppToolTipModel
    {

        public string Title { set; get; }
        public string PopUpTitle { set; get; }

        public string PopUpBody { set; get; }
        // public Bitmap ImageBitmap { set; get; }
        public string Image { set; get; }
        public string ID { get; set; }
    }

    public class EppToolTipTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
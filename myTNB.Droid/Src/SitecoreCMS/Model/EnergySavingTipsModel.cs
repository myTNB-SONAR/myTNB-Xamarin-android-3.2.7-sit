using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.SitecoreCMS.Model
{
    public class EnergySavingTipsTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<EnergySavingTipsTimeStamp> Data { set; get; }
    }

    public class EnergySavingTipsResponseModel
    {
        public string Status { set; get; }
        public List<EnergySavingTipsModel> Data { set; get; }
    }

    public class EnergySavingTipsModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public Bitmap ImageBitmap { set; get; }

        public string ID { set; get; }
    }

    public class EnergySavingTipsTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
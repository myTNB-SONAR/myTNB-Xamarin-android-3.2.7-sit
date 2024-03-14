using System.Collections.Generic;

namespace myTNB.Android.Src.SitecoreCMS.Model
{
    public class HelpTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<HelpTimeStamp> Data { set; get; }
    }

    public class HelpResponseModel
    {
        public string Status { set; get; }
        public List<HelpModel> Data { set; get; }
    }

    public class HelpModel
    {
        public string BGStartColor { set; get; }
        public string BGEndColor { set; get; }
        public string BGGradientDirection { set; get; }

        public string Title { set; get; }
        public string Description { set; get; }
        public string TopicBodyTitle { set; get; }
        public string TopicBodyContent { set; get; }
        public string TopicBGImage { set; get; }

        public string CTA { set; get; }
        public string Tags { set; get; }
        public string TargetItem { set; get; }

        public string ID { set; get; }
    }

    public class HelpTimeStamp
    {
        public string Timestamp { set; get; }
        public bool ShowNeedHelp { set; get; }
        public string ID { set; get; }
    }
}
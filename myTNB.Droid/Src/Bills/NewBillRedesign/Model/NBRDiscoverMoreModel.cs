using System.Collections.Generic;

namespace myTNB_Android.Src.Bills.NewBillRedesign.Model
{
    public class NBRDiscoverMoreModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Banner1 { get; set; }
        public string Banner2 { get; set; }
        public bool IsZoomable { get; set; }
        public string FooterMessage { get; set; }
        public bool ShouldTrackHeader { set; get; }
        public string DynatraceTagHeader { set; get; }
        public bool ShouldTrackFooter { set; get; }
        public string DynatraceTagFooter { set; get; }

        public List<DiscoverMoreItem> DiscoverMoreItemList { get; set; }
        public class DiscoverMoreItem
        {
            public string Title { get; set; }
            public string Banner { get; set; }
            public string Content { get; set; }
            public bool ShouldTrack { set; get; }
            public string DynatraceTag { set; get; }
        }
    }
}
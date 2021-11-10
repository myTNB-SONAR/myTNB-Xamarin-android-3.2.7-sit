using System.Collections.Generic;

namespace myTNB.Mobile
{
    public class GSLInfoModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public List<ExpandCollapseModel> ExpandCollapseList { set; get; }
    }

    public class ExpandCollapseModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
    }
}
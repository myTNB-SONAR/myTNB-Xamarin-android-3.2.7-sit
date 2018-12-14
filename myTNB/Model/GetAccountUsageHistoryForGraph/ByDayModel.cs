using System.Collections.Generic;

namespace myTNB.Model
{
    public class ByDayModel : ByRangeModelBase
    {
        public List<SegmentDetailsModel> Days { set; get; }
    }
}
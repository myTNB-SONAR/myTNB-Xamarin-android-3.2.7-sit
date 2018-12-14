using System.Collections.Generic;
namespace myTNB.Model
{
    public class ByMonthModel : ByRangeModelBase
    {
        public List<SegmentDetailsModel> Months { get; set; }
    }
}
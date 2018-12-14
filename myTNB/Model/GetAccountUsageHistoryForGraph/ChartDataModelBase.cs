using System.Collections.Generic;

namespace myTNB.Model
{
    public class ChartDataModelBase
    {
        public ChartDataModelBase()
        {
            ByDay = new List<ByDayModel>();
        }
        public List<ByDayModel> ByDay { get; set; }

        public string CustomMessage { get; set; }
    }
}

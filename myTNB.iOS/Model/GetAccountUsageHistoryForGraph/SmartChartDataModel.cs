
using System.Collections.Generic;

namespace myTNB.Model
{
    public class SmartChartDataModel : ChartDataModelBase
    {
        public SmartChartDataModel()
        {
            ByMonth = new List<ByMonthModel>();
        }
        public List<ByMonthModel> ByMonth { get; set; }
        public UsageMetrics OtherUsageMetrics { get; set; }

        private string IsCO2Disabled { get; set; }
        public bool IsEmissionDisabled
        {
            get
            {
                if (!string.IsNullOrEmpty(IsCO2Disabled))
                {
                    if (bool.TryParse(IsCO2Disabled, out bool res))
                    {
                        return res;
                    }
                }
                return true;
            }
        }

        public List<ToolTipsModel> ToolTips { set; get; }
    }
}

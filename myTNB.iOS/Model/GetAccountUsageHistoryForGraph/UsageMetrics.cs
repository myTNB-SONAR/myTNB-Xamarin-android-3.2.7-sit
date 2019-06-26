using System.Collections.Generic;

namespace myTNB.Model
{
    public class UsageMetrics
    {
        string _currentCycleStartDate = string.Empty;
        public RealTimeMetrics StatsByCost
        {
            get;
            set;
        }

        public RealTimeMetrics StatsByUsage
        {
            get;
            set;
        }

        public List<EmissionMetric> StatsByCo2
        {
            get;
            set;
        }

        public string CurrentCycleStartDate
        {
            set;
            get;
        }

        public string FromCycleDate
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentCycleStartDate)
                    || string.IsNullOrWhiteSpace(CurrentCycleStartDate))
                {
                    return null;
                }
                string formattedDate = DateHelper.GetFormattedDate(CurrentCycleStartDate, "dd MMM");
                return formattedDate;
            }
        }
    }
}

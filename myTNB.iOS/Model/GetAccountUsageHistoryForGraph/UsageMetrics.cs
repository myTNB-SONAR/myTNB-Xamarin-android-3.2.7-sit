using System.Collections.Generic;

namespace myTNB.Model
{
    public class UsageMetrics
    {
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
    }
}

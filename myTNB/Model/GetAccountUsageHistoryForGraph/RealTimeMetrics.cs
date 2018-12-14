using System;
namespace myTNB.Model
{
    public class RealTimeMetrics
    {
        public string AsOf
        {
            get;
            set;
        }

        string _currentCharges = string.Empty;
        public string CurrentCharges
        {
            get
            {
                return _currentCharges;
            }
            set
            {
                double parsedAmount = 0;
                if (double.TryParse(value, out parsedAmount))
                {
                    _currentCharges = value;
                }
                else
                {
                    _currentCharges = "0";
                }
            }

        }

        public string ProjectedCost
        {
            get;
            set;
        }

        string _currentUsageKWH = string.Empty;
        public string CurrentUsageKWH
        {
            get
            {
                return _currentUsageKWH;
            }
            set
            {
                double parsedAmount = 0;
                if (double.TryParse(value, out parsedAmount))
                {
                    _currentUsageKWH = value;
                }
                else
                {
                    _currentUsageKWH = "0";
                }
            }

        }

        public string UsageComparedToPrevious
        {
            get;
            set;
        }

    }
}

using System;
using System.Globalization;

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

                if (!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _currentCharges = parsedAmount.ToString(CultureInfo.InvariantCulture);
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
                if (!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _currentUsageKWH = parsedAmount.ToString(CultureInfo.InvariantCulture);
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

        public string ProjectedCostAsOf { set; get; }
    }
}

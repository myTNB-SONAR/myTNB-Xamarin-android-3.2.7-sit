using System.Globalization;

namespace myTNB.Model
{
    public class SegmentDetailsModel
    {
        string _amount = "0";
        string _consumption = "0";
        string _co2 = "0";
        string _usage = "0";
        string _isEstimatedReading = "false";

        public string Date { set; get; }
        public string Month { set; get; }
        public string Day { set; get; }
        public string Amount
        {
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _amount = parsedAmount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _amount = "0";
                }
            }
            get
            {
                return _amount ?? "0";
            }
        }
        public string Consumption
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _consumption = parsedAmount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _consumption = "0";
                }
            }
            get
            {
                return _consumption ?? "0";
            }
        }
        public string CO2
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _co2 = parsedAmount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _co2 = "0";
                }
            }
            get
            {
                return _co2 ?? "0";
            }
        }

        public string Usage
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    double parsedAmount = TextHelper.ParseStringToDouble(value);
                    _usage = parsedAmount.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _usage = "0";
                }
            }
            get
            {
                return _usage ?? "0";
            }
        }

        public string IsEstimatedReading 
        {
            set
            {
                if (bool.TryParse(value, out bool parsed))
                {
                    _isEstimatedReading = value;
                }
                else
                {
                    _isEstimatedReading = "false";
                }
            }
            get
            {
                return _isEstimatedReading ?? "false";
            }
        }
    }
}
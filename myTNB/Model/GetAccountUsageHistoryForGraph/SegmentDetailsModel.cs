namespace myTNB.Model
{
    public class SegmentDetailsModel
    {
        string _amount = "0";
        string _consumption = "0";
        string _co2 = "0";
        string _isEstimatedReading = "false";

        public string Date { set; get; }
        public string Month { set; get; }
        public string Day { set; get; }
        public string Amount
        {
            set
            {
                double parsedAmount = 0;
                if (double.TryParse(value, out parsedAmount))
                {
                    _amount = value;
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
                double parsedAmount = 0;
                if (double.TryParse(value, out parsedAmount))
                {
                    _consumption = value;
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
                double parsedAmount = 0;
                if (double.TryParse(value, out parsedAmount))
                {
                    _co2 = value;
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
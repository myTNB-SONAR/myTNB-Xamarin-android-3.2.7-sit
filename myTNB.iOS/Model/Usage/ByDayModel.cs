using System.Collections.Generic;

namespace myTNB.Model.Usage
{
    public class ByDayModel
    {
        public string Range { set; get; }
        public string CurrentCycle { set; get; }
        public List<DayItemModel> Days { set; get; }
    }

    public class DayItemModel
    {
        public string Year { set; get; }
        public string Date { set; get; }
        public string Month { set; get; }
        public string Day { set; get; }
        public string Consumption { set; get; }
        public string Amount { set; get; }
        public string CO2 { set; get; }
        public string IsEstimatedReading { set; get; }
        public List<TariffItemModel> tariffBlocks { set; get; }

        public bool IsMissingReading
        {
            get
            {
                if (!string.IsNullOrEmpty(IsEstimatedReading) && !string.IsNullOrWhiteSpace(IsEstimatedReading))
                {
                    return IsEstimatedReading.ToUpper() == "TRUE";
                }
                return false;
            }
        }
    }
}
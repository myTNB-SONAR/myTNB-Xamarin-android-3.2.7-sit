using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.SummaryDashBoard.Models
{
    public class SummaryDashBoardDetails
    {
        public SummaryDashBoardDetails()
        {



        }


        [JsonProperty("amountDue")]
        public string AmountDue { get; set; }

        [JsonProperty("billDueDate")]
        public string BillDueDate { get; set; }

        [JsonProperty("accNum")]
        public string AccNumber { get; set; }

        [JsonProperty("IncrementREDueDateByDays")]
        public string IncrementREDueDateByDays { get; set; }

        [JsonProperty("IsError")]
        public bool IsError { get; set; }

        public string AccType { get; set; }

        public string AccName { get; set; }

        public bool IsAccSelected { get; set; }

        public string SmartMeterCode { get; set; }
        public bool IsTaggedSMR { get; set; }
    }
}

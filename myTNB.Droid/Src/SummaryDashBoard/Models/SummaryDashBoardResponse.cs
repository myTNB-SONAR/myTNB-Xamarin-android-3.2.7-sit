using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.SummaryDashBoard.Models
{
    public class SummaryDashBoardResponse
    {
        public SummaryDashBoardResponse()
        {





        }



        [JsonProperty("d")]
        public SummaryDashBoardData Data { get; set; }

        public class SummaryDashBoardData
        {
            [JsonProperty("__type")]
            public string Type { get; set; }


            [JsonProperty("data")]
            public List<SummaryDashBoardDetails> data { get; set; }

            [JsonProperty("RefreshTitle")]
            public string RefreshTitle { get; set; }

            [JsonProperty("RefreshMessage")]
            public string RefreshMessage { get; set; }

            [JsonProperty("RefreshBtnText")]
            public string RefreshBtnText { get; set; }

            [JsonProperty("MandatoryChargesTitle")]
            public string MandatoryChargesTitle { get; set; }

            [JsonProperty("MandatoryChargesMessage")]
            public string MandatoryChargesMessage { get; set; }

            [JsonProperty("MandatoryChargesPriButtonText")]
            public string MandatoryChargesPriButtonText { get; set; }

            [JsonProperty("MandatoryChargesSecButtonText")]
            public string MandatoryChargesSecButtonText { get; set; }

            [JsonProperty("ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty("ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty("DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty("DisplayType")]
            public string DisplayType { get; set; }


            [JsonProperty("DisplayTitle")]
            public string DisplayTitle { get; set; }
        }
    }
}

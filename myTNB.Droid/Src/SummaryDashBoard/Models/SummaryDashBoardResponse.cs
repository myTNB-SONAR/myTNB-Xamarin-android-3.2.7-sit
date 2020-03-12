using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB_Android.Src.SummaryDashBoard.Models
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
        }
    }
}

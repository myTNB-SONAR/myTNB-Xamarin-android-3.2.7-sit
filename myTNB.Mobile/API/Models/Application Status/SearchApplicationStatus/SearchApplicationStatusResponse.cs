using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class SearchApplicationStatusResponse : BaseResponse<SearchApplicationStatusModel>
    {

    }

    public class SearchApplicationStatusModel
    {
        [JsonProperty("searchApplications")]
        public List<SearchDetails> SearchApplications { set; get; }
    }

    public class SearchDetails : Details
    {
        [JsonProperty("actionMessage")]
        public string ActionMessage { set; get; }
        [JsonProperty("isUpdated")]
        public bool IsUpdated { set; get; }
        [JsonProperty("note")]
        public string Note { set; get; }
        [JsonProperty("isProgressDisplayed")]
        public bool IsProgressDisplayed { set; get; }
        [JsonProperty("progress")]
        public List<Progress> Progress { set; get; }
    }
}
using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR.AutoOptIn
{
    public class GetAutoOptInCaResponse : BaseResponse<GetAutoOptInCaModel>
    {

    }

    public class GetAutoOptInCaModel
    {
        [JsonProperty("accountNumber")]
        public string accountNumber { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("userId")]
        public string userId { get; set; }
        [JsonProperty("isPopupSeen")]
        public bool isPopupSeen { get; set; }
        [JsonProperty("popupSeenDate")]
        public DateTime? popupSeenDate { get; set; }
        [JsonProperty("currentMonthCount")]
        public int currentMonthCount { get; set; }
        [JsonProperty("autoOptInDate")]
        public DateTime? autoOptInDate { get; set; }

    }
}

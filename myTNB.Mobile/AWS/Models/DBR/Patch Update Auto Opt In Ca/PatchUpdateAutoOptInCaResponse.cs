using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PatchUpdateAutoOptInCaResponse : BaseResponse<UpdateAutoOptInCaModel>
    {

    }

    public class UpdateAutoOptInCaModel
    {
        [JsonProperty("accountNumber")]
        public string AccountNumber { set; get; }
        [JsonProperty("email")]
        public string Email { set; get; }
        [JsonProperty("userId")]
        public string UserId { set; get; }
        [JsonProperty("isPopupSeen")]
        public string IsPopupSeen { set; get; }
        [JsonProperty("popupSeenDate")]
        public DateTime popupSeenDate { set; get; }
        [JsonProperty("currentMonthCount")]
        public int CurrentMonthCount { set; get; }
        [JsonProperty("autoOptInDate")]
        public DateTime AutoOptInDate { set; get; }
    }
}
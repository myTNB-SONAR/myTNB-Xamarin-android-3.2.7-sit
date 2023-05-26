using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class PostGetAutoOptInCaResponse : BaseResponse<GetAutoOptInCaModel>
    {

    }

    public class GetAutoOptInCaModel
    {
        [JsonProperty("accountNumber")]
        public string AccountNumber { set; get; }
        [JsonProperty("email")]
        public string Email { set; get; }
        [JsonProperty("userId")]
        public string UserId { set; get; }
        [JsonProperty("isPopupSeen")]
        public bool IsPopupSeen { set; get; }
        [JsonProperty("popupSeenDate")]
        public DateTime? PopupSeenDate { set; get; }
        [JsonProperty("currentMonthCount")]
        public int CurrentMonthCount { set; get; }
        [JsonProperty("autoOptInDate")]
        public DateTime? AutoOptInDate { set; get; }
    }
}
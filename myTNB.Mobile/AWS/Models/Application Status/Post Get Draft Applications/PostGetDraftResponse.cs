using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS
{
    public class PostGetDraftResponse : BaseResponse<PostGetDraftResponseModel>
    {

    }

    public class PostGetDraftResponseModel
    {
        [JsonProperty("applications")]
        public List<PostGetDraftResponseItemModel> Applications { set; get; }

        public string ReminderTitle { set; get; } = string.Empty;
        public string ReminderMessage { set; get; } = string.Empty;
        public string PrimaryCTA { set; get; } = string.Empty;
        public string SecondaryCTA { set; get; } = string.Empty;
        public bool IsMultipleDraft { set; get; }
        public List<string> ApplicationList { set; get; }
    }

    public class PostGetDraftResponseItemModel
    {
        [JsonProperty("applicationId")]
        public int ApplicationID { set; get; }
        [JsonProperty("referenceNo")]
        public string ReferenceNo { set; get; }
        [JsonProperty("system")]
        public string System { set; get; }
        [JsonProperty("applicationType")]
        public string ApplicationType { set; get; }
    }
}
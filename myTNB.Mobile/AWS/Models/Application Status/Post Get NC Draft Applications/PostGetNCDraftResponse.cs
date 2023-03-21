using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS
{
    public class PostGetNCDraftResponse : BaseResponse<PostGetNCDraftResponseModel>
    {

    }

    public class PostGetNCDraftResponseModel
    {
        [JsonProperty("applications")]
        public List<PostGetNCDraftResponseItemModel> Applications { set; get; }

        public string ReminderTitle { set; get; } = string.Empty;
        public string ReminderMessage { set; get; } = string.Empty;
        public string PrimaryCTA { set; get; } = string.Empty;
        public string SecondaryCTA { set; get; } = string.Empty;
        public List<string> NCApplicationList { set; get; }
    }

    public class PostGetNCDraftResponseItemModel
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
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.ApplicationStatus.PostNCDraftApplications
{
    public class PostGetNCDraftRequest
    {
        [JsonProperty("UserID")]
        public string UserID { set; get; }
        [JsonProperty("email")]
        public string Email { set; get; }
    }
}
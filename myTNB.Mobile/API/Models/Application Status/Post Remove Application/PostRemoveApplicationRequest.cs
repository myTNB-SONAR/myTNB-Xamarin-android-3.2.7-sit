using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.PostRemoveApplication
{
    internal class PostRemoveApplicationRequest
    {
        [JsonProperty("system")]
        public string System { set; get; }

        [JsonProperty("applicationType")]
        public string ApplicationType { set; get; }

        [JsonProperty("appId")]
        public string AppId { set; get; }
    }
}
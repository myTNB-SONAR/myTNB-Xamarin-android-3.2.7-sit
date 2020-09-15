using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication
{
    public class SaveApplicationRequest
    {
        [JsonProperty("saveApplication")]
        public SaveApplication SaveApplication { set; get; }
    }

    public class SaveApplication
    {
        public string ReferenceNo { set; get; } = string.Empty;
        public string ModuleName { set; get; } = string.Empty;
        public string SrNo { set; get; } = string.Empty;
        public string SrType { set; get; } = string.Empty;
        public string StatusCode { set; get; } = string.Empty;
        public string SrCreatedDate { set; get; } = string.Empty;
    }
}
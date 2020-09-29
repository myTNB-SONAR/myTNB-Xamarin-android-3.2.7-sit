using System;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.SaveApplication
{
    public class PostSaveApplicationRequest
    {
        [JsonProperty("saveApplication")]
        public PostSaveApplication SaveApplication { set; get; }

#pragma warning disable IDE1006 // Naming Styles
        public string lang { set; get; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class PostSaveApplication
    {
        public string ReferenceNo { set; get; } = string.Empty;
        public string ApplicationModuleId { set; get; } = string.Empty;
        public string ApplicationType { set; get; } = string.Empty;
        public string BackendReferenceNo { set; get; } = string.Empty;
        public string BackendApplicationType { set; get; } = string.Empty;
        public string BackendModule { set; get; } = string.Empty;
        public string StatusCode { set; get; } = string.Empty;
        public DateTime SrCreatedDate { set; get; }
    }
}
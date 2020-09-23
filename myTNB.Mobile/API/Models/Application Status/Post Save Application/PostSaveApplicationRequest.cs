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
        public string ModuleName { set; get; } = string.Empty;
        public string SrNo { set; get; } = string.Empty;
        public string SrType { set; get; } = string.Empty;
        public string StatusCode { set; get; } = string.Empty;
        public DateTime SrCreatedDate { set; get; }
    }
}
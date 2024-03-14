using Newtonsoft.Json;
using System.Collections.Generic;

namespace myTNB.Android.Src.Base.Models
{
    public class SubmittedFeedbackDetails
    {
        [JsonProperty("ServiceReqNo")]
        public string ServiceReqNo { get; set; }

        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("StateId")]
        public string StateId { get; set; }

        [JsonProperty("StateName")]
        public string StateName { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("PoleNum")]
        public string PoleNum { get; set; }

        [JsonProperty("FeedbackTypeId")]
        public string FeedbackTypeId { get; set; }

        [JsonProperty("FeedbackTypeName")]
        public string FeedbackTypeName { get; set; }

        [JsonProperty("IsOwner")]
        public bool? IsOwner { get; set; }

        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("FeedbackImage")]
        public List<ImageResponse> ImageList { get; set; }

        [JsonProperty("StatusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("StatusDesc")]
        public string StatusDesc { get; set; }

        [JsonProperty("ContactName")]
        public string ContactName { get; set; }

        [JsonProperty("ContactEmailAddress")]
        public string ContactEmailAddress { get; set; }

        [JsonProperty("ContactMobileNo")]
        public string ContactMobileNo { get; set; }

        [JsonProperty("RelationshipWithCA")]
        public int? RelationshipWithCA { get; set; }

        [JsonProperty("RelationshipWithCADesc")]
        public string RelationshipWithCADesc { get; set; }

        [JsonProperty("EnquiryName")]
        public string EnquiryName { get; set; }

        [JsonProperty("FeedbackUpdateDetails")]
        public List<FeedbackUpdate> FeedbackUpdateDetails { get; set; }

        [JsonProperty("IncidentInfos")]
        public List<Incident> IncidentInfos { get; set; }

        [JsonProperty("RebateId")]
        public string RebateId { get; set; }

        [JsonProperty("TenantFullName")]
        public string TenantFullName { get; set; }

        [JsonProperty("TenantEmail")]
        public string TenantEmail { get; set; }

        [JsonProperty("TenantMobileNumber")]
        public string TenantMobileNumber { get; set; }

        public class ImageResponse
        {
            [JsonProperty("imageHex")]
            public string ImageHex { get; set; }

            [JsonProperty("fileHex")]
            public string FileHex { get; set; }

            [JsonProperty("fileName")]
            public string FileName { get; set; }

            [JsonProperty("fileSize")]
            public int FileSize { get; set; }
        }

        public class FeedbackUpdate
        {
            [JsonProperty("FeedbackUpdInfoType")]
            public int FeedbackUpdInfoType { get; set; }

            [JsonProperty("FeedbackUpdInfoTypeDesc")]
            public string FeedbackUpdInfoTypeDesc { get; set; }

            [JsonProperty("FeedbackUpdInfoValue")]
            public string FeedbackUpdInfoValue { get; set; }
        }

        public class Incident
        {
            [JsonProperty("IncidentDate")]
            public string IncidentDate { get; set; }

            [JsonProperty("IncidentStartTime")]
            public string IncidentStartTime { get; set; }

            [JsonProperty("RestoreDate")]
            public string RestoreDate { get; set; }

            [JsonProperty("RestoreTime")]
            public string RestoreTime { get; set; }
        }
    }
}
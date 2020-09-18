using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class AllApplicationsResponse : BaseResponse<AllApplicationsModel>
    {

    }

    public class AllApplicationsModel
    {
        [JsonProperty("applications")]
        public List<Application> Applications { set; get; }

        [JsonProperty("currentPage")]
        public int CurrentPage { set; get; }

        [JsonProperty("total")]
        public int Total { set; get; }

        [JsonProperty("previousPage")]
        public string PreviousPage { set; get; }

        [JsonProperty("nextPage")]
        public string NextPage { set; get; }
    }

    public class Application
    {
        [JsonProperty("savedApplicationId")]
        public string SavedApplicationId { set; get; }

        [JsonProperty("applicationId")]
        public string ApplicationId { set; get; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { set; get; }

        [JsonProperty("applicationModuleId")]
        public string ApplicationModuleId { set; get; }

        [JsonProperty("srNo")]
        public string SRNo { set; get; }

        [JsonProperty("srType")]
        public string SRType { set; get; }

        [JsonProperty("searchApplicationType")]
        public string SearchApplicationType { set; get; }

        [JsonProperty("applicationModuleDescription")]
        public string ApplicationModuleDescription { set; get; }

        [JsonProperty("statusId")]
        public string StatusID { set; get; }

        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("isPremiseServiceReady")]
        public string IsPremiseServiceReady { set; get; }

        [JsonProperty("reTsType")]
        public string RETSType { set; get; }

        [JsonProperty("isSmartMeter")]
        public string IsSmartMeter { set; get; }

        [JsonProperty("isOpc")]
        public string IsOpc { set; get; }

        [JsonProperty("isLpc")]
        public string IsLpc { set; get; }

        [JsonProperty("isExpress")]
        public string IsExpress { set; get; }

        [JsonProperty("isGe")]
        public string IsGe { set; get; }

        [JsonProperty("isMeterChanged")]
        public string IsMeterChanged { set; get; }

        [JsonProperty("isLegacyAndInvalid")]
        public string IsLegacyAndInvalid { set; get; }

        [JsonProperty("isViewable")]
        public string IsViewable { set; get; }

        [JsonProperty("ViewMode")]
        public string ViewMode { set; get; }

        [JsonProperty("createdBy")]
        public string CreatedBy { set; get; }

        [JsonProperty("createdByUserId")]
        public string CreatedByUserID { set; get; }

        [JsonProperty("createdByRoleId")]
        public string CreatedByRoleID { set; get; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { set; get; }

        [JsonProperty("lastModifiedDate")]
        public DateTime LastModifiedDate { set; get; }
    }
}
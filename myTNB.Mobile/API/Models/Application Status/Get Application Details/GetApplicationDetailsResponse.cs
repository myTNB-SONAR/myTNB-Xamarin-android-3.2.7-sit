using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails
{
    public class GetApplicationDetailsResponse : BaseResponse<GetApplicationDetailsModel>
    {

    }

    public class GetApplicationDetailsModel : GetApplicationStatusModel
    {
        [JsonProperty("savedApplicationDetail")]
        public object SavedApplicationDetail { set; get; }

        [JsonProperty("addressSearchRequest")]
        public ASRDetail AddressSearchRequest { set; get; }

        [JsonProperty("changeLoadDetail")]
        public object ChangeLoadDetail { set; get; }

        [JsonProperty("changeOfTenancyDetail")]
        public object ChangeOfTenancyDetail { set; get; }

        [JsonProperty("changeProductDetail")]
        public object ChangeProductDetail { set; get; }

        [JsonProperty("merdekaIncentiveDetail")]
        public object MerdekaIncentiveDetail { set; get; }

        [JsonProperty("closeOfAccountDetail")]
        public object CloseOfAccountDetail { set; get; }

        [JsonProperty("greenTariffDetail")]
        public object GreenTariffDetail { set; get; }

        [JsonProperty("reTechStudyDetail")]
        public object RETechStudyDetail { set; get; }

        [JsonProperty("gslDetail")]
        public object GSLDetail { set; get; }

        [JsonProperty("helpFormDetail")]
        public object HelpFormDetail { set; get; }

        [JsonProperty("newConnectionDetail")]
        public NCDetail NewConnectionDetail { set; get; }

        [JsonProperty("projectDetail")]
        public PRJDetail ProjectDetail { set; get; }

        [JsonProperty("smrDetail")]
        public object SMRDetail { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class BaseApplicationDetails
    {
        [JsonProperty("referenceNo")]
        public string referenceNo { set; get; }

        [JsonProperty("applicationModuleId")]
        public string applicationModuleId { set; get; }

        [JsonProperty("statusId")]
        public string statusId { set; get; }

        [JsonProperty("createdByUserId")]
        public string createdByUserId { set; get; }

        [JsonProperty("createdByRoleId")]
        public string createdByRoleId { set; get; }

        [JsonProperty("createdDate")]
        public string createdDate { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ASRDetail : BaseApplicationDetails
    {
        [JsonProperty("addressSearchRequestId")]
        public string addressSearchRequestId { set; get; }

        [JsonProperty("applicationRefNo")]
        public string applicationRefNo { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class NCDetail : BaseApplicationDetails
    {
        [JsonProperty("newConnectionId")]
        public string newConnectionId { set; get; }

        [JsonProperty("srNo")]
        public string srNo { set; get; }

        [JsonProperty("srType")]
        public string srType { set; get; }

        [JsonProperty("statusCode")]
        public string statusCode { set; get; }

        [JsonProperty("firstName")]
        public string firstName { set; get; }

        [JsonProperty("lastName")]
        public string lastName { set; get; }

        [JsonProperty("premiseCategory")]
        public string premiseCategory { set; get; }

        [JsonProperty("premiseCategoryDescription")]
        public string premiseCategoryDescription { set; get; }

        [JsonProperty("accountType")]
        public string accountType { set; get; }

        [JsonProperty("accountTypeDescription")]
        public string accountTypeDescription { set; get; }

        [JsonProperty("electricityStartDate")]
        public string electricityStartDate { set; get; }

        [JsonProperty("statusDate")]
        public string statusDate { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class PRJDetail : BaseApplicationDetails
    {
        [JsonProperty("projectId")]
        public object projectId { set; get; }

        [JsonProperty("tnB_ProjectID")]
        public object tnB_ProjectID { set; get; }

        [JsonProperty("srNo")]
        public object srNo { set; get; }

        [JsonProperty("srType")]
        public object srType { set; get; }

        [JsonProperty("statusCode")]
        public object statusCode { set; get; }

        [JsonProperty("firstName")]
        public object firstName { set; get; }

        [JsonProperty("lastName")]
        public object lastName { set; get; }

        [JsonProperty("statusDate")]
        public object statusDate { set; get; }
    }
}
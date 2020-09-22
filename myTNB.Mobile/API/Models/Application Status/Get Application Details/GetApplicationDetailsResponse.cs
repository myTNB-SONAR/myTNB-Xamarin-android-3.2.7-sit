using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails
{
    public class GetApplicationDetailsResponse : BaseResponse<GetApplicationDetailsModel>
    {

    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class GetApplicationDetailsModel : GetApplicationStatusModel
    {
        [JsonProperty("savedApplicationDetail")]
        public object savedApplicationDetail { set; get; }

        [JsonProperty("addressSearchRequest")]
        public ASRDetail addressSearchRequest { set; get; }

        [JsonProperty("changeLoadDetail")]
        public object changeLoadDetail { set; get; }

        [JsonProperty("changeOfTenancyDetail")]
        public object changeOfTenancyDetail { set; get; }

        [JsonProperty("changeProductDetail")]
        public object changeProductDetail { set; get; }

        [JsonProperty("merdekaIncentiveDetail")]
        public object merdekaIncentiveDetail { set; get; }

        [JsonProperty("closeOfAccountDetail")]
        public object closeOfAccountDetail { set; get; }

        [JsonProperty("greenTariffDetail")]
        public object greenTariffDetail { set; get; }

        [JsonProperty("reTechStudyDetail")]
        public object reTechStudyDetail { set; get; }

        [JsonProperty("gslDetail")]
        public object gslDetail { set; get; }

        [JsonProperty("helpFormDetail")]
        public object helpFormDetail { set; get; }

        [JsonProperty("newConnectionDetail")]
        public NCDetail newConnectionDetail { set; get; }

        [JsonProperty("projectDetail")]
        public PRJDetail projectDetail { set; get; }

        [JsonProperty("smrDetail")]
        public object smrDetail { set; get; }
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
        public DateTime? createdDate { set; get; }

        [JsonIgnore]
        public string createDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = createdDate != null && createdDate.Value != null
                    ? createdDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
                return date;
            }
        }
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
        public DateTime? electricityStartDate { set; get; }

        [JsonProperty("statusDate")]
        public DateTime? statusDate { set; get; }

        [JsonIgnore]
        public string electricityStartDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = electricityStartDate != null && electricityStartDate.Value != null
                    ? electricityStartDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
                return date;
            }
        }
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
        public DateTime statusDate { set; get; }
    }
}
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
        public SavedDetail savedApplicationDetail { set; get; }

        public ASRDetail addressSearchRequest { set; get; }

        public object changeLoadDetail { set; get; }

        public object changeOfTenancyDetail { set; get; }

        public object changeProductDetail { set; get; }

        public object merdekaIncentiveDetail { set; get; }

        public object closeOfAccountDetail { set; get; }

        public object greenTariffDetail { set; get; }

        public object reTechStudyDetail { set; get; }

        public object gslDetail { set; get; }

        public object helpFormDetail { set; get; }

        public NCDetail newConnectionDetail { set; get; }

        public PRJDetail projectDetail { set; get; }

        public object smrDetail { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class BaseApplicationDetails
    {
        public string referenceNo { set; get; }

        public string applicationModuleId { set; get; }

        public string statusId { set; get; }

        public string createdByUserId { set; get; }

        public string createdByRoleId { set; get; }

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
    public class SavedDetail : BaseApplicationDetails
    {
        public string applicationId { set; get; }

        public string srNo { set; get; }

        public string srType { set; get; }

        public string statusCode { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ASRDetail : BaseApplicationDetails
    {
        public string addressSearchRequestId { set; get; }

        public string applicationRefNo { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class NCDetail : BaseApplicationDetails
    {
        public string newConnectionId { set; get; }

        public string srNo { set; get; }

        public string srType { set; get; }

        public string statusCode { set; get; }

        public string firstName { set; get; }

        public string lastName { set; get; }

        public string premiseCategory { set; get; }

        public string premiseCategoryDescription { set; get; }

        public string accountType { set; get; }

        public string accountTypeDescription { set; get; }

        public DateTime? electricityStartDate { set; get; }

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
        public object projectId { set; get; }

        public object tnB_ProjectID { set; get; }

        public object srNo { set; get; }

        public object srType { set; get; }

        public object statusCode { set; get; }

        public object firstName { set; get; }

        public object lastName { set; get; }

        public DateTime statusDate { set; get; }
    }

}
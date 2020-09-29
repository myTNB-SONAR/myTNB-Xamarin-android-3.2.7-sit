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

        public RW_LCDetail changeLoadDetail { set; get; }

        public COTDetail changeOfTenancyDetail { set; get; }

        public RW_PCDetail changeProductDetail { set; get; }

        public MDDetail merdekaIncentiveDetail { set; get; }

        public COADetail closeOfAccountDetail { set; get; }

        public GTDetail greenTariffDetail { set; get; }

        public RE_TSDetail reTechStudyDetail { set; get; }

        public RB_GSLDetail gslDetail { set; get; }

        public RB_SRQDetail helpFormDetail { set; get; }

        public NCDetail newConnectionDetail { set; get; }

        public PRJDetail projectDetail { set; get; }

        public SMRDetail smrDetail { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class BaseApplicationDetail
    {
        public string referenceNo { set; get; }

        public string applicationModuleId { set; get; }

        public string statusId { set; get; }

        public string createdByUserId { set; get; }

        public string createdByRoleId { set; get; }

        public string srNo { set; get; }

        public string srType { set; get; }

        public string statusCode { set; get; }

        public DateTime? createdDate { set; get; }

        public DateTime? statusDate { set; get; }

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
    public class SavedDetail : BaseApplicationDetail
    {
        public string applicationId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ASRDetail : BaseApplicationDetail
    {
        public string addressSearchRequestId { set; get; }

        public string applicationRefNo { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class NCDetail : BaseApplicationDetail
    {
        public string newConnectionId { set; get; }

        public string firstName { set; get; }

        public string lastName { set; get; }

        public string premiseCategory { set; get; }

        public string premiseCategoryDescription { set; get; }

        public string accountType { set; get; }

        public string accountTypeDescription { set; get; }

        public DateTime? electricityStartDate { set; get; }

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
    public class PRJDetail : BaseApplicationDetail
    {
        public string projectId { set; get; }

        public string tnB_ProjectID { set; get; }

        public string firstName { set; get; }

        public string lastName { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RB_SRQDetail : BaseApplicationDetail
    {
        public string helpFormDetail { set; get; }

        public string helpFormId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class COADetail : BaseApplicationDetail
    {
        public string closeOfAccountId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RB_GSLDetail : BaseApplicationDetail
    {
        public string gslId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class SMRDetail : BaseApplicationDetail
    {
        public string smrId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class COTDetail : BaseApplicationDetail
    {
        public string changeOfTenancyId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class MDDetail : BaseApplicationDetail
    {

    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RE_TSDetail : BaseApplicationDetail
    {

    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RW_LCDetail : BaseApplicationDetail
    {

    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RW_PCDetail : BaseApplicationDetail
    {

    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class GTDetail : BaseApplicationDetail
    {

    }
}
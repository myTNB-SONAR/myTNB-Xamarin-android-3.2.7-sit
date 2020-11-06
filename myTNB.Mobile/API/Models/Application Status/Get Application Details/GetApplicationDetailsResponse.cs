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
        public string createdByUserId { set; get; }
        public string createdByRoleId { set; get; }
        public string srNo { set; get; }
        public string srType { set; get; }
        public string statusId { set; get; }
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
        public string backendReferenceNo { set; get; }
        public string backendApplicationType { set; get; }
        public string backendModule { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ASRDetail : BaseApplicationDetail
    {
        public string addressSearchRequestId { set; get; }
        public string applicationRefNo { set; get; }
        public string linkedApplicationId { set; get; }
        public string linkedApplicationNo { set; get; }
        public string linkedApplicationType { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class NCDetail : BaseApplicationDetail
    {
        public string newConnectionId { set; get; }
        public string contractAccountNo { set; get; }
        public string accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string premiseCategory { set; get; }
        public string premiseCategoryDescription { set; get; }
        public string premiseAddress { set; get; }
        public string loadRequirement { set; get; }
        public string linkedAsrId { set; get; }
        public string linkedAsrReferenceNo { set; get; }
        public bool? isPremiseServiceReady { set; get; } = false;
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
        public string linkedAsrId { set; get; }
        public string linkedAsrReferenceNo { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RB_SRQDetail : BaseApplicationDetail
    {
        public string helpFormId { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class COADetail : BaseApplicationDetail
    {
        public string closeOfAccountId { set; get; }
        public string contractAccountNo { set; get; }
        public string premiseAddress { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RB_GSLDetail : BaseApplicationDetail
    {
        public string gslId { set; get; }
        public string contractAccountNo { set; get; }
        public string premiseAddress { set; get; }
        public string gsL_CategoryValue { set; get; }
        public string gsL_CategoryDescription { set; get; }
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
        public string linkedAsrId { set; get; }
        public string linkedAsrReferenceNo { set; get; }
        public string accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string ownershipType { set; get; }
        public string ownershipTypeDescription { set; get; }
        public bool? hasExistingOwnerSupportingDocument { set; get; } = false;
        public string premiseAddress { set; get; }

        [JsonIgnore]
        public string hasExistingOwnerSupportingDocumentDisplay
        {
            get
            {
                string key = hasExistingOwnerSupportingDocument == null
                    ? "no"
                    : hasExistingOwnerSupportingDocument.Value
                        ? "yes"
                        : "no";
                return LanguageManager.Instance.GetCommonValue(key);
            }
        }
    }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class MDDetail : BaseApplicationDetail
    {
        public string merdekaIncentiveId { set; get; }
        public string contractAccountNo { set; get; }
        public string premiseAddress { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RE_TSDetail : BaseApplicationDetail
    {
        public string reTechStudyId { set; get; }
        public string snNo { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RW_LCDetail : BaseApplicationDetail
    {
        public string changeLoadId { set; get; }
        public string contractAccountNo { set; get; }
        public string accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string identificationNo { set; get; }
        public string premiseAddress { set; get; }
        public string loadRequirement { set; get; }
        public string currentVoltage { set; get; }
        public string newVoltage { set; get; }
        public string electricityStartDate { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class RW_PCDetail : BaseApplicationDetail
    {
        public string changeProductId { set; get; }
        public string contractAccountNo { set; get; }
        public string accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string premiseType { set; get; }
        public string premiseAddress { set; get; }
        public string loadType { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class GTDetail : BaseApplicationDetail
    {
        public string greenTariffId { set; get; }
        public string contractAccountNo { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string premiseTypeItem { set; get; }
        public string premiseTypeItemDescription { set; get; }
        public string premiseAddress { set; get; }
    }
}
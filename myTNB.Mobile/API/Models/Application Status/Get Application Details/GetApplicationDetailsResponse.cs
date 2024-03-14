using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails
{
    public class GetApplicationDetailsResponse : BaseResponse<GetApplicationDetailsModel> { }

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

        [JsonProperty("applicationRatingDetail")]
        public ApplicationRatingDetail ApplicationRatingDetail { set; get; }

        [JsonProperty("applicationAppointmentDetail")]
        public ApplicationAppointmentDetail ApplicationAppointmentDetail { set; get; }

        [JsonProperty("myHomeDetails")]
        public MyHomeDetails MyHomeDetails { set; get; }
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
                string date = createdDate != null && createdDate.HasValue
                    ? createdDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
                return date;
            }
        }
        public string email { set; get; }
        public string mobileNo { set; get; }
        public string businessArea { set; get; }
        public bool isOwnApplication { set; get; }
        public string channel { set; get; }
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
        public int accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string premiseCategory { set; get; }
        public string premiseCategoryDescription { set; get; }
        public string premiseAddress { set; get; }
        public string loadRequirement { set; get; }
        public string linkedAsrId { set; get; }
        public string linkedAsrReferenceNo { set; get; }
        public string signApplicationURL { set; get; }
        public bool isVerifyNow { set; get; }
        public bool? isPremiseServiceReady { set; get; } = false;
        public DateTime? electricityStartDate { set; get; }
        public int premiseTypeHeaderId { set; get; }

        public bool isContractorApplied { set; get; }
        public string identificationNo { get; set; }
        public int? identificationType { get; set; }

        [JsonIgnore]
        public string electricityStartDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = electricityStartDate != null && electricityStartDate.HasValue
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
        public string accountName { set; get; }
        public int accountType { set; get; }
        public int premiseTypeHeaderId { set; get; }
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
        public int accountType { set; get; }
        public string accountTypeDescription { set; get; }
        public string ownershipType { set; get; }
        public string ownershipTypeDescription { set; get; }
        public bool? hasExistingOwnerSupportingDocument { set; get; } = false;
        public string premiseAddress { set; get; }
        public bool isExistingOwner { set; get; }
        public string contractAccountNo { set; get; }
        public string accountName { set; get; }
        public int premiseTypeHeaderId { set; get; }

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
        public string signApplicationURL { set; get; }
        public bool isVerifyNow { set; get; }
        public bool isContractorApplied { set; get; }
        public int? identificationType { get; set; }
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

    public class ApplicationRatingDetail
    {
        [JsonProperty("customerRating")]
        public CustomerRating CustomerRating { set; get; }
        [JsonProperty("contractorRating")]
        public ContractorRating ContractorRating { set; get; }
        [JsonProperty("submissionRating")]
        public SubmissionRating SubmissionRating { set; get; }
    }

    public class CustomerRating
    {
        [JsonProperty("srNo")]
        public string SRNo { set; get; }
        [JsonProperty("transactionId")]
        public string TransactionId { set; get; }
        [JsonProperty("rating")]
        public int? Rating { set; get; }
    }

    public class ContractorRating
    {
        [JsonProperty("contractorRatingUrl")]
        public string ContractorRatingUrl { set; get; }
        [JsonProperty("transactionId")]
        public string TransactionId { set; get; }
    }

    public class SubmissionRating
    {
        [JsonProperty("isSubmissionSurveyCompleted")]
        public bool IsSubmissionSurveyCompleted { set; get; }
    }

    public class ApplicationAppointmentDetail
    {
        [JsonProperty("srNo")]
        public string SRNo { set; get; }
        [JsonProperty("srType")]
        public string SRType { set; get; }
        [JsonProperty("mode")]
        public string Mode { set; get; }
        [JsonProperty("businessArea")]
        public string BusinessArea { set; get; }
        [JsonProperty("appointmentDate")]
        public DateTime? AppointmentDate { set; get; }
        [JsonProperty("appointmentStartTime")]
        public DateTime? AppointmentStartTime { set; get; }
        [JsonProperty("appointmentEndTime")]
        public DateTime? AppointmentEndTime { set; get; }
        [JsonProperty("appointmentDeadline")]
        public DateTime? AppointmentDeadline { set; get; }

        /// <summary>
        /// Use to display in CTA Message
        /// </summary>
        [JsonIgnore]
        public string AppointmentDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDate != null && AppointmentDate.HasValue
                    ? AppointmentDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }

        /// <summary>
        /// Use to display in CTA Message
        /// </summary>
        [JsonIgnore]
        public string AppointmentDeadlineDateTimeDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDeadline != null && AppointmentDeadline.HasValue
                    ? AppointmentDeadline.Value.ToString("dd MMM yyyy, h tt", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }

        /// <summary>
        /// Use to display in CTA Message
        /// </summary>
        [JsonIgnore]
        public string AppointmentDeadlineDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                return AppointmentDeadline != null && AppointmentDeadline.HasValue
                    ? AppointmentDeadline.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
            }
        }

        [JsonIgnore]
        public string TimeSlotDisplay
        {
            get
            {
                try
                {
                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                    string start = string.Empty;
                    string end = string.Empty;
                    string display = string.Empty;
                    if (AppointmentStartTime != null && AppointmentStartTime.HasValue)
                    {
                        start = AppointmentStartTime.Value.ToString("hh:mm tt", dateCultureInfo);
                    }

                    if (AppointmentEndTime != null && AppointmentEndTime.HasValue)
                    {
                        end = AppointmentEndTime.Value.ToString("hh:mm tt", dateCultureInfo);
                    }

                    if (start.IsValid() && end.IsValid())
                    {
                        display = string.Format("{0} - {1}", start, end);
                    }
                    else if (start.IsValid())
                    {
                        display = start;
                    }
                    else if (end.IsValid())
                    {
                        display = end;
                    }
                    return display;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] TimeSlotDisplay Error: " + e.Message);
                }
                return string.Empty;
            }
        }
    }

    public class MyHomeDetails
    {
        [JsonProperty("ssoDomain")]
        public string SSODomain { set; get; }
        [JsonProperty("originURL")]
        public string OriginURL { set; get; }
        [JsonProperty("redirectURL")]
        public string RedirectURL { set; get; }
        [JsonProperty("isOTPFailed")]
        public bool IsOTPFailed { set; get; }
        [JsonProperty("applicationFormDownloadURL")]
        public string ApplicationFormDownloadURL { set; get; }
    }
}
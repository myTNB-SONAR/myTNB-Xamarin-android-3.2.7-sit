using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace myTNB.Mobile.API.Models.ApplicationStatus
{
    public class GetApplicationStatusResponse : BaseResponse<GetApplicationStatusModel>
    {
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class GetApplicationStatusModel
    {
        public ApplicationDetail applicationDetail { set; get; }

        public ApplicationPaymentDetail applicationPaymentDetail { set; get; }

        [JsonProperty("applicationStatusDetail")]
        public ApplicationStatusDetail ApplicationStatusDetail { set; get; }

        [JsonProperty("applicationActivityLogDetail")]
        public List<ApplicationActivityLogDetail> ApplicationActivityLogDetail { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ApplicationDetail
    {
        public string applicationId { set; get; }

        public string referenceNo { set; get; }

        public string applicationModuleId { set; get; }

        public string backendReferenceNo { set; get; }

        public string backendApplicationType { set; get; }

        public string backendModule { set; get; }

        //public string srNo { set; get; }

        //public string srType { set; get; }

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
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ApplicationPaymentDetail
    {
        public double outstandingChargesAmount { set; get; }

        public double latestBillAmount { set; get; }

        public double oneTimeChargesAmount { set; get; }

        public OneTimeChargesDetail oneTimeChargesDetail { set; get; }

        public double totalPayableAmount { set; get; }

        public string caNo { set; get; }

        public string sdDocumentNo { set; get; }

        public string srNo { set; get; }

        public string snNo { set; get; }

        public bool? hasInvoiceAttachment { set; get; } = false;

        [JsonIgnore]
        public bool dbrEnabled { set; get; } = false;
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class OneTimeChargesDetail
    {
        public double connectionChargesAmount { set; get; }

        public ConnectionChargesDetails connectionChargesDetail { set; get; }

        public double technicalStudyFeeAmount { set; get; }

        public double securityDepositAmount { set; get; }

        public double meterFeeAmount { set; get; }

        public double stampDutyAmount { set; get; }

        public double processingFeeAmount { set; get; }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class ConnectionChargesDetails
    {
        public double connectionChargesNetAmount { set; get; }
        public double connectionChargesTaxAmount { set; get; }
    }

    public class ApplicationStatusDetail
    {
        [JsonProperty("statusId")]
        public int StatusId { set; get; }

        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("statusDescriptionColor")]
        public string StatusDescriptionColor { set; get; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { set; get; }

        [JsonProperty("userAction")]
        public string UserAction { set; get; }

        [JsonProperty("isPostPayment")]
        public bool IsPostPayment { set; get; }

        [JsonProperty("statusTracker")]
        public List<StatusTracker> StatusTracker { set; get; }
    }

    public class StatusTracker
    {
        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("statusMode")]
        public string StatusMode { set; get; }

        [JsonProperty("progressDetail")]
        public ProgressDetail ProgressDetail { set; get; }

        [JsonProperty("sequence")]
        public int Sequence { set; get; }

        [JsonProperty("statusDate")]
        public DateTime? StatusDate { set; get; }
    }

    public class ProgressDetail
    {
        [JsonProperty("tnB_ProjectID")]
        public string TNBProjectID { set; get; }
        [JsonProperty("progressTrackers")]
        public List<object> ProgressTrackers { set; get; }
    }

    public class ApplicationActivityLogDetail
    {
        [JsonProperty("statusId")]
        public int StatusID { set; get; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("changeLogs")]
        public List<ChangeLogs> ChangeLogs { set; get; }

        [JsonProperty("reasons")]
        public List<string> Reasons { set; get; }

        [JsonProperty("comment")]
        public string Comment { set; get; }

        [JsonProperty("createdBy")]
        public string CreatedBy { set; get; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { set; get; }
    }

    public class ChangeLogs
    {
        [JsonProperty("changeType")]
        public string ChangeType { set; get; }

        [JsonProperty("fieldName")]
        public string FieldName { set; get; }

        [JsonProperty("fieldDescription")]
        public string FieldDescription { set; get; }

        [JsonProperty("beforeValue")]
        public string BeforeValue { set; get; }

        [JsonProperty("beforeValueDescription")]
        public string BeforeValueDescription { set; get; }

        [JsonProperty("afterValue")]
        public string AfterValue { set; get; }

        [JsonProperty("afterValueDescription")]
        public string AfterValueDescription { set; get; }
    }
}
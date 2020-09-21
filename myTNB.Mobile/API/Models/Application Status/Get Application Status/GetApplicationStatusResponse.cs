using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.API.Models.ApplicationStatus
{
    public class GetApplicationStatusResponse : BaseResponse<GetApplicationStatusModel>
    {
    }

    public class GetApplicationStatusModel
    {
        [JsonProperty("applicationDetail")]
        public ApplicationDetail ApplicationDetail { set; get; }

        [JsonProperty("applicationPaymentDetail")]
        public ApplicationPaymentDetail ApplicationPaymentDetail { set; get; }

        [JsonProperty("applicationStatusDetail")]
        public ApplicationStatusDetail ApplicationStatusDetail { set; get; }

        [JsonProperty("applicationActivityLogDetail")]
        public List<ApplicationActivityLogDetail> ApplicationActivityLogDetail { set; get; }
    }

    public class ApplicationDetail
    {
        [JsonProperty("applicationId")]
        public int ApplicationId { set; get; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { set; get; }

        [JsonProperty("applicationModuleId")]
        public int ApplicationModuleId { set; get; }

        [JsonProperty("srNo")]
        public string SRNo { set; get; }

        [JsonProperty("srType")]
        public string SRType { set; get; }

        [JsonProperty("statusId")]
        public int StatusID { set; get; }

        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { set; get; }
    }

    public class ApplicationPaymentDetail
    {
        [JsonProperty("outstandingChargesAmount")]
        public double OutstandingChargesAmount { set; get; }

        [JsonProperty("latestBillAmount")]
        public double LatestBillAmount { set; get; }

        [JsonProperty("oneTimeChargesAmount")]
        public double OneTimeChargesAmount { set; get; }

        [JsonProperty("oneTimeChargesDetail")]
        public OneTimeChargesDetail OneTimeChargesDetail { set; get; }

        [JsonProperty("totalPayableAmount")]
        public double TotalPayableAmount { set; get; }

        [JsonProperty("caNo")]
        public string CANo { set; get; }

        [JsonProperty("sdDocumentNo")]
        public string SDDocumentNo { set; get; }

        [JsonProperty("srNo")]
        public string SRNo { set; get; }
    }

    public class OneTimeChargesDetail
    {
        [JsonProperty("connectionChargesAmount")]
        public double ConnectionChargesAmount { set; get; }

        [JsonProperty("securityDepositAmount")]
        public double SecurityDepositAmount { set; get; }

        [JsonProperty("meterFeeAmount")]
        public double MeterFeeAmount { set; get; }

        [JsonProperty("stampDutyAmount")]
        public double StampDutyAmount { set; get; }

        [JsonProperty("processingFeeAmount")]
        public double ProcessingFeeAmount { set; get; }
    }

    public class ApplicationStatusDetail
    {
        [JsonProperty("statusId")]
        public int StatusId { set; get; }

        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

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
        public DateTime CreatedDate { set; get; }
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
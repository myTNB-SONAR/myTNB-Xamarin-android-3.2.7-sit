using System;
using System.Collections.Generic;
using System.Globalization;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile
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

        //Mark: Display Specific Properties
        [JsonIgnore]
        public List<TitleValueModel> AdditionalInfoList { set; get; } = new List<TitleValueModel>();

        [JsonIgnore]
        public string ApplicationType { set; get; } = string.Empty;

        [JsonIgnore]
        public bool IsPayment
        {
            get
            {
                return ApplicationStatusDetail.UserAction.IsValid()
                    && ApplicationStatusDetail.UserAction == "payment";
            }
        }

        [JsonIgnore]
        public int[] StatusColor
        {
            get
            {
                switch (StatusColorDisplay)
                {
                    case Color.Green:
                        {
                            return new int[] { 32, 89, 76 };
                        }
                    case Color.Orange:
                        {
                            return new int[] { 255, 158, 67 };
                        }
                    case Color.Grey:
                    default:
                        {
                            return new int[] { 73, 73, 74 };
                        }
                }
            }
        }

        [JsonIgnore]
        public bool IsPortalMessageDisplayed
        {
            get
            {
                return StatusColorDisplay == Color.Orange && !IsPayment;
            }
        }

        [JsonIgnore]
        public string PortalMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "portalMessage");
            }
        }

        //Todo: Add logic to configure from landing and search
        [JsonIgnore]
        public bool IsSaveMessageDisplayed
        {
            get
            {
                return true;
            }
        }

        [JsonIgnore]
        public string SaveMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "saveMessage");
            }
        }

        [JsonIgnore]
        public string ApplicationTypeID { set; get; }

        [JsonIgnore]
        private Color StatusColorDisplay
        {
            get
            {
                if (ApplicationStatusDetail != null && ApplicationStatusDetail.UserAction.IsValid() && !IsPayment)
                {
                    if (ApplicationStatusDetail.StatusTracker != null
                        && ApplicationStatusDetail.StatusTracker.Count > 0
                        && ApplicationStatusDetail.StatusTracker[ApplicationStatusDetail.StatusTracker.Count - 1].StatusMode == "active")
                    {
                        //Mark: For ASR, only 127 can be completed
                        if (ApplicationTypeID == "ASR"
                            && ApplicationStatusDetail.StatusId != 127)
                        {
                            return Color.Orange;
                        }
                        return Color.Green;
                    }
                }
                else
                {
                    return Color.Orange;
                }
                return Color.Grey;
            }
        }

        private enum Color
        {
            Grey,
            Orange,
            Green
        }
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

        //Mark: Display Specific Properties
        [JsonIgnore]
        public string CreatedDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = CreatedDate.ToString("dd MMM yyyy", dateCultureInfo);
                return date;
            }
        }

        //Todo: Map with correct property after BE deployed
        [JsonIgnore]
        public string LastUpdatedDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string message = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "lastUpdatedDate");
                string date = CreatedDate != null ? CreatedDate.ToString("dd MMM yyyy", dateCultureInfo) : string.Empty;
                string displayDate = string.Format(message, date);
                return displayDate;
            }
        }
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

        [JsonProperty("userAction")]
        public string UserAction { set; get; }

        [JsonProperty("isPostPayment")]
        public bool IsPostPayment { set; get; }

        [JsonProperty("statusTracker")]
        public List<StatusTracker> StatusTracker { set; get; }

        //Mark: Display Specific Properties
        [JsonIgnore]
        public string StatusDescriptionDisplay
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "for") + StatusDescription;
            }
        }
    }

    public class StatusTracker
    {
        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("statusMode")]
        public string StatusMode { set; get; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { set; get; }

        [JsonProperty("progressDetail")]
        public string ProgressDetail { set; get; }

        [JsonProperty("sequence")]
        public int Sequence { set; get; }
    }

    public class ApplicationActivityLogDetail
    {
        [JsonProperty("statusId")]
        public string StatusID { set; get; }

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
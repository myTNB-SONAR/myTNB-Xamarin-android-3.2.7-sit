using System;
using System.Collections.Generic;
using System.Globalization;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile
{
    public class ApplicationDetailDisplay
    {
        public GetApplicationStatusDisplay Content { set; get; }
        public StatusDetail StatusDetail { set; get; }
    }

    public class GetApplicationStatusDisplay
    {
        public ApplicationDetailDisplayModel ApplicationDetail { set; get; }
        public ApplicationPaymentDisplayModel ApplicationPaymentDetail { set; get; }
        public ApplicationStatusDetailDisplayModel ApplicationStatusDetail { set; get; }
        public List<ApplicationActivityLogDetailDisplay> ApplicationActivityLogDetail { set; get; }

        //Mark: Display Specific Properties
        public bool IsActivityLogDisplayed
        {
            get
            {
                return ApplicationActivityLogDetail != null && ApplicationActivityLogDetail.Count > 0;
            }
        }

        public List<TitleValueModel> AdditionalInfoList { set; get; } = new List<TitleValueModel>();

        private string _applicationType = string.Empty;
        public string ApplicationType {
            set
            {
                if (value.IsValid())
                {
                    _applicationType = value;
                }
            }
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "for") + _applicationType;
            }
        }

        public string Status
        {
            get
            {
                return ApplicationStatusDetail.StatusDescription;
            }
        }

        public bool IsPayment
        {
            get
            {
                return ApplicationStatusDetail.UserAction.IsValid()
                    && ApplicationStatusDetail.UserAction == "payment";
            }
        }

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

        public bool IsPortalMessageDisplayed
        {
            get
            {
                return StatusColorDisplay == Color.Orange && !IsPayment;
            }
        }

        public string PortalMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "portalMessage");
            }
        }

        //Todo: Add logic to configure from landing and search
        public bool IsSaveMessageDisplayed { set; get; }
        public bool IsFullApplicationTooltipDisplayed { set; get; }

        public string SaveMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "saveMessage");
            }
        }

        public string ApplicationTypeID { set; get; }

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

    public class ApplicationDetailDisplayModel
    {
        public int ApplicationId { set; get; }
        public string ReferenceNo { set; get; }
        public int ApplicationModuleId { set; get; }
        public string SRNo { set; get; }
        public string SRType { set; get; }
        public int StatusID { set; get; }
        public string StatusCode { set; get; }
        public DateTime CreatedDate { set; get; }

        //Mark: Display Specific Properties
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

    public class ApplicationPaymentDisplayModel
    {
        public double OutstandingChargesAmount { set; get; }
        public double LatestBillAmount { set; get; }
        public double OneTimeChargesAmount { set; get; }
        public OneTimeChargesDisplayModel OneTimeChargesDetail { set; get; }
        public double TotalPayableAmount { set; get; }
    }

    public class OneTimeChargesDisplayModel
    {
        public double ConnectionChargesAmount { set; get; }
        public double SecurityDepositAmount { set; get; }
        public double MeterFeeAmount { set; get; }
        public double StampDutyAmount { set; get; }
        public double ProcessingFeeAmount { set; get; }
    }

    public class ApplicationStatusDetailDisplayModel
    {
        public int StatusId { set; get; }
        public string StatusCode { set; get; }
        public string StatusDescription { set; get; }
        public string UserAction { set; get; }
        public bool IsPostPayment { set; get; }
        public List<StatusTrackerDisplay> StatusTracker { set; get; }
        //Mark: Display Specific Properties
        public string StatusDescriptionDisplay
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "for") + StatusDescription;
            }
        }

        public bool IsLastStatusCompleted { set; get; }
    }

    public class StatusTrackerDisplay
    {
        public string StatusDescription { set; get; }
        public string StatusMode { set; get; }
        public string StatusMessage { set; get; }
        public string ProgressDetail { set; get; }
        public int Sequence { set; get; }
    }

    public class ApplicationActivityLogDetailDisplay
    {
        public int StatusID { set; get; }
        public string StatusDescription { set; get; }
        public List<ChangeLogsDisplay> ChangeLogs { set; get; }
        public string Comment { set; get; }
        public string CreatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public List<string> DetailsUpdateList { set; get; }
        public List<string> DocumentsUpdateList { set; get; }
        public List<string> Reasons { set; get; }
        public bool IsAwaitingApproval
        {
            get
            {
                return StatusID == 17;
            }
        }
        public string DisplayDate
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = CreatedDate.ToString("dd MMM yyyy", dateCultureInfo);
                return date;
            }
        }

    }

    public class ChangeLogsDisplay
    {
        public string ChangeType { set; get; }
        public string FieldName { set; get; }
        public string FieldDescription { set; get; }
        public string BeforeValue { set; get; }
        public string BeforeValueDescription { set; get; }
        public string AfterValue { set; get; }
        public string AfterValueDescription { set; get; }

        public ChangeType Type
        {
            get
            {
                if (ChangeType == "Documents")
                {
                    return Mobile.ChangeType.Documents;
                }
                else
                {
                    return Mobile.ChangeType.Fields;
                }
            }
        }

        public ChangeEvent Event
        {
            get
            {
                if (BeforeValueDescription.IsValid())
                {
                    return AfterValueDescription.IsValid() ? ChangeEvent.Update : ChangeEvent.Remove;
                }
                else
                {
                    return AfterValueDescription.IsValid() ? ChangeEvent.Add : ChangeEvent.Remove;
                }
            }
        }
    }

    public enum ChangeType
    {
        Documents,
        Fields
    }

    public enum ChangeEvent
    {
        Add,
        Update,
        Remove
    }
}

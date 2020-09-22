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
        public string ApplicationType
        {
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
                return ApplicationStatusDetail.IsPayment;
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
                            return new int[] { 32, 189, 76 };
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

        public DetailCTAType CTAType
        {
            get
            {
                DetailCTAType type = DetailCTAType.None;
                if (IsSaveMessageDisplayed)
                {
                    type = DetailCTAType.Save;
                }
                else if (IsPayment)
                {
                    type = DetailCTAType.Pay;
                }
                return type;
            }
        }

        private Color StatusColorDisplay
        {
            get
            {
                if (ApplicationStatusDetail != null)
                {
                    if (ApplicationStatusDetail.StatusTracker != null
                       && ApplicationStatusDetail.StatusTracker.Count > 0
                       && ApplicationStatusDetail.StatusTracker[ApplicationStatusDetail.StatusTracker.Count - 1].TrackerItemState == State.Completed)
                    {
                        return Color.Green;
                    }
                    if (!ApplicationStatusDetail.UserAction.IsValid())
                    {
                        return Color.Grey;
                    }
                    return Color.Orange;
                }
                return Color.Grey;
            }
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
        public DateTime? CreatedDate { set; get; }

        //Mark: Display Specific Properties
        public string CreatedDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = CreatedDate != null && CreatedDate.Value != null
                    ? CreatedDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
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
                string date = CreatedDate != null && CreatedDate.Value != null
                    ? CreatedDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
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
        public string CANo { set; get; }
        public string SDDocumentNo { set; get; }
        public string SRNo { set; get; }
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
        public string StatusMessage { set; get; }
        public string UserAction { set; get; }
        public bool IsPostPayment { set; get; }
        public List<StatusTrackerDisplay> StatusTracker { set; get; }
        //Mark: Display Specific Properties
        public bool IsPayment
        {
            get
            {
                if (UserAction != null || UserAction.IsValid())
                {
                    return UserAction.ToUpper() == "PAYMENT";
                }
                return false;
            }
        }
    }

    public class StatusTrackerDisplay
    {
        public string StatusDescription { set; get; }
        public string StatusMode { set; get; }
        public ProgressDetailDisplay ProgressDetail { set; get; }
        public int Sequence { set; get; }
        //Mark: Display Specific Properties
        public State TrackerItemState
        {
            get
            {
                State state = State.Inactive;
                if (StatusMode != null && StatusMode.IsValid())
                {
                    switch (StatusMode.ToUpper())
                    {
                        case "ACTIVE":
                            {
                                state = State.Active;
                                break;
                            }
                        case "PAST":
                            {
                                state = State.Past;
                                break;
                            }
                        case "COMPLETED":
                            {
                                state = State.Completed;
                                break;
                            }
                        case "INACTIVE":
                        default:
                            {
                                state = State.Inactive;
                                break;
                            }
                    }
                }
                return state;
            }
        }
    }

    public class ProgressDetailDisplay
    {
        public string ProjectID { set; get; }
        public List<object> ProgressTrackers { set; get; }
    }

    public class ApplicationActivityLogDetailDisplay
    {
        public int StatusID { set; get; }
        public string StatusDescription { set; get; }
        public List<ChangeLogsDisplay> ChangeLogs { set; get; }
        public string Comment { set; get; }
        public string CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
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
                string date = CreatedDate != null && CreatedDate.Value != null
                    ? CreatedDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
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
                if (ChangeType == "Documents" || ChangeType == "documents" || ChangeType == "DOCUMENTS")
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

    public enum Color
    {
        Grey,
        Orange,
        Green,
        VeryLightPink
    }

    public enum State
    {
        Inactive,
        Active,
        Payment,
        Past,
        Completed
    }

    public enum DetailCTAType
    {
        SetAppointment,
        Reschedule,
        Call,
        Rate,
        Save,
        Remove,
        Pay,
        None
    }
}
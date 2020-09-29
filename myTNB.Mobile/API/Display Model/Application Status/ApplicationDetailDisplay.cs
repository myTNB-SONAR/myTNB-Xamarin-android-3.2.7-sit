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
        private string _applicationType = string.Empty;

        public ApplicationDetailDisplayModel ApplicationDetail { set; get; }
#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// To be passed in payment service
        /// </summary>
        public ApplicationPaymentDetail applicationPaymentDetail { set; get; }
#pragma warning restore IDE1006 // Naming Styles
        public ApplicationStatusDetailDisplayModel ApplicationStatusDetail { set; get; }
        public List<ApplicationActivityLogDetailDisplay> ApplicationActivityLogDetail { set; get; }
        /// <summary>
        /// List of Title and Value used for payment details
        /// </summary>
        public List<TitleValueModel> PaymentDetailsList { set; get; }
        /// <summary>
        /// Get the total payable and one time charges display amounts
        /// </summary>
        public PaymentDisplayModel PaymentDisplay { set; get; }

        private bool IsPayment
        {
            get
            {
                return ApplicationStatusDetail.IsPayment;
            }
        }

        /// <summary>
        /// Determines if the application was saved or not
        /// Searched application defaults to false
        /// </summary>
        public bool IsSavedApplication { set; get; }

        /// <summary>
        /// This Determines if the Linked with section of NC should be displayed
        /// </summary>
        public bool IsLinkedWithDisplayed
        {
            get
            {
                //Mark: Saved and Searched application shouldn't see this
                if (IsSavedApplication || IsSaveMessageDisplayed)
                {
                    return false;
                }
                //Todo: Map property once available in unified service
                return false;
            }
        }

        /// <summary>
        /// Determines if the save message whould be hidden or not
        /// </summary>
        public bool IsSaveMessageDisplayed { set; get; }
        /// <summary>
        /// Determines if the Full Application Tooltip at the bottom of the page should be displayed or not
        /// </summary>
        public bool IsFullApplicationTooltipDisplayed { set; get; }
        /// <summary>
        /// Determines if the go to portal message should be displayed or not
        /// </summary>
        public bool IsPortalMessageDisplayed
        {
            get
            {
                return StatusColorDisplay == Color.Orange && !IsPayment;
            }
        }
        /// <summary>
        /// Determines if activity log CTA is displayed or not
        /// </summary>
        public bool IsActivityLogDisplayed
        {
            get
            {
                return ApplicationActivityLogDetail != null && ApplicationActivityLogDetail.Count > 0;
            }
        }
        /// <summary>
        /// Determines Kedai Tenaga Application
        /// </summary>
        public bool IsKedaiTenagaApplication
        {
            get
            {
                return !ApplicationDetail.ApplicationId.IsValid();
            }
        }
        /// <summary>
        /// Determines if Delete is displayed or not
        /// </summary>
        public bool IsDeleteEnable { set; get; }

        public string ApplicationTypeReference { set; get; }
        /// <summary>
        /// Application Type Text
        /// </summary>
        public string ApplicationType
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "for") + ApplicationTypeReference;
            }
        }
        /// <summary>
        /// Status Message
        /// </summary>
        public string Status
        {
            get
            {
                return ApplicationStatusDetail.StatusDescription;
            }
        }
        /// <summary>
        /// Go to Portal Message
        /// </summary>
        public string PortalMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "portalMessage");
            }
        }
        /// <summary>
        /// Save Message
        /// </summary>
        public string SaveMessage
        {
            get
            {
                return LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "saveMessage");
            }
        }

        public string ApplicationTypeID { set; get; }
        /// <summary>
        /// RGB of the Status
        /// </summary>
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
        /// <summary>
        /// Additional information list that contains Title and Value
        /// </summary>
        public List<TitleValueModel> AdditionalInfoList { set; get; } = new List<TitleValueModel>();
        /// <summary>
        /// Determines the CTA to display
        /// </summary>
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

    public class PaymentDisplayModel : ApplicationPaymentDetail
    {
        /// <summary>
        /// Total Payable amount Display without the currency
        /// </summary>
        public string TotalPayableAmountDisplay
        {
            get
            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (totalPayableAmount != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                {
                    return totalPayableAmount.ToAmountDisplayString();
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// One Time charges display without the Currency
        /// </summary>
        public string OneTimeChargesAmountDisplay
        {
            get
            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (oneTimeChargesAmount != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                {
                    return oneTimeChargesAmount.ToAmountDisplayString();
                }
                return string.Empty;
            }
        }
    }

    public class ApplicationDetailDisplayModel
    {
        public string ApplicationId { set; get; }
        public string ReferenceNo { set; get; }
        public string ApplicationModuleId { set; get; }
        public string BackendReferenceNo { set; get; }
        public string BackendApplicationType { set; get; }
        public string BackendModule { set; get; }
        public string SRNo { set; get; }
        public string SRType { set; get; }
        public string StatusID { set; get; }
        public string StatusCode { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? StatusDate { set; get; }

        /// <summary>
        /// Formatted Created date display
        /// </summary>
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
        /// <summary>
        /// Determines if last updated date should be displayed or not
        /// </summary>
        public bool IsDisplayLastUpdatedDate
        {
            get
            {
                return StatusDate != null && StatusDate.Value != null;
            }
        }
        /// <summary>
        /// Formatted lasy updated date display
        /// </summary>
        public string LastUpdatedDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string message = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "lastUpdatedDate");
                string date = StatusDate != null && StatusDate.Value != null
                    ? StatusDate.Value.ToString("dd MMM yyyy", dateCultureInfo) ?? string.Empty
                    : string.Empty;
                string displayDate = string.Format(message, date);
                return displayDate;
            }
        }
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
        /// <summary>
        /// Determines if the application requires payment or not
        /// </summary>
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
        /// <summary>
        /// Determines the state of each item in progress tracker
        /// </summary>
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
        /// <summary>
        /// List of Updated details
        /// Should be displayed with •
        /// </summary>
        public List<string> DetailsUpdateList { set; get; }
        /// <summary>
        /// List of Updated documents
        /// Should be displayed with •
        /// </summary>
        public List<string> DocumentsUpdateList { set; get; }
        /// <summary>
        /// List of reasons
        /// Should be displayed with •
        /// </summary>
        public List<string> Reasons { set; get; }
        /// <summary>
        /// Determines if item is awaiting for approval
        /// This is used to determine which icon to display
        /// </summary>
        public bool IsAwaitingApproval
        {
            get
            {
                return StatusID == 17;
            }
        }
        /// <summary>
        /// Formatted display date
        /// </summary>
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
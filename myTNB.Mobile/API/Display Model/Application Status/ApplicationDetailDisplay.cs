using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using myTNB.Mobile.API.Models.ApplicationStatus;
using myTNB.Mobile.API.Models.ApplicationStatus.ApplicationDetails;
using myTNB.Mobile.API.Models.Payment.PostApplicationsPaidDetails;
using myTNB.Mobile.Extensions;
using myTNB.Mobile.SessionCache;

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

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// To be passed in payment service
        /// </summary>
        public ApplicationPaymentDetail applicationPaymentDetail { set; get; }
#pragma warning restore IDE1006 // Naming Styles
        public ApplicationStatusDetailDisplayModel ApplicationStatusDetail { set; get; }
        public List<ApplicationActivityLogDetailDisplay> ApplicationActivityLogDetail { set; get; }

        public ApplicationRatingDetail ApplicationRatingDetail { set; get; }

        /// <summary>
        /// List of Title and Value used for payment details
        /// </summary>
        public List<TitleValueModel> PaymentDetailsList { set; get; }

        /// <summary>
        /// Get the total payable and one time charges display amounts
        /// </summary>
        public PaymentDisplayModel PaymentDisplay { set; get; }

        /// <summary>
        /// Used for Linked With Information
        /// </summary>
        public LinkedWithDisplay LinkedWithDisplay { set; get; }

        /// <summary>
        /// Used to display details of Receipt
        /// </summary>
        public List<ReceiptDisplay> ReceiptDisplay { set; get; }

        /// <summary>
        /// Used to display details of Tax Invoice
        /// </summary>
        public TaxInvoiceDisplay TaxInvoiceDisplay { set; get; }

        /// <summary>
        /// Determines if receipt segment is displayed or not
        /// </summary>
        public bool IsReceiptDisplayed
        {
            get
            {
                return ReceiptDisplay != null && ReceiptDisplay.Count > 0;
            }
        }

        /// <summary>
        /// Determines if Tax Invoice is displayed or not
        /// </summary>
        public bool IsTaxInvoiceDisplayed
        {
            get
            {
                return PaymentDisplay != null
                    && PaymentDisplay.hasInvoiceAttachment != null
                    && PaymentDisplay.hasInvoiceAttachment.Value;
            }
        }

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
                //Mark: Saved, Searched and Kedai application shouldn't see this
                if (IsSavedApplication || IsSaveMessageDisplayed || IsKedaiTenagaApplication)
                {
                    return false;
                }
                return LinkedWithDisplay != null
                    && LinkedWithDisplay.ReferenceNo.IsValid()
                    && LinkedWithDisplay.ID.IsValid();
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

        /// <summary>
        /// Determines if BCRM is Offline and if need to show BCRM Downtime
        /// </summary>
        public bool IsOffLine
        {
            set;
            get;
        }

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
        /// Used to pass in Remove Application
        /// </summary>
        public string System { set; get; } = string.Empty;

        /// <summary>
        /// Used to pass in Remove Application
        /// </summary>
        public string SavedApplicationID { set; get; } = string.Empty;

        /// <summary>
        /// Used to pass in Remove Application
        /// </summary>
        public string ApplicationTypeCode { set; get; } = string.Empty;

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
                else if (IsPayment && IsOffLine)
                {
                    type = DetailCTAType.PayOffline;
                }
                else if (IsPayment && applicationPaymentDetail != null)
                {
                    type = DetailCTAType.Pay;
                    if (ReceiptDisplay != null && ReceiptDisplay.Count > 0)
                    {
                        int isPendingIndex = ReceiptDisplay.FindIndex(x => x.IsPaymentPending);
                        if (isPendingIndex > -1)
                        {
                            type = DetailCTAType.PayInProgress;
                        }
                    }
                }
                else if (ApplicationRatingDetail != null)
                {
                    try
                    {
                        if (ApplicationStatusDetail != null
                            && ApplicationStatusDetail.StatusDescriptionColor is string descriptionColor
                            && descriptionColor.IsValid()
                            && descriptionColor.ToUpper() == "COMPLETED") {
                            if (ApplicationRatingDetail.CustomerRating != null
                                && !ApplicationRatingDetail.CustomerRating.TransactionId.IsValid())
                            {
                                type = DetailCTAType.CustomerRating;
                            }
                            else if (ApplicationRatingDetail.ContractorRating != null
                                && ApplicationRatingDetail.ContractorRating.ContractorRatingUrl.IsValid())
                            {
                                type = DetailCTAType.ContractorRating;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("[DEBUG] ApplicationRatingDetail CTA Error: " + e.Message);
                    }
                }
                return type;
            }
        }
        /// <summary>
        /// Returns the type of tutorial to display
        /// </summary>
        public DetailTutorialType TutorialType
        {
            get
            {
                DetailTutorialType type = DetailTutorialType.NoAction;
                if (IsSaveMessageDisplayed)
                {
                    return DetailTutorialType.None;
                }
                if (ApplicationStatusDetail != null
                    && ApplicationStatusDetail.StatusDescriptionColor is string descriptionColor
                    && descriptionColor.IsValid())
                {
                    switch (descriptionColor.ToUpper())
                    {
                        case "COMPLETED":
                        case "CANCELLED":
                        default:
                            {
                                type = DetailTutorialType.NoAction;
                                if (CTAType == DetailCTAType.CustomerRating || CTAType == DetailCTAType.ContractorRating)
                                {
                                    type = DetailTutorialType.Action;
                                }
                                break;
                            }
                        case "ACTION":
                            {
                                if (CTAType == DetailCTAType.None)
                                {
                                    type = DetailTutorialType.InProgress;
                                }
                                else
                                {
                                    type = DetailTutorialType.Action;
                                }
                                break;
                            }
                    }
                }
                return type;
            }
        }

        /// <summary>
        /// Used for getting ASMX Payment Details
        /// </summary>
        public string SRNumber { set; get; }

        /// <summary>
        /// Display Rating
        /// </summary>
        public string RatingDisplay
        {
            get
            {
                string rating = string.Empty;
                if (ApplicationRatingDetail != null
                    && ApplicationRatingDetail.CustomerRating != null
                    && ApplicationRatingDetail.CustomerRating.Rating != null
                    && ApplicationRatingDetail.CustomerRating.Rating.Value > 0)
                {
                    rating = string.Format("{0}{1}"
                        , LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "youRated")
                        , ApplicationRatingDetail.CustomerRating.Rating.Value.ToString());
                }
                return rating;
            }
        }

        /// <summary>
        /// Determines if Rating is Displayed or Not
        /// </summary>
        public bool IsRatingDisplayed
        {
            get
            {
                return RatingDisplay.IsValid();
            }
        }

        /// <summary>
        /// Determine if Ccontractor Rating Enabled
        /// </summary>
        public bool IsContractorRating
        {
            get
            {
                return ApplicationRatingDetail != null
                    && ApplicationRatingDetail.ContractorRating != null
                    && ApplicationRatingDetail.ContractorRating.ContractorRatingUrl.IsValid();
            }
        }

        /// <summary>
        /// URL used for contractor rating
        /// </summary>
        public string ContractorRatingURL
        {
            get
            {
                return ApplicationRatingDetail != null
                    && ApplicationRatingDetail.ContractorRating != null
                    && ApplicationRatingDetail.ContractorRating.ContractorRatingUrl.IsValid()
                        ? ApplicationRatingDetail.ContractorRating.ContractorRatingUrl
                        : string.Empty;
            }
        }

        private Color StatusColorDisplay
        {
            get
            {
                Color color = Color.Grey;
                if (ApplicationStatusDetail != null
                    && ApplicationStatusDetail.StatusDescriptionColor is string descriptionColor
                    && descriptionColor.IsValid())
                {

                    switch (descriptionColor.ToUpper())
                    {
                        case "COMPLETED":
                            {
                                color = Color.Green;
                                break;
                            }
                        case "ACTION":
                            {
                                color = Color.Orange;
                                break;
                            }
                        case "CANCELLED":
                        default:
                            {
                                color = Color.Grey;
                                break;
                            }
                    }
                }
                return color;
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
        //public string SRNo { set; get; }
        //public string SRType { set; get; }
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
        public string StatusDescriptionColor { set; get; }
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
        public DateTime? StatusDate { set; get; }
        /// <summary>
        /// Displays Completed Date
        /// </summary>
        public string CompletedDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = string.Empty;
                if (StatusDate != null && StatusDate.Value != null)
                {
                    string dateString = StatusDate.Value.ToString("dd MMM yyyy", dateCultureInfo);
                    if (dateString.IsValid())
                    {
                        date = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "on") + dateString;
                    }
                }
                return date;
            }
        }

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

    public class LinkedWithDisplay
    {
        public string ID { set; get; }
        public string ReferenceNo { set; get; }
        public string Type { set; get; }
        public string System
        {
            get
            {
                return "myTNB";
            }
        }
        public string ApplicationModuleDescription
        {
            get
            {
                return SearchApplicationTypeCache.Instance.GetApplicationTypeDescription(Type);
            }
        }
        public bool IsPremiseServiceReady { set; get; }
    }

    public class ReceiptDisplay : PostApplicationsPaidDetailsDataModel
    {
        public string PaymentDateDisplay
        {
            get
            {
                CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(AppInfoManager.Instance.Language.ToString());
                string date = string.Empty;
                if (PaymentDoneDate != null && PaymentDoneDate.Value != null)
                {
                    string dateString = PaymentDoneDate.Value.ToString("dd MMM", dateCultureInfo);
                    if (dateString.IsValid())
                    {
                        date = dateString.ToUpper() + LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "paymentReceiptTitle");
                    }
                }
                return date;
            }
        }

        public string AmountDisplay
        {
            get
            {
                return Amount.ToAmountDisplayString(true);
            }
        }
    }

    public class TaxInvoiceDisplay
    {
        public string SRNumber { set; get; } = string.Empty;
        public string Amount { set; get; }
        public string AmountDisplay
        {
            get
            {
                string format = "{0} {1}";
                string amountString = Amount.IsValid() ? Amount : "0.00";
                return string.Format(format, Constants.Constants_Currency, amountString);
            }
        }
    }

    #region Enums
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
        CustomerRating,
        ContractorRating,
        Save,
        Remove,
        Pay,
        PayInProgress,
        PayOffline,
        None
    }

    public enum DetailTutorialType
    {
        NoAction,
        InProgress,
        Action,
        None
    }
    #endregion
}
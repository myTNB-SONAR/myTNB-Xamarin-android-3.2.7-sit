namespace myTNB_Android.Src.Utils
{
    public class LanguageConstants
    {
        internal static readonly string DASHBOARD_HOME = "DashboardHome";
        internal static readonly string BILLS = "Bills";
        internal static readonly string COMMON = "Common";
        internal static readonly string ERROR = "Error";
        internal static readonly string NBR_COMMS = "NewBillDesignComms";
        internal static readonly string BILL_DETAILS = "BillDetails";
        internal static readonly string BILL_FILTER = "BillFilter";
        internal static readonly string SUBMIT_ENQUIRY = "SubmitEnquiry";
        internal static readonly string FEEDBACK_FORM = "FeedbackForm";

        internal struct Common
        {
            internal static readonly string GOT_IT = "gotIt";
            internal static readonly string NEXT = "next";
            internal static readonly string MOBILE_NO = "mobileNo";
            internal static readonly string CANCEL = "cancel";
        }

        internal struct Error
        {
            internal static readonly string RWDS_UNAVAILABLE_TITLE = "rewardsUnavailableTitle";
            internal static readonly string RWDS_UNAVAILABLE_MSG = "rewardsUnavailableMsg";
        }

        internal struct DashboardHome
        {
            internal static readonly string DBR_REMINDER_POPUP_TITLE = "dbrReminderPopupTitle";
            internal static readonly string DBR_REMINDER_POPUP_MESSAGE = "dbrReminderPopupMessage";
            internal static readonly string DBR_REMINDER_POPUP_START_NOW = "dbrReminderPopupStartNow";
            internal static readonly string GOT_IT = "gotIt";
        }

        internal struct Bills
        {
            internal static readonly string TOOLTIP_ACT_STMT_TITLE = "accountStatementTenantTitle";
            internal static readonly string TOOLTIP_ACT_STMT_MSG = "accountStatementTenantMessage";
        }

        internal struct BillDetails
        {
            internal static readonly string OUTSTANDING_CHARGES = "outstandingCharges";
            internal static readonly string OUTSTANDING_CHARGES_V2 = "outstandingChargesV2";
            internal static readonly string BILL_THIS_MONTH = "billThisMonth";
            internal static readonly string BILL_THIS_MONTH_V2 = "billThisMonthV2";

        }

        internal struct BillFilter
        {
            internal static readonly string FILTER_TITLE = "selectViewTitle";
            internal static readonly string FILTER_DESC = "descriptionRedesign";
        }

        internal struct NBRComms
        {
            internal static readonly string NBR_TITLE = "title";
            internal static readonly string NBR_BTN_TITLE = "viewYourBills";
        }

        internal struct SubmitEnquiry
        {
            internal static readonly string GSL_TITLE = "gslTitle";
            internal static readonly string GSL_DESC = "gslDescription";
            internal static readonly string GSL_HEADER_TITLE = "gslHeaderTitle";
            internal static readonly string GSL_STEP_TITLE = "stepTitle";
            internal static readonly string GSL_CLAIM_TITLE = "rebateClaimTitle";
            internal static readonly string GSL_REBATE_TYPE_TITLE = "rebateTypeTitle";
            internal static readonly string GSL_TENANT_INFO_TITLE = "tenantInfoTitle";
            internal static readonly string REBATE_TYPE = "rebateType";
            internal static readonly string REBATE_TYPE_KEY = "key";
            internal static readonly string REBATE_TYPE_DESC = "description";
            internal static readonly string FULL_NAME_HINT = "nameHint";
            internal static readonly string EMAIL_HINT = "emailHint";
            internal static readonly string MOBILE_HINT = "mobileHint";
            internal static readonly string EMAIL_ERROR = "emailReq";
            internal static readonly string MOBILE_ERROR = "mobileReq";
            internal static readonly string FULL_NAME_ERROR = "ownerReq";
            internal static readonly string INCIDENT_DATE_HINT = "incidentDate";
            internal static readonly string INCIDENT_TIME_HINT = "incidentTime";
            internal static readonly string RESTORATION_DATE_HINT = "restorationDate";
            internal static readonly string RESTORATION_TIME_HINT = "restorationTime";
            internal static readonly string GSL_INCIDENT_INFO_TITLE = "incidentInformationTitle";
            internal static readonly string GSL_INCIDENT_ITEM_TITLE = "incidentTitle";
            internal static readonly string GSL_UPLOAD_TITLE = "uploadDocumentTitle";
            internal static readonly string GSL_UPLOAD_TA_TITLE = "tenancyAgreementTitle";
            internal static readonly string GSL_UPLOAD_TA_DEC = "tenancyAgreementAttachmentDescription";
            internal static readonly string UPLOAD_OWNER_IC = "ownerIcOwner";
            internal static readonly string UPLOAD_ATTACH_DESC = "attachDescription";
            internal static readonly string UPLOAD_IC_TOOLTIP = "icInfo";
            internal static readonly string DOCUMENT_PDF = "documentPdf";
        }

        internal struct FeedbackForm
        {
            internal static readonly string TAKE_PHOTO = "takePhoto";
            internal static readonly string CHOOSE_FROM_LIBRARY = "chooseFromLibrary";
            internal static readonly string SELECT_OPTIONS = "selectOptions";
        }
    }
}

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
        internal static readonly string APPLICATION_STATUS_DETAILS = "ApplicationStatusDetails";
        internal static readonly string GSL = "GSL";
        internal static readonly string STATEMENT_PERIOD = "StatementPeriod";
        internal static readonly string PUSH_NOTIF_DETAILS = "PushNotificationDetails";
        internal static readonly string DBR_WEBVIEW = "DBRWebview";
        internal static readonly string USAGE = "Usage";
        internal static readonly string MANAGE_DIGITAL_BILL = "ManageDigitalBillLanding";
        internal static readonly string DS_LANDING = "DSLanding";
        internal static readonly string DS_WEBVIEW = "DSWebview";

        internal struct Common
        {
            internal static readonly string GOT_IT = "gotIt";
            internal static readonly string NEXT = "next";
            internal static readonly string MOBILE_NO = "mobileNo";
            internal static readonly string CANCEL = "cancel";
            internal static readonly string REFRESH_MSG = "refreshDescription";
            internal static readonly string REFRESH_NOW = "refreshNow";
            internal static readonly string OK = "ok";
            internal static readonly string SUBMIT = "submit";
        }

        internal struct Error
        {
            internal static readonly string RWDS_UNAVAILABLE_TITLE = "rewardsUnavailableTitle";
            internal static readonly string RWDS_UNAVAILABLE_MSG = "rewardsUnavailableMsg";
            internal static readonly string INVALID_FULLNAME = "invalid_fullname";
            internal static readonly string INVALID_EMAIL = "invalid_email";
            internal static readonly string DEFAULT_ERROR_TITLE = "defaultErrorTitle";
            internal static readonly string DEFAULT_ERROR_MSG = "defaultErrorMessage";
        }

        internal struct DashboardHome
        {
            internal static readonly string DBR_REMINDER_POPUP_TITLE = "dbrReminderPopupTitle";
            internal static readonly string DBR_REMINDER_POPUP_MESSAGE = "dbrReminderPopupMessage";
            internal static readonly string DBR_REMINDER_POPUP_START_NOW = "dbrReminderPopupStartNow";
            internal static readonly string DBR_REMINDER_POPUP_GOT_IT = "dbrReminderPopupGotIt";
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
            internal static readonly string ROUNDING_ADJUSTMENT = "roundingAdjustment";
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
            internal static readonly string UPLOAD_IC_TOOLTIP_TITLE = "copyICTitle";
            internal static readonly string UPLOAD_IC_TOOLTIP_MSG = "copyIcDet";
            internal static readonly string ENQUIRY_TNC = "enquiryTnc";
            internal static readonly string ENQUIRY_TNC_READ = "enquiryTncRead";
            internal static readonly string TNC = "tnc";
            internal static readonly string PERSONAL_DISCLAIMER = "personalDisclamer";
            internal static readonly string TERMS_OF_USE = "tnbTermUse";
            internal static readonly string PRIVACY_POLICY_TITLE = "privacyPolicyTitle";
            internal static readonly string TNC_AGREE_OWNER = "tncAgreeOwner";
            internal static readonly string TNC_AGREE_NON_OWNER = "tncAgreeNonOwner";
            internal static readonly string ANTI_SPAM_POLICY = "antiSpamPolicy";
            internal static readonly string PRIVACY_POLICY = "privacyPolicy";
            internal static readonly string SERVICE_NO_TITLE = "serviceNoTitle";
            internal static readonly string CONTACT_DETAILS_TITLE = "contactDetailsTitle";
            internal static readonly string SUPPORTING_DOCS_TITLE = "supportingDocTitle";
            internal static readonly string FOR = "for";
            internal static readonly string GSL_SUCCESS_TITLE = "thankYouGSLTitle";
            internal static readonly string GSL_SUCCESS_DESC = "thankYouGSLDescription";
            internal static readonly string SUCCESS_TITLE = "thankYouTitle";
            internal static readonly string SUCCESS_DESC = "thankYouDescription";
            internal static readonly string SUCCESS_SERVICE_NO_TITLE = "serviceNoTitle";
            internal static readonly string SUCCESS_BACK_TO_HOME = "backHomeButton";
            internal static readonly string SUCCESS_BACK_TO_LOGIN = "backLogin";
            internal static readonly string SUCCESS_VIEW_SUBMITTED_ENQUIRY = "viewSubmittedEnquiry";
            internal static readonly string PREMISES_ADDRESS = "premiseAddressTitle";
        }

        internal struct FeedbackForm
        {
            internal static readonly string TAKE_PHOTO = "takePhoto";
            internal static readonly string CHOOSE_FROM_LIBRARY = "chooseFromLibrary";
            internal static readonly string SELECT_OPTIONS = "selectOptions";
        }

        internal struct ApplicationStatusDetails
        {
            internal static readonly string STATUS_LABEL = "status";
        }

        internal struct Gsl
        {
            internal static readonly string TITLE = "title";
            internal static readonly string BACK_BTN_TITLE = "primaryCTA";
            internal static readonly string PROCEED_BTN_TITLE = "secondaryCTA";
        }

        internal struct StatementPeriod
        {
            internal static readonly string TITLE = "title";
            internal static readonly string PROCESSING_TITLE = "processingRequestTitle";
            internal static readonly string PROCESSING_MSG = "processingRequestMessage";
            internal static readonly string TIMEOUT_TITLE = "timeoutTitle";
            internal static readonly string TIMEOUT_MSG = "timeoutMessage";
            internal static readonly string BACK_TO_BILLS = "backToBills";
            internal static readonly string DISCLAIMER = "disclaimer";
            internal static readonly string REQUEST_TITLE = "iWantToRequestForTitle";
            internal static readonly string STATEMENT_PERIOD_TITLE = "preferredStatementPeriodTitle";
            internal static readonly string PAST_3_MONTHS = "past3Months";
            internal static readonly string PAST_6_MONTHS = "past6Months";
            internal static readonly string CONFIRM = "confirm";
            internal static readonly string NO_TRANSACTION_MSG = "noTransactionMessage";
        }

        internal struct PushNotificationDetails
        {
            internal static readonly string VIEW_ACCT_STMNT = "viewAccountStatement";
            internal static readonly string NOTIF_TITLE_DEFAULT = "notification";
            internal static readonly string NOTIF_TITLE_ENERGY_BUDGET = "EnergyBudgetTitle";
            internal static readonly string NOTIF_TITLE_SRVC_DISTRUPTION = "serviceDistruption";
            internal static readonly string NOTIF_TITLE_DIGITAL_SIGNATURE = "digitalSignature";
        }

        internal struct DBRWebview
        {
            internal static readonly string GO_PAPERLESS = "goPaperless";
            internal static readonly string UPDATE_BILL_DELIVERY = "updateBillDelivery";
            internal static readonly string CONFIRM_TITLE = "confirmPopupTitle";
            internal static readonly string CONFIRM_MSG = "confirmPopupMessage";
            internal static readonly string YES = "yes";
            internal static readonly string NO = "no";
        }

        internal struct Usage
        {
            internal static readonly string DBR_REMINDER_POPUP_TITLE = "dbrReminderPopupTitle";
            internal static readonly string DBR_REMINDER_POPUP_MSG = "dbrReminderPopupMessage";
            internal static readonly string DBR_REMINDER_POPUP_VIEW_MORE = "dbrReminderPopupViewMore";
            internal static readonly string DBBR_REMINDER_POPUP_GOT_IT = "dbrReminderPopupGotIt";
        }

        internal struct ManageDigitalBill
        {
            internal static readonly string DBR_INFO_TITLE_0 = "dbrInfoTitle0";
            internal static readonly string DBR_INFO_TITLE_1 = "dbrInfoTitle1";
            internal static readonly string DBR_INFO_TITLE_2 = "dbrInfoTitle2";
            internal static readonly string DBR_INFO_TITLE_2_V2 = "dbrInfoTitle2V2";
            internal static readonly string DBR_INFO_DESC_2_V2_NON_RES = "dbrInfoDescription2V2NonResidential";
            internal static readonly string DBR_INFO_TITLE_3 = "dbrInfoTitle3";
            internal static readonly string DBR_INFO_DESC_0 = "dbrInfoDescription0";
            internal static readonly string DBR_INFO_DESC_1 = "dbrInfoDescription1";
            internal static readonly string DBR_INFO_DESC_2 = "dbrInfoDescription2";
            internal static readonly string DBR_INFO_DESC_2_V2 = "dbrInfoDescription2V2";
            internal static readonly string DBR_INFO_DESC_3 = "dbrInfoDescription3";
        }

        internal struct DSLanding
        {
            internal static readonly string TITLE = "title";
            internal static readonly string CONTINUE = "continue";
            internal static readonly string SUB_HEADER = "subHeader";
            internal static readonly string ITEM_LIST_TITLE_1 = "howItWorksTitle1";
            internal static readonly string ITEM_LIST_TITLE_2 = "howItWorksTitle2";
            internal static readonly string ITEM_LIST_TITLE_3 = "howItWorksTitle3";
            internal static readonly string ITEM_LIST_DESC_1 = "howItWorksDescription1";
            internal static readonly string ITEM_LIST_DESC_2 = "howItWorksDescription2";
            internal static readonly string ITEM_LIST_DESC_3 = "howItWorksDescription3";
        }

        internal struct DSWebView
        {
            internal static readonly string TITLE = "title";
            internal static readonly string CONFIRM_POPUP_TITLE = "confirmPopupTitle";
            internal static readonly string CONFIRM_POPUP_MSG = "confirmPopupMessage";
            internal static readonly string LEAVE = "leave";
            internal static readonly string STAY = "stay";
        }
    }
}

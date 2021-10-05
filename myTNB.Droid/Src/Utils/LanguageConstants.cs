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

        internal struct Common
        {
            internal static readonly string GOT_IT = "gotIt";
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
        }
    }
}

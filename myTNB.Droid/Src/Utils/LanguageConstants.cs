namespace myTNB_Android.Src.Utils
{
    public class LanguageConstants
    {
        internal static readonly string BILLS = "Bills";
        internal static readonly string COMMON = "Common";
        internal static readonly string ERROR = "Error";

        internal struct Common
        {
            internal static readonly string GOT_IT = "gotIt";
        }

        internal struct Error
        {
            internal static readonly string RWDS_UNAVAILABLE_TITLE = "rewardsUnavailableTitle";
            internal static readonly string RWDS_UNAVAILABLE_MSG = "rewardsUnavailableMsg";
        }

        internal struct Bills
        {
            internal static readonly string TOOLTIP_ACT_STMT_TITLE = "accountStatementTenantTitle";
            internal static readonly string TOOLTIP_ACT_STMT_MSG = "accountStatementTenantMessage";
        }
    }
}

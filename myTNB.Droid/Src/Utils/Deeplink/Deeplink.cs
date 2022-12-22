namespace myTNB_Android.Src.Utils.Deeplink
{
    public class Deeplink
    {
        public class Constants
        {
            internal static readonly string WhatsNewIDKey = "wnid";
            internal static readonly string RewardsIDKey = "rid";
            internal static readonly string GetBillIDKey = "CA";
            internal static readonly string UserIDKey = "userID";
            internal static readonly string eKYCIsContractorAppliedKey = "isContractorApplied";
            internal static readonly string eKYCAppRefKey = "appRef";
            internal static readonly string identificationType = "idType";
            internal static readonly string identificationNo = "idNo";
            internal static readonly string applicationModuleID = "applicationModuleID";
            internal static readonly string email = "email";


            internal static readonly string Pattern = "\\b{0}.*\\b";
            internal static readonly string ReplaceKey = "{0}=";
            internal static readonly string Slash = "/";
            internal static readonly string AmperSand = "&";
        }

        public enum ScreenEnum
        {
            Rewards,
            WhatsNew,
            ApplicationListing,
            ApplicationDetails,
            OvervoltageClaimDetails,
            QR,
            GetBill,
            ManageBillDelivery,
            IdentityVerification,
            None
        }
    }
}

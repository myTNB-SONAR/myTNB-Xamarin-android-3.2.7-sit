namespace myTNB_Android.Src.Utils.Deeplink
{
    public class Deeplink
    {
        public class Constants
        {
            internal static readonly string WhatsNewIDKey = "wnid";
            internal static readonly string RewardsIDKey = "rid";
            internal static readonly string GetBillIDKey = "CA";

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
            None
        }
    }
}

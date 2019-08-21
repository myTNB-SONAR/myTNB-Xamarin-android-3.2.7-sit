using myTNB_Android.Src.myTNBMenu.Models;

namespace myTNB_Android.Src.Utils
{
    public class InnerDashboardAPICahceUtil
    {
        private static SMRActivityInfoResponse smrResponse;

        private static GetInstallationDetailsResponse accountStatusResponse;

        public static void OnSetSMRActivityInfoResponse(SMRActivityInfoResponse response)
        {
            smrResponse = response;
        }

        public static SMRActivityInfoResponse OnGetSMRActivityInfoResponse()
        {
            return smrResponse;
        }

        public static void OnSetAccountStatusResponse(GetInstallationDetailsResponse response)
        {
            accountStatusResponse = response;
        }

        public static GetInstallationDetailsResponse OnGetAccountStatusResponse()
        {
            return accountStatusResponse;
        }

    }
}
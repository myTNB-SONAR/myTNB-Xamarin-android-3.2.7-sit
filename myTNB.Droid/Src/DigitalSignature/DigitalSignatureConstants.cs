namespace myTNB.AndroidApp.Src.DigitalSignature
{
    public class DigitalSignatureConstants
    {
        internal static readonly string DS_LANDING_SELECTOR = "DSLanding";
        internal static readonly string DS_ID_TYPE = "idType";
        internal static readonly string DS_IDENTIFICATION_MODEL = "identificationModel";
        internal static readonly string DS_EKYC_START = "ekyc/startekyc";
        internal static readonly string DS_EKYC_SUCCESS = "ekyc/success";
        internal static readonly string DS_EKYC_ERROR = "ekyc/error";
        internal static readonly string DS_ACT_BACK_TO_APP = "mytnbapp://action=backtoapp";
        internal static readonly string DS_ACT_BACK_TO_HOME = "mytnbapp://action=backtohome";
        internal static readonly string DS_DYNAMIC_LINK_PARAMS_MODEL = "dsDynamicLinkParamsModel";
        internal static readonly string EKYC_SET_APPOINTMENT_URL = "https://www.mytnb.com.my/tnbtemujanji";
        internal static readonly string EKYC_STATUS_PENDING = "PENDING";

        public struct EKYCNotifType
        {
            public const string EKYC_VERIFY_FIRST_NOTIF = "EKYCFIRSTNOTIFICATION";
            public const string EKYC_VERIFY_SECOND_NOTIF = "EKYCSECONDNOTIFICATION";
            public const string EKYC_SUCCESS = "EKYCVERIFICATIONSUCCESS";
            public const string EKYC_FAILED = "EKYCFAILED";
            public const string EKYC_THREE_TIMES_FAILURE = "EKYCTHREETIMESFAILURE";
            public const string EKYC_NOTIF_ID_NOT_MATCH = "EKYCIDNOTMATCHING";
        }
    }
}

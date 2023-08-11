namespace myTNB_Android.Src.Utils.Notification
{
    public class Notification
    {
        public class Constants
        {
            internal static readonly string TYPE = "Type";
            internal static readonly string NOTIFICATION_TYPE = "NotificationType";
            internal static readonly string PUSH_MAP_ID = "PushMapId";
            internal static readonly string ACCOUNT_NUMBER = "AccountNumber";
            internal static readonly string EMAIL = "Email";
        }

        public enum TypeEnum
        {
            AppUpdate,
            AccountStatement,
            NewBillDesign,
            NCAddressSearchCompleted,
            NCResumeApplication,
            NCApplicationCompleted,
            NCApplicationContractorCompleted,
            NCOTPVerify,
            EKYC,
            ApplicationStatus,
            None
        }
    }
}

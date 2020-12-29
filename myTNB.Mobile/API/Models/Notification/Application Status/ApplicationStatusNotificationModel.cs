namespace myTNB.Mobile
{
    public class ApplicationStatusNotificationModel
    {
        public string SaveApplicationID { set; get; } = string.Empty;
        public string ApplicationID { set; get; } = string.Empty;
        public string ApplicationType { set; get; } = string.Empty;

        public static string TYPE_APPLICATIONSTATUS = "APPLICATIONSTATUS";
        public static string TYPE_SAVEAPPLICATIONID = "SaveApplicationId";
        public static string TYPE_APPLICATIONID = "ApplicationID";
        public static string TYPE_APPLICATIONTYPE = "ApplicationType";
    }
}
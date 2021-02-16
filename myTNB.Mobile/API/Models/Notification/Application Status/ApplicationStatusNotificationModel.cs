namespace myTNB.Mobile
{
    public class ApplicationStatusNotificationModel
    {
        public string SaveApplicationID { set; get; } = string.Empty;
        public string ApplicationID { set; get; } = string.Empty;
        public string ApplicationType { set; get; } = string.Empty;
        public string System { set; get; } = string.Empty;

        public static string TYPE_APPLICATIONSTATUS = "APPLICATIONSTATUS";
        public static string Param_SAVEAPPLICATIONID = "SaveApplicationId";
        public static string Param_APPLICATIONID = "ApplicationID";
        public static string Param_APPLICATIONTYPE = "ApplicationType";
        public static string Param_System = "System";
    }
}
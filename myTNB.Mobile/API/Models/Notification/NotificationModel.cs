namespace myTNB.Mobile
{
    public class NotificationModel
    {
        public string Type { set; get; } = string.Empty;  // NotificationTypeID
        public string RequestTransId { set; get; } = string.Empty;  //NotificationRequestId
        public string EventId { set; get; } = string.Empty; // BCRMNotificationTypeID

        public static string TYPE = "Type";
        public static string PARAM_REQUESTTRANSID = "RequestTransId";
        public static string Param_EVENTID = "EventId";
    }
}
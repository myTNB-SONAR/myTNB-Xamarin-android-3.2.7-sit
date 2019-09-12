namespace myTNB.Model
{
    public class NotificationDetailedInfoResponseModel
    {
        public NotificationDetailedInfoModel d { set; get; } = new NotificationDetailedInfoModel();
    }

    public class NotificationDetailedInfoModel : BaseModelV2
    {
        public NotificationDetailedInfoData data { set; get; } = new NotificationDetailedInfoData();
    }

    public class NotificationDetailedInfoData
    {
        public UserNotificationDataModel UserNotification { set; get; } = new UserNotificationDataModel();
    }
}
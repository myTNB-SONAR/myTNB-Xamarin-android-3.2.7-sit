namespace myTNB.Model
{
    public class UserNotificationDataModel
    {
        public string Id { set; get; }
        public string Email { set; get; }
        public string DeviceId { set; get; }
        public string AccountNum { set; get; }
        public string Title { set; get; }
        public string Message { set; get; }
        public string IsRead { set; get; }
        public string IsDeleted { set; get; }
        public string NotificationTypeId { set; get; }
        public string BCRMNotificationTypeId { set; get; }
        public string CreatedDate { set; get; }
        public AccountDetailsModel AccountDetails { set; get; }
        public string NotificationTitle { set; get; }
    }
}
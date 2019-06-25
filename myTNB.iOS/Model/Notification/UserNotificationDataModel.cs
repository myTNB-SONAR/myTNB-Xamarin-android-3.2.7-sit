using myTNB.Enums;
using Newtonsoft.Json;

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
        public string NotificationType { set; get; }
        public string Target { set; get; }

        [JsonIgnore]
        public BCRMNotificationEnum BCRMNotificationType
        {
            get
            {
                BCRMNotificationEnum notificationType = default(BCRMNotificationEnum);

                if (!string.IsNullOrEmpty(BCRMNotificationTypeId))
                {
                    switch (BCRMNotificationTypeId)
                    {
                        case "01":
                            notificationType = BCRMNotificationEnum.NewBill;
                            break;
                        case "02":
                            notificationType = BCRMNotificationEnum.BillDue;
                            break;
                        case "03":
                            notificationType = BCRMNotificationEnum.Dunning;
                            break;
                        case "04":
                            notificationType = BCRMNotificationEnum.Disconnection;
                            break;
                        case "05":
                            notificationType = BCRMNotificationEnum.Reconnection;
                            break;
                        case "97":
                            notificationType = BCRMNotificationEnum.Promotion;
                            break;
                        case "98":
                            notificationType = BCRMNotificationEnum.News;
                            break;
                        case "99":
                            notificationType = BCRMNotificationEnum.Maintenance;
                            break;
                    }
                }

                return notificationType;
            }
        }

        [JsonIgnore]
        public bool IsSelected { set; get; }
    }
}
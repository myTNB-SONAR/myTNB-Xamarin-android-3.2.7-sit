using myTNB.AndroidApp.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationDetails.Models
{
    public class UserDetailsNotificationData
    {
        
        private bool IsSelectOptionShown = false;
        private bool IsNotificationSelected = false;

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("AccountNum")]
        public string AccountNum { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("IsRead")]
        public bool IsRead { get; set; }

        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("NotificationTypeId")]
        public string NotificationTypeId { get; set; }

        [JsonProperty("BCRMNotificationTypeId")]
        public string BCRMNotificationTypeId { get; set; }

        [JsonProperty("NotificationRequestId")]
        public string NotificationRequestId { get; set; }

        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("NotificationType")]
        public string NotificationType { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("ODNBatchSubcategory")]
        public string ODNBatchSubcategory { get; set; }

        [JsonProperty("isForceDisplay")]
        public bool IsForceDisplay { get; set; }



        public bool ShowSelectButton
        {
            get
            {
                return IsSelectOptionShown;
            }
            set
            {
                IsSelectOptionShown = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return IsNotificationSelected;
            }
            set
            {
                IsNotificationSelected = value;
            }
        }

        public static UserDetailsNotificationData Get(UserNotificationEntity userNotification, string Code)
        {
            return new UserDetailsNotificationData()
            {
                Id = userNotification.Id,
                Email = userNotification.Email,
                DeviceId = userNotification.DeviceId,
                AccountNum = userNotification.AccountNum,
                Title = userNotification.Title,
                Message = userNotification.Message,
                IsRead = userNotification.IsRead,
                IsDeleted = userNotification.IsDeleted,
                NotificationTypeId = userNotification.NotificationTypeId,
                BCRMNotificationTypeId = userNotification.BCRMNotificationTypeId,
                CreatedDate = userNotification.CreatedDate,
                Code = Code,
                NotificationType = userNotification.NotificationType,
                Target = userNotification.Target,
                ODNBatchSubcategory = userNotification.ODNBatchSubcategory,
                IsForceDisplay = userNotification.IsForceDisplay
            };
        }
    }
}
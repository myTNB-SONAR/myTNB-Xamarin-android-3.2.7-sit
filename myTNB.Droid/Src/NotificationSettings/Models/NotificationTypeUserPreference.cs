using myTNB.Android.Src.Database.Model;

namespace myTNB.Android.Src.SelectNotification.Models
{
    public class NotificationTypeUserPreference
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string PreferenceMode { get; set; }

        public string Type { get; set; }

        public string CreatedDate { get; set; }

        public string MasterId { get; set; }

        public bool IsOpted { get; set; }

        public static NotificationTypeUserPreference Get(UserNotificationTypesEntity entity)
        {
            return new NotificationTypeUserPreference()
            {
                Id = entity.Id,
                Title = entity.Title,
                PreferenceMode = entity.PreferenceMode,
                Type = entity.Type,
                CreatedDate = entity.CreatedDate,
                MasterId = entity.MasterId,
                IsOpted = entity.IsOpted
            };
        }
    }
}
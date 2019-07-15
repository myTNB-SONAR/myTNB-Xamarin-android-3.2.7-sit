using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class MasterDataModel
    {
        [JsonProperty("AppVersionList")]
        public List<AppVersionModel> AppVersions { set; get; }

        [JsonProperty("Downtime")]
        public List<DowntimeDataModel> SystemStatus { set; get; }

        [JsonProperty("WebLink")]
        public List<WebLinksDataModel> WebLinks { set; get; }

        [JsonProperty("LocationType")]
        public List<LocationTypeDataModel> LocationTypes { set; get; }

        [JsonProperty("State")]
        public List<StatesForFeedbackDataModel> States { set; get; }

        [JsonProperty("FeedbackCategory_v2")]
        public List<FeedbackCategoryDataModel> FeedbackCategories { set; get; }

        [JsonProperty("FeedbackType")]
        public List<OtherFeedbackTypeDataModel> FeedbackTypes { set; get; }

        [JsonProperty("NotificationType")]
        public List<NotificationPreferenceModel> NotificationTypes { set; get; }

        [JsonProperty("NotificationTypeChannel")]
        public List<NotificationPreferenceModel> NotificationChannels { set; get; }

        [JsonProperty("MaintenanceTitle")]
        public string MaintenanceTitle { set; get; }

        [JsonProperty("MaintenanceMessage")]
        public string MaintenanceMessage { set; get; }

        [JsonProperty("ForceUpdateInfo")]
        public ForceUpdateInfoModel ForceUpdateInfo { set; get; }
    }
}
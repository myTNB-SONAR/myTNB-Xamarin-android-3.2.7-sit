using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Refit;
using static myTNB_Android.Src.FindUs.Response.GetLocationTypesResponse;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class MasterData
    {
        [JsonProperty(PropertyName = "AppVersionList")]
        [AliasAs("AppVersionList")]
        public List<AppVersionList> AppVersionList { get; set; }

        [JsonProperty(PropertyName = "Downtime")]
        [AliasAs("Downtime")]
        public List<DownTime> Downtimes { get; set; }

        [JsonProperty(PropertyName = "WebLink")]
        [AliasAs("WebLink")]
        public List<Weblink> WebLinks { get; set; }

        [JsonProperty(PropertyName = "LocationType")]
        [AliasAs("LocationType")]
        public List<LocationType> LocationTypes { get; set; }

        [JsonProperty(PropertyName = "State")]
        [AliasAs("State")]
        public List<FeedbackState> States { get; set; }

        [JsonProperty(PropertyName = "FeedbackCategory")]
        [AliasAs("FeedbackCategory")]
        public List<FeedbackCategory> FeedbackCategorys { get; set; }

        [JsonProperty(PropertyName = "FeedbackType")]
        [AliasAs("FeedbackType")]
        public List<FeedbackType> FeedbackTypes { get; set; }

        [JsonProperty(PropertyName = "NotificationType")]
        [AliasAs("NotificationType")]
        public List<NotificationTypes> NotificationTypes { get; set; }

        [JsonProperty(PropertyName = "NotificationTypeChannel")]
        [AliasAs("NotificationTypeChannel")]
        public List<NotificationChannels> NotificationTypeChannels { get; set; }

        [JsonProperty(PropertyName = "MaintenanceTitle")]
        [AliasAs("MaintenanceTitle")]
        public string MaintainanceTitle { get; set; }

        [JsonProperty(PropertyName = "MaintenanceMessage")]
        [AliasAs("MaintenanceMessage")]
        public string MaintainanceMessage { get; set; }
    }
}
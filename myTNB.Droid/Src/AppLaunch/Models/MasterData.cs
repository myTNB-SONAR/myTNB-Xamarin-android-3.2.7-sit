using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.FindUs.Response.GetLocationTypesResponse;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
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

        [JsonProperty(PropertyName = "FeedbackCategory_v2")]
        [AliasAs("FeedbackCategory_v2")]
        public List<FeedbackCategory> FeedbackCategorysV2 { get; set; }

        [JsonProperty(PropertyName = "FeedbackType")]
        [AliasAs("FeedbackType")]
        public List<FeedbackType> FeedbackTypes { get; set; }

        [JsonProperty(PropertyName = "NotificationType")]
        [AliasAs("NotificationType")]
        public List<NotificationTypes> NotificationTypes { get; set; }

        [JsonProperty(PropertyName = "NotificationTypeChannel")]
        [AliasAs("NotificationTypeChannel")]
        public List<NotificationChannels> NotificationTypeChannels { get; set; }

        [JsonProperty(PropertyName = "ForceUpdateInfo")]
        [AliasAs("ForceUpdateInfo")]
        public ForceUpdateInfoData ForceUpdateInfo { get; set; }
        //public ForceUpdateInfoData ForceUpdateInfo
        //{
        //    get{
        //        return new ForceUpdateInfoData();
        //    }
        //}


        public class ForceUpdateInfoData
        {
            //private bool _isAndroidForceUpdateOn;
            //private string _AndroidLatestVersion, _ModalTitle, _ModalBody, _ModalBtnText;

            [JsonProperty(PropertyName = "isAndroidForceUpdateOn")]
            [AliasAs("isAndroidForceUpdateOn")]
            public bool isAndroidForceUpdateOn { get; set; }

            [JsonProperty(PropertyName = "AndroidLatestVersion")]
            [AliasAs("AndroidLatestVersion")]
            public string AndroidLatestVersion { get; set; }

            [JsonProperty(PropertyName = "ModalTitle")]
            [AliasAs("ModalTitle")]
            public string ModalTitle { get; set; }

            [JsonProperty(PropertyName = "ModalBody")]
            [AliasAs("ModalBody")]
            public string ModalBody { get; set; }

            [JsonProperty(PropertyName = "ModalBtnText")]
            [AliasAs("ModalBtnText")]
            public string ModalBtnText { get; set; }

            //public ForceUpdateInfoData()
            //{
            //    _isAndroidForceUpdateOn = true;
            //    _AndroidLatestVersion = "58";
            //    _ModalTitle = "New Version Available (v1.1.8)";
            //    _ModalBody = "<p>Get the newest version of myTNB to enjoy a better experience</p>";
            //    _ModalBtnText = "Update Now";
            //}

            //public bool isAndroidForceUpdateOn
            //{
            //    get
            //    {
            //        return _isAndroidForceUpdateOn;
            //    }
            //}
            //public string AndroidLatestVersion
            //{
            //    get
            //    {
            //        return _AndroidLatestVersion;
            //    }
            //}
            //public string ModalTitle
            //{
            //    get
            //    {
            //        return _ModalTitle;
            //    }
            //}
            //public string ModalBody
            //{
            //    get
            //    {
            //        return _ModalBody;
            //    }
            //}
            //public string ModalBtnText
            //{
            //    get
            //    {
            //        return _ModalBtnText;
            //    }
            //}
        }
    }
}
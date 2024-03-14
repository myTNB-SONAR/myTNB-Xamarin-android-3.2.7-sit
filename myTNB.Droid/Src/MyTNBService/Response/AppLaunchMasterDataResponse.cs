using System;
using System.Collections.Generic;
using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.FindUs.Response;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.MyTNBService.Response
{
    public class AppLaunchMasterDataResponse : BaseResponseV2<AppLaunchMasterDataResponse.AppLaunchMasterDataModel>
    {
        public AppLaunchMasterDataModel GetData()
        {
            return Response.Data;
        }

        public class AppLaunchMasterDataModel
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
            public List<GetLocationTypesResponse.LocationType> LocationTypes { get; set; }

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

            //[JsonProperty(PropertyName = "IsFeedbackUpdateDetailDisabled")]
            //[AliasAs("IsFeedbackUpdateDetailDisabled")]
            //public bool IsFeedbackUpdateDetailDisabled { get; set; }

            [JsonProperty(PropertyName = "ServicesPreLogin")]
            [AliasAs("ServicesPreLogin")]
            public List<MyService> ServicesPreLogin { set; get; }
            
            [JsonProperty(PropertyName = "UserVerificationInfo")]
            [AliasAs("NotificatiUserVerificationInfoonType")]
            public UserVerificationInfo UserVerificationInfo { get; set; }

        }

        public class ForceUpdateInfoData
        {
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
        }

        public class UserVerificationInfo
        {
            [JsonProperty("Email")]
            public bool Email { get; set; }
        }
    }
}

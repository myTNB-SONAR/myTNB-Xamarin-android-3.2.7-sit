﻿using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.FindUs.Response;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    /*public class AppLaunchMasterDataBaseResponseAWS
    {
        [JsonProperty(PropertyName = "d")]
        public AppLaunchMasterDataResponseAWS Response { get; set; }

        public bool IsSuccessResponse()
        {
            bool IsSuccess = false;
            if (Response != null && Response.ErrorCode == Constants.SERVICE_CODE_SUCCESS)
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }

        public AppLaunchMasterDataResponseAWS.AppLaunchMasterDataModel GetData()
        {
            return Response.Data;
        }
    }*/

    public class AppLaunchMasterDataResponseAWS
    {
        [JsonProperty(PropertyName = "IdentificationNo")]
        public string IdentificationNo { get; set; }

        [JsonProperty(PropertyName = "data")]
        [AliasAs("data")]
        public AppLaunchMasterDataModel Data { get; set; }

        [JsonProperty(PropertyName = "IsSMRApplyDisabled")]
        public bool IsSMRApplyDisabled { get; set; }

        [JsonProperty(PropertyName = "IsEnergyTipsDisabled")]
        public bool IsEnergyTipsDisabled { get; set; }

        [JsonProperty(PropertyName = "IsOCRDown")]
        public bool IsOCRDown { get; set; }

        [JsonProperty(PropertyName = "IsSMRFeatureDisabled")]
        public bool IsSMRFeatureDisabled { get; set; }

        [JsonProperty(PropertyName = "IsRewardsDisabled")]
        public bool IsRewardsDisabled { get; set; }

        [JsonProperty(PropertyName = "IsPayEnabled")]
        public bool IsPayEnabled { get; set; }

        [JsonProperty(PropertyName = "IsLargeFontDisabled")]
        public bool IsLargeFontDisabled { get; set; }

        [JsonProperty(PropertyName = "IsApplicationSyncAPIEnable")]
        public bool IsApplicationSyncAPIEnable { get; set; }

        [JsonProperty(PropertyName = "ApplicationSyncAPIInterval")]
        public double ApplicationSyncAPIInterval { get; set; }

        [JsonProperty(PropertyName = "IsAppointmentDisabled")]
        public bool IsAppointmentDisabled { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "ErrorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayMessage")]
        public string DisplayMessage { get; set; }

        [JsonProperty(PropertyName = "DisplayType")]
        public string DisplayType { get; set; }

        [JsonProperty(PropertyName = "DisplayTitle")]
        public string DisplayTitle { get; set; }

        [JsonProperty(PropertyName = "RefreshTitle")]
        public string RefreshTitle { get; set; }

        [JsonProperty(PropertyName = "RefreshMessage")]
        public string RefreshMessage { get; set; }

        [JsonProperty(PropertyName = "RefreshBtnText")]
        public string RefreshBtnText { get; set; }

        [JsonProperty(PropertyName = "ShowWLTYPage")]
        public bool ShowWLTYPage { get; set; }

        [JsonProperty(PropertyName = "IsSMROpenToTenant")]
        public bool IsSMROpenToTenant { get; set; }

        [JsonProperty(PropertyName = "IsSMROpenToTenantV2")]
        public bool IsSMROpenToTenantV2 { get; set; }

        [JsonProperty(PropertyName = "IsWalkthroughAppUpdate")]
        public bool IsWalkthroughAppUpdate { get; set; }


        public class AppLaunchMasterDataModel : BaseResponseV2<AppLaunchMasterDataResponse.AppLaunchMasterDataModel>
        {
            [JsonProperty(PropertyName = "AppVersionList")]
            [AliasAs("AppVersionList")]
            public List<AppVersionList> AppVersionList { get; set; } = new List<AppVersionList>();

            [JsonProperty(PropertyName = "Downtime")]
            [AliasAs("Downtime")]
            public List<DownTime> Downtimes { get; set; } = new List<DownTime>();

            [JsonProperty(PropertyName = "WebLink")]
            [AliasAs("WebLink")]
            public List<Weblink> WebLinks { get; set; } = new List<Weblink>();

            [JsonProperty(PropertyName = "LocationType")]
            [AliasAs("LocationType")]
            public List<GetLocationTypesResponse.LocationType> LocationTypes { get; set; } = new List<GetLocationTypesResponse.LocationType>();

            [JsonProperty(PropertyName = "State")]
            [AliasAs("State")]
            public List<FeedbackState> States { get; set; } = new List<FeedbackState>();

            [JsonProperty(PropertyName = "FeedbackCategory")]
            [AliasAs("FeedbackCategory")]
            public List<FeedbackCategory> FeedbackCategorys { get; set; } = new List<FeedbackCategory>();

            [JsonProperty(PropertyName = "FeedbackCategory_v2")]
            [AliasAs("FeedbackCategory_v2")]
            public List<FeedbackCategory> FeedbackCategorysV2 { get; set; } = new List<FeedbackCategory>();

            [JsonProperty(PropertyName = "FeedbackType")]
            [AliasAs("FeedbackType")]
            public List<FeedbackType> FeedbackTypes { get; set; } = new List<FeedbackType>();

            [JsonProperty(PropertyName = "NotificationType")]
            [AliasAs("NotificationType")]
            public List<NotificationTypes> NotificationTypes { get; set; } = new List<NotificationTypes>();

            [JsonProperty(PropertyName = "NotificationTypeChannel")]
            [AliasAs("NotificationTypeChannel")]
            public List<NotificationChannels> NotificationTypeChannels { get; set; } = new List<NotificationChannels>();

            [JsonProperty(PropertyName = "ForceUpdateInfo")]
            [AliasAs("ForceUpdateInfo")]
            public ForceUpdateInfoData ForceUpdateInfo { get; set; }

            [JsonProperty(PropertyName = "RecommendUpdateInfo")]
            [AliasAs("RecommendUpdateInfo")]
            public RecommendUpdateInfo RecommendUpdateInfo { get; set; }

            [JsonProperty(PropertyName = "IsFeedbackUpdateDetailDisabled")]
            [AliasAs("IsFeedbackUpdateDetailDisabled")]
            public bool IsFeedbackUpdateDetailDisabled { get; set; }

            [JsonProperty(PropertyName = "ServicesPreLogin")]
            [AliasAs("ServicesPreLogin")]
            public List<MyService> ServicesPreLogin { set; get; } = new List<MyService>();

            //[JsonProperty(PropertyName = "UserVerificationInfo")]
            //[AliasAs("NotificatiUserVerificationInfoonType")]
            //public UserVerificationInfo UserVerificationInfo { get; set; }
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

        public class RecommendUpdateInfo
        {
            [JsonProperty(PropertyName = "isAndroidRecommendUpdateOn")]
            [AliasAs("isAndroidRecommendUpdateOn")]
            public bool isAndroidRecommendUpdateOn { get; set; }

            [JsonProperty(PropertyName = "RecommendUpdatePopUpDayDelay")]
            [AliasAs("RecommendUpdatePopUpDayDelay")]
            public int RecommendUpdatePopUpDayDelay { get; set; }

            [JsonProperty(PropertyName = "EndDateTimeRecommendUpdate")]
            [AliasAs("EndDateTimeRecommendUpdate")]
            public string EndDateTimeRecommendUpdate { get; set; }

            [JsonProperty(PropertyName = "RecommendUpdatePopUpCount")]
            [AliasAs("RecommendUpdatePopUpCount")]
            public int RecommendUpdatePopUpCount { get; set; }

            [JsonProperty(PropertyName = "PublishDateTimeRecommendUpdate")]
            [AliasAs("PublishDateTimeRecommendUpdate")]
            public string PublishDateTimeRecommendUpdate { get; set; }

            [JsonProperty(PropertyName = "rangeAndroidRecommendUpdate")]
            [AliasAs("rangeAndroidRecommendUpdate")]
            public bool rangeAndroidRecommendUpdate { get; set; }

            [JsonProperty(PropertyName = "AndroidLastRecommendVersion")]
            [AliasAs("AndroidLastRecommendVersion")]
            public string AndroidLastRecommendVersion { get; set; }

            [JsonProperty(PropertyName = "AndroidLatestRecommendVersion")]
            [AliasAs("AndroidLatestRecommendVersion")]
            public string AndroidLatestRecommendVersion { get; set; }

            [JsonProperty(PropertyName = "selectAndroidRecommendUpdate")]
            [AliasAs("selectAndroidRecommendUpdate")]
            public bool selectAndroidRecommendUpdate { get; set; }

            [JsonProperty(PropertyName = "AndroidVersionToUpdate")]
            [AliasAs("AndroidVersionToUpdate")]
            public List<string> AndroidVersionToUpdate { get; set; } = new List<string>();

            [JsonProperty(PropertyName = "ModalRecommendTitle")]
            [AliasAs("ModalRecommendTitle")]
            public string ModalRecommendTitle { get; set; }

            [JsonProperty(PropertyName = "ModalRecommendBody")]
            [AliasAs("ModalRecommendBody")]
            public string ModalRecommendBody { get; set; }

            [JsonProperty(PropertyName = "ModalRecommendBtnYesText")]
            [AliasAs("ModalRecommendBtnYesText")]
            public string ModalRecommendBtnYesText { get; set; }

            [JsonProperty(PropertyName = "ModalRecommendBtnNoText")]
            [AliasAs("ModalRecommendBtnNoText")]
            public string ModalRecommendBtnNoText { get; set; }
        }
    }
}

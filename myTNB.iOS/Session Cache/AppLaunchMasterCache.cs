using System;
using System.Collections.Generic;
using myTNB.Enums;
using myTNB.Model;

namespace myTNB
{
    public sealed class AppLaunchMasterCache
    {
        private static readonly Lazy<AppLaunchMasterCache> lazy = new Lazy<AppLaunchMasterCache>(() => new AppLaunchMasterCache());
        public static AppLaunchMasterCache Instance { get { return lazy.Value; } }

        private static AppLaunchResponseModel response = new AppLaunchResponseModel();
        private static AppLaunchMasterDataModel dataModel = new AppLaunchMasterDataModel();
        private static MasterDataModel masterData = new MasterDataModel();

        public static void AddAppLaunchResponseData(AppLaunchResponseModel resp)
        {
            if (response == null)
            {
                response = new AppLaunchResponseModel();
            }
            if (dataModel == null)
            {
                dataModel = new AppLaunchMasterDataModel();
            }
            if (masterData == null)
            {
                masterData = new MasterDataModel();
            }
            response = resp;
            if (response != null && response.d != null &&
                response.d.data != null && response.d.IsSuccess)
            {
                dataModel = response.d;
                masterData = response.d.data;
                IsSuccess = response.d.IsSuccess;

                DataManager.DataManager.SharedInstance.IsFeedbackUpdateDetailDisabled = dataModel.IsFeedbackUpdateDetailDisabled;
                DataManager.DataManager.SharedInstance.LatestAppVersion = masterData?.ForceUpdateInfo?.iOSLatestVersion;
                DataManager.DataManager.SharedInstance.SystemStatus = masterData?.SystemStatus ?? new List<DowntimeDataModel>();
                DataManager.DataManager.SharedInstance.SetSystemsAvailability();
                DataManager.DataManager.SharedInstance.WebLinks = masterData?.WebLinks ?? new List<WebLinksDataModel>();

                DataManager.DataManager.SharedInstance.LocationTypes = masterData?.LocationTypes ?? new List<LocationTypeDataModel>();
                if (masterData?.LocationTypes != null)
                {
                    LocationTypeDataModel allLocationModel = new LocationTypeDataModel();
                    allLocationModel.Id = "all";
                    allLocationModel.Title = "All";
                    allLocationModel.Description = "All";
                    if (DataManager.DataManager.SharedInstance.LocationTypes != null)
                    {
                        DataManager.DataManager.SharedInstance.LocationTypes.Insert(0, allLocationModel);
                    }
                }

                DataManager.DataManager.SharedInstance.StatesForFeedBack = masterData?.States ?? new List<StatesForFeedbackDataModel>();
                DataManager.DataManager.SharedInstance.FeedbackCategory = masterData?.FeedbackCategories ?? new List<FeedbackCategoryDataModel>();
                DataManager.DataManager.SharedInstance.OtherFeedbackType = masterData?.FeedbackTypes ?? new List<OtherFeedbackTypeDataModel>();

                var rawNotifGeneralTypes = masterData?.NotificationTypes ?? new List<NotificationPreferenceModel>();
                DataManager.DataManager.SharedInstance.NotificationGeneralTypes = rawNotifGeneralTypes.FindAll(item => item?.ShowInFilterList?.ToLower() == "true") ?? new List<NotificationPreferenceModel>();

                if (masterData?.NotificationTypes != null)
                {
                    NotificationPreferenceModel allNotificationItem = new NotificationPreferenceModel();
                    allNotificationItem.Title = LanguageUtility.GetCommonI18NValue(Constants.Common_AllNotifications);
                    allNotificationItem.Id = "all";
                    if (DataManager.DataManager.SharedInstance.NotificationGeneralTypes != null)
                    {
                        DataManager.DataManager.SharedInstance.NotificationGeneralTypes.Insert(0, allNotificationItem);
                    }
                }
            }
        }

        public static AppLaunchResponseModel GetAppLaunchResponse()
        {
            if (response != null)
            {
                return response;
            }
            return new AppLaunchResponseModel();
        }

        public static MasterDataModel GetAppLaunchMasterData()
        {
            if (masterData != null)
            {
                return masterData;
            }
            return new MasterDataModel();
        }

        public static bool IsSMRApplyDisabled
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsSMRApplyDisabled;
                }
                return true;
            }
        }

        public static bool IsEnergyTipsDisabled
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsEnergyTipsDisabled;
                }
                return true;
            }
        }

        public static bool IsOCRDown
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsOCRDown;
                }
                return true;
            }
        }

        public static bool IsPayEnabled
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsPayEnabled;
                }
                return true;
            }
        }

        public static bool IsRewardsDisabled
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsRewardsDisabled;
                }
                return true;
            }
        }

        public static bool IsSuccess
        {
            private set;
            get;
        } = false;

        public static bool IsSMRFeatureDisabled
        {
            get
            {
                if (dataModel != null)
                {
                    return dataModel.IsSMRFeatureDisabled;
                }
                return true;
            }
        }

        public static bool IsPayDisabled
        {
            get
            {
                if (dataModel != null && dataModel.data != null && dataModel.data.SystemStatus != null && dataModel.data.SystemStatus.Count > 0)
                {
                    DowntimeDataModel bcrm = dataModel.data.SystemStatus.Find(x => x.SystemType == SystemEnum.BCRM);
                    DowntimeDataModel cc = dataModel.data.SystemStatus.Find(x => x.SystemType == SystemEnum.PaymentCreditCard);
                    DowntimeDataModel fpx = dataModel.data.SystemStatus.Find(x => x.SystemType == SystemEnum.PaymentFPX);
                    if (cc != null && fpx != null && bcrm != null)
                    {
                        if (!bcrm.IsAvailable)
                        {
                            return true;
                        }
                        else
                        {
                            bool pgDisable = !cc.IsAvailable && !fpx.IsAvailable;
                            return pgDisable;
                        }
                    }
                }
                return false;
            }
        }

        private static bool _isBCRMPopupDisplayed;
        public static bool IsBCRMPopupDisplayed
        {
            set
            {
                _isBCRMPopupDisplayed = value;
            }
            get
            {
                DowntimeDataModel bcrm = dataModel.data.SystemStatus.Find(x => x.SystemType == SystemEnum.BCRM);
                if (bcrm == null || bcrm.IsAvailable)
                {
                    return true;
                }
                return _isBCRMPopupDisplayed;
            }
        }
    }
}
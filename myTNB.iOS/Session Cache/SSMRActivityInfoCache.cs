using System;
using System.Collections.Generic;
using Force.DeepCloner;
using Foundation;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRActivityInfoCache
    {
        private static readonly Lazy<SSMRActivityInfoCache> lazy = new Lazy<SSMRActivityInfoCache>(() => new SSMRActivityInfoCache());
        public static SSMRActivityInfoCache Instance { get { return lazy.Value; } }

        private static List<PopupModel> PopupDetailList = new List<PopupModel>();
        private static Dictionary<string, List<PopupSelectorModel>> SMRPhotoPopUpDetails;
        private static List<PopupSelectorModel> SMRPhotoPopUpList;

        //Dashboard Cache
        private static SMRAccountActivityInfoDataModel DB_DataModel = new SMRAccountActivityInfoDataModel();
        private static MeterReadingHistoryModel DB_MeterReadingHistory = new MeterReadingHistoryModel();
        private static List<MeterReadingHistoryItemModel> DB_ReadingHistoryList = new List<MeterReadingHistoryItemModel>();
        private static List<SMRMROValidateRegisterDetailsInfoModel> DB_SSMRPreviousMeterReadingList = new List<SMRMROValidateRegisterDetailsInfoModel>();

        //Reading History Cache
        private static SMRAccountActivityInfoDataModel RH_DataModel = new SMRAccountActivityInfoDataModel();
        private static MeterReadingHistoryModel RH_MeterReadingHistory = new MeterReadingHistoryModel();
        private static List<MeterReadingHistoryItemModel> RH_ReadingHistoryList = new List<MeterReadingHistoryItemModel>();
        private static List<SMRMROValidateRegisterDetailsInfoModel> RH_SSMRPreviousMeterReadingList = new List<SMRMROValidateRegisterDetailsInfoModel>();

        private static readonly string SelectorKey = SSMR.SSMRConstants.Pagename_SSMRCaptureMeter;
        private static readonly string PopupKey = SSMR.SSMRConstants.Popup_SMRPhotoPopUpDetails;
        private static readonly string TakePhotoToolTipKey = "TakePhotoKey";

        public static void SetDashboardCache(SMRAccountActivityInfoResponseModel data, CustomerAccountRecordModel account)
        {
            DB_DataModel = data.d.DeepClone();
            DB_MeterReadingHistory = data.d.data.DeepClone();
            DB_ReadingHistoryList = data.d.data.MeterReadingHistory.DeepClone();
            DB_SSMRPreviousMeterReadingList = data.d.data.SMRMROValidateRegisterDetails.DeepClone();
            DashboardAccount = account;
            PopupDetailList = data.d.data.SMRPhotoPopUpDetails.DeepClone();
        }

        public static void SetReadingHistoryCache(SMRAccountActivityInfoResponseModel data, CustomerAccountRecordModel account)
        {
            RH_DataModel = data.d.DeepClone();
            RH_MeterReadingHistory = data.d.data.DeepClone();
            RH_ReadingHistoryList = data.d.data.MeterReadingHistory.DeepClone();
            RH_SSMRPreviousMeterReadingList = data.d.data.SMRMROValidateRegisterDetails.DeepClone();
            ViewHistoryAccount = account;
            PopupDetailList = data.d.data.SMRPhotoPopUpDetails.DeepClone();
        }

        public static CustomerAccountRecordModel SubmittedAccount { set; get; }

        public static CustomerAccountRecordModel DashboardAccount { get; private set; } = new CustomerAccountRecordModel();

        public static CustomerAccountRecordModel ViewHistoryAccount { get; private set; } = new CustomerAccountRecordModel();

        public static SMRAccountActivityInfoDataModel DashboardDataModel
        {
            get
            {
                return DB_DataModel ?? new SMRAccountActivityInfoDataModel();
            }
        }

        public static SMRAccountActivityInfoDataModel ViewDataModel
        {
            get
            {
                return RH_DataModel ?? new SMRAccountActivityInfoDataModel();
            }
        }

        public static MeterReadingHistoryModel DashboardMeterReadingHistory
        {
            get
            {
                return DB_MeterReadingHistory != null ? DB_MeterReadingHistory : new MeterReadingHistoryModel();
            }
        }

        public static MeterReadingHistoryModel ViewMeterReadingHistory
        {
            get
            {
                return RH_MeterReadingHistory != null ? RH_MeterReadingHistory : new MeterReadingHistoryModel();
            }
        }

        public static List<MeterReadingHistoryItemModel> DashboardReadingHistoryList
        {
            get
            {
                return DB_ReadingHistoryList != null ? DB_ReadingHistoryList : new List<MeterReadingHistoryItemModel>();
            }
        }

        public static List<MeterReadingHistoryItemModel> ViewReadingHistoryList
        {
            get
            {
                return RH_ReadingHistoryList != null ? RH_ReadingHistoryList : new List<MeterReadingHistoryItemModel>();
            }
        }

        public static List<SMRMROValidateRegisterDetailsInfoModel> DashboardPreviousReading
        {
            get
            {
                return DB_SSMRPreviousMeterReadingList != null ? DB_SSMRPreviousMeterReadingList : new List<SMRMROValidateRegisterDetailsInfoModel>();
            }
        }

        public static List<SMRMROValidateRegisterDetailsInfoModel> ViewPreviousReading
        {
            get
            {
                return RH_SSMRPreviousMeterReadingList != null ? RH_SSMRPreviousMeterReadingList : new List<SMRMROValidateRegisterDetailsInfoModel>();
            }
        }

        #region Popup
        public static PopupModel GetPopupDetailsByType(string type)
        {
            SetPopupSelectorValues();
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrWhiteSpace(type))
            {
                PopupSelectorModel fallback = GetFallbackPopupValue(type);
                if (PopupDetailList != null)
                {
                    int index = PopupDetailList.FindIndex(x => x.Type.ToLower() == type.ToLower());
                    if (index > -1)
                    {
                        PopupModel popupDetails = PopupDetailList[index];
                        return new PopupModel
                        {
                            Title = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Title)
                                ? popupDetails.Title : fallback.Title,
                            Description = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Description)
                                ? popupDetails.Description : fallback.Description,
                            CTA = popupDetails != null && !string.IsNullOrEmpty(popupDetails.CTA)
                                ? popupDetails.CTA : fallback.CTA,
                            Type = popupDetails != null && !string.IsNullOrEmpty(popupDetails.Type)
                                ? popupDetails.Type : fallback.Type
                        };
                    }
                }
                else
                {
                    return new PopupModel
                    {
                        Title = fallback.Title,
                        Description = fallback.Description,
                        CTA = fallback.CTA,
                        Type = fallback.Type
                    };
                }
            }
            return null;
        }

        private static void SetPopupSelectorValues()
        {
            if (SMRPhotoPopUpDetails == null || SMRPhotoPopUpDetails.Count == 0
                || SMRPhotoPopUpList == null || SMRPhotoPopUpList.Count == 0)
            {
                SMRPhotoPopUpDetails = LanguageManager.Instance.GetPopupSelectorsByPage(SelectorKey);
                if (SMRPhotoPopUpDetails.ContainsKey(PopupKey))
                {
                    SMRPhotoPopUpList = SMRPhotoPopUpDetails[PopupKey];
                }
            }
        }

        private static PopupSelectorModel GetFallbackPopupValue(string type)
        {
            if (SMRPhotoPopUpList != null || SMRPhotoPopUpList.Count > 0)
            {
                PopupSelectorModel item = SMRPhotoPopUpList.Find(x => x.Type == type);
                if (item != null)
                {
                    return item;
                }
            }
            return new PopupSelectorModel();
        }

        public static bool IsPhotoToolTipDisplayed
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(value, TakePhotoToolTipKey);
                sharedPreference.Synchronize();
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey(TakePhotoToolTipKey);
            }
        }

        public static bool RH_IsOCRDown
        {
            get
            {
                if (RH_MeterReadingHistory != null)
                {
                    return RH_MeterReadingHistory.IsOCRDown;
                }
                return true;
            }
        }

        public static bool RH_IsOCRDisabled
        {
            get
            {
                if (RH_MeterReadingHistory != null)
                {
                    return RH_MeterReadingHistory.IsOCRDisabled;
                }
                return true;
            }
        }

        public static bool DB_IsOCRDown
        {
            get
            {
                if (DB_MeterReadingHistory != null)
                {
                    return DB_MeterReadingHistory.IsOCRDown;
                }
                return true;
            }
        }

        public static bool DB_IsOCRDisabled
        {
            get
            {
                if (DB_MeterReadingHistory != null)
                {
                    return DB_MeterReadingHistory.IsOCRDisabled;
                }
                return true;
            }
        }
        #endregion
    }
}
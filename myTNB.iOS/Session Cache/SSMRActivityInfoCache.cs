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

        private static SMRAccountActivityInfoResponseModel SSMRActivityInfoResponse = new SMRAccountActivityInfoResponseModel();
        private static List<SMRMROValidateRegisterDetailsInfoModel> SSMRPreviousMeterReadingList;
        private static List<PopupModel> PopupDetailList = new List<PopupModel>();
        private static Dictionary<string, List<PopupSelectorModel>> SMRPhotoPopUpDetails;
        private static List<PopupSelectorModel> SMRPhotoPopUpList;

        private static readonly string SelectorKey = SSMR.SSMRConstants.Pagename_SSMRCaptureMeter;
        private static readonly string PopupKey = SSMR.SSMRConstants.Popup_SMRPhotoPopUpDetails;
        private static readonly string TakePhotoToolTipKey = "TakePhotoKey";

        public static void SetData(SMRAccountActivityInfoResponseModel data)
        {
            SSMRActivityInfoResponse = data;
            PopupDetailList = data.d.data.SMRPhotoPopUpDetails;
            SSMRPreviousMeterReadingList = data.d.data.SMRMROValidateRegisterDetails;
        }

        public static PopupModel GetPopupDetailsByType(string type)
        {
            SetPopupSelectorValues();
            PopupSelectorModel fallback = GetFallbackPopupValue(type);
            if (PopupDetailList != null && !string.IsNullOrEmpty(type) && !string.IsNullOrWhiteSpace(type))
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

        public static List<SMRMROValidateRegisterDetailsInfoModel> GetPreviousMeterReadingList()
        {
            List<SMRMROValidateRegisterDetailsInfoModel> list = new List<SMRMROValidateRegisterDetailsInfoModel>();
            if (SSMRPreviousMeterReadingList != null && SSMRPreviousMeterReadingList.Count > 0)
            {
                for (int i = 0; i < SSMRPreviousMeterReadingList.Count; i++)
                {
                    SMRMROValidateRegisterDetailsInfoModel item = SSMRPreviousMeterReadingList[i];
                    if (item == null) { continue; }
                    list.Add(item.DeepClone());
                }
            }
            return list;
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
    }
}
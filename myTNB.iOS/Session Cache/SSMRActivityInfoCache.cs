using System;
using System.Collections.Generic;
using Force.DeepCloner;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRActivityInfoCache
    {
        private static readonly Lazy<SSMRActivityInfoCache> lazy = new Lazy<SSMRActivityInfoCache>(() => new SSMRActivityInfoCache());
        public static SSMRActivityInfoCache Instance { get { return lazy.Value; } }

        private SMRAccountActivityInfoResponseModel SSMRActivityInfoResponse = new SMRAccountActivityInfoResponseModel();
        private List<SMRMROValidateRegisterDetailsInfoModel> SSMRPreviousMeterReadingList;
        private List<PopupModel> PopupDetailList = new List<PopupModel>();
        private Dictionary<string, List<PopupSelectorModel>> SMRPhotoPopUpDetails;
        private List<PopupSelectorModel> SMRPhotoPopUpList;

        private readonly string SelectorKey = SSMR.SSMRConstants.Pagename_SSMRCaptureMeter;
        private readonly string PopupKey = SSMR.SSMRConstants.Popup_SMRPhotoPopUpDetails;

        public void SetData(SMRAccountActivityInfoResponseModel data)
        {
            SSMRActivityInfoResponse = data;
            PopupDetailList = data.d.data.SMRPhotoPopUpDetails;
            SSMRPreviousMeterReadingList = data.d.data.SMRMROValidateRegisterDetails;
        }

        public PopupModel GetPopupDetailsByType(string type)
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

        public List<SMRMROValidateRegisterDetailsInfoModel> GetPreviousMeterReadingList()
        {
            List<SMRMROValidateRegisterDetailsInfoModel> list = new List<SMRMROValidateRegisterDetailsInfoModel>();
            if (SSMRPreviousMeterReadingList != null && SSMRPreviousMeterReadingList.Count > 0)
            {
                list = SSMRPreviousMeterReadingList.DeepClone();
            }
            return list;
        }

        private void SetPopupSelectorValues()
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

        private PopupSelectorModel GetFallbackPopupValue(string type)
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
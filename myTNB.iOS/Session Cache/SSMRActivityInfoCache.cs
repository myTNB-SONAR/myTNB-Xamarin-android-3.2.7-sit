using System;
using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public sealed class SSMRActivityInfoCache
    {
        private static readonly Lazy<SSMRActivityInfoCache> lazy = new Lazy<SSMRActivityInfoCache>(() => new SSMRActivityInfoCache());
        public static SSMRActivityInfoCache Instance { get { return lazy.Value; } }

        private SMRAccountActivityInfoResponseModel SSMRActivityInfoResponse = new SMRAccountActivityInfoResponseModel();
        private List<PopupModel> PopupDetailList = new List<PopupModel>();

        public void SetData(SMRAccountActivityInfoResponseModel data)
        {
            SSMRActivityInfoResponse = data;
            PopupDetailList = data.d.data.SMRPhotoPopUpDetails;
        }

        public PopupModel GetPopupDetailsByType(string type)
        {
            if (PopupDetailList != null && !string.IsNullOrEmpty(type) && !string.IsNullOrWhiteSpace(type))
            {
                int index = PopupDetailList.FindIndex(x => x.Type.ToLower() == type.ToLower());
                if (index > -1)
                {
                    PopupModel popupDetails = PopupDetailList[index];
                    //Todo: Handle fallback
                    return popupDetails;
                }
            }
            return null;
        }
    }
}
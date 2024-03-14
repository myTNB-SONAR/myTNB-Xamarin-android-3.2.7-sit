using myTNB.Mobile;
using System;
using System.Collections.Generic;
using static myTNB.Mobile.FeatureInfoClass;

namespace myTNB.Android.Src.MyTNBService.Request
{
    public class SaveLanguagePreferenceRequest : BaseRequest
    {
        public string langPref;
        public List<FeatureInfo> featureInfo;

        public SaveLanguagePreferenceRequest(string langPref)
        {
            this.langPref = langPref;
            this.featureInfo = FeatureInfoManager.Instance.GetFeatureInfo();
        }
    }
}

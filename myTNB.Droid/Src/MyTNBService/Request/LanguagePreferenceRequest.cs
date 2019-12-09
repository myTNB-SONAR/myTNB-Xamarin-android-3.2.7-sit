using System;
using myTNB_Android.Src.Base.Request;

namespace myTNB_Android.Src.MyTNBService.Request
{
    public class LanguagePreferenceRequest : APIBaseRequest
    {
        public string langPref;
        public LanguagePreferenceRequest(string mLangPref)
        {
            langPref = mLangPref;
        }
    }
}

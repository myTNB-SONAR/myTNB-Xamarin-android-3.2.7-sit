using System;
using myTNB.Android.Src.Base.Request;

namespace myTNB.Android.Src.MyTNBService.Request
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

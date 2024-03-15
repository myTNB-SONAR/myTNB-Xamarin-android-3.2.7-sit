using System;
using myTNB.AndroidApp.Src.Base.Request;

namespace myTNB.AndroidApp.Src.MyTNBService.Request
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

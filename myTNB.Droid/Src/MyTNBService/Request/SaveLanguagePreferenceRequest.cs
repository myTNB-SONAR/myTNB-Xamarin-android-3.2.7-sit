using System;
namespace myTNB_Android.Src.MyTNBService.Request
{
    public class SaveLanguagePreferenceRequest : BaseRequest
    {
        public string langPref;

        public SaveLanguagePreferenceRequest(string langPref)
        {
            this.langPref = langPref;
        }
    }
}

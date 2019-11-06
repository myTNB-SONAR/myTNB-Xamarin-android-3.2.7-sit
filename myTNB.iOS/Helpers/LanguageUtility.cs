using System.Collections.Generic;
using Foundation;

namespace myTNB
{
    public static class LanguageUtility
    {
        #region Language Settings
        private static readonly string IsLanguageSetKey = "isLanguageSet";
        private static readonly string CurrentLanguageKey = "currentLanguage";
        private static readonly List<string> SupportedLanguage = new List<string> { "EN", "MS" };

        public static bool IsLanguageSet
        {
            get
            {
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                return userDefaults.BoolForKey(IsLanguageSetKey);
            }
        }

        public static string CurrentSavedLanguage
        {
            get
            {
                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                return userDefaults.StringForKey(CurrentLanguageKey);
            }
        }

        public static string CurrentDeviceLanguage
        {
            get
            {
                string lang = NSLocale.PreferredLanguages[0];
                ValidateLanguage(ref lang);
                return lang;
            }
        }

        public static void SetLanguage(string lang)
        {
            ValidateLanguage(ref lang);
            NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
            userDefaults.SetString(lang, CurrentLanguageKey);
            userDefaults.SetBool(true, IsLanguageSetKey);
            userDefaults.Synchronize();
        }

        private static void ValidateLanguage(ref string lang)
        {
            string language = lang;
            int index = SupportedLanguage.FindIndex(x => language.ToLower().Contains(x.ToLower()));
            if (index < 0)
            {
                //Set to EN if other language code was returned
                lang = SupportedLanguage[0];
            }
            else
            {
                lang = SupportedLanguage[index];
            }
        }

        public static void SetAppLanguage(string lang)
        {
            TNBGlobal.APP_LANGUAGE = lang;
        }

        public static void InitializeLanguage()
        {
            SetAppLanguage(IsLanguageSet ? CurrentSavedLanguage : CurrentDeviceLanguage);
        }

        #endregion

        #region I18N Settings
        public static string GetCommonI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.CommonI18NDictionary != null
                && DataManager.DataManager.SharedInstance.CommonI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.CommonI18NDictionary[key] : string.Empty;
        }
        public static string GetHintI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.HintI18NDictionary != null
                && DataManager.DataManager.SharedInstance.HintI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.HintI18NDictionary[key] : string.Empty;
        }
        public static string GetErrorI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.ErrorI18NDictionary != null
                && DataManager.DataManager.SharedInstance.ErrorI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.ErrorI18NDictionary[key] : string.Empty;
        }

        public static void SetLanguageGlobals()
        {
            DataManager.DataManager.SharedInstance.CommonI18NDictionary = LanguageManager.Instance.GetCommonValuePairs();
            DataManager.DataManager.SharedInstance.HintI18NDictionary = LanguageManager.Instance.GetHintValuePairs();
            DataManager.DataManager.SharedInstance.ErrorI18NDictionary = LanguageManager.Instance.GetErrorValuePairs();
        }
        #endregion
    }
}
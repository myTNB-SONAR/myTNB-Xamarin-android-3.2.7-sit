using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using myTNB.Model;
using myTNB.Model.Language;

namespace myTNB
{
    public static class LanguageUtility
    {
        #region Language Settings
        private static readonly string IsLanguageSetKey = "isLanguageSet";
        private static readonly string CurrentLanguageKey = "currentLanguage";
        private static readonly List<string> SupportedLanguage = new List<string> { "EN", "MS" };
        private static readonly string LanguageContentKey = "LanguageContent";

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

        public static void SetAppLanguageByIndex(int index)
        {
            if (index > -1 && index < SupportedLanguage.Count)
            {
                SetAppLanguage(SupportedLanguage[index]);
            }
        }

        public static void InitializeLanguage()
        {
            SetAppLanguage(IsLanguageSet ? CurrentSavedLanguage : CurrentDeviceLanguage);
        }

        public static int CurrentLanguageIndex
        {
            get
            {
                if (IsLanguageSet)
                {
                    int index = SupportedLanguage.FindIndex(x => CurrentSavedLanguage.ToLower().Contains(x.ToLower()));
                    if (index > -1 && index < SupportedLanguage.Count)
                    {
                        return index;
                    }
                }
                else
                {
                    int index = SupportedLanguage.FindIndex(x => CurrentDeviceLanguage.ToLower().Contains(x.ToLower()));
                    if (index > -1 && index < SupportedLanguage.Count)
                    {
                        return index;
                    }
                }
                return 0;
            }
        }

        public static void SaveLanguageContent(string content)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetString(content, LanguageContentKey);
                sharedPreference.Synchronize();
            }
        }

        public static string LanguageContent
        {
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                string content = sharedPreference.StringForKey(LanguageContentKey);
                return content ?? string.Empty;
            }
        }

        public static bool HasSavedContent
        {
            get
            {
                return !string.IsNullOrEmpty(LanguageContent) && !string.IsNullOrWhiteSpace(LanguageContent);
            }
        }

        public static string GetLanguageCodeByIndex(int index)
        {
            if (index > -1 && index < SupportedLanguage.Count)
            {
                return SupportedLanguage[index];
            }
            return string.Empty;
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

        #region Services
        private static readonly string Service_SaveLanguage = "SaveLanguagePreference";
        private static readonly string Service_GetLanguage = "GetLanguagePreference";
        public static LanguageResponseModel GetLanguageResponse { private set; get; } = new LanguageResponseModel();
        public static LanguageResponseModel SaveLanguageResponse { private set; get; } = new LanguageResponseModel();
        private static readonly string DidUserChangeLanguageKey = "didUserChangeLanguage";

        public static bool DidUserChangeLanguage
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(value, DidUserChangeLanguageKey);
                sharedPreference.Synchronize();
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                return sharedPreference.BoolForKey(DidUserChangeLanguageKey);
            }
        }

        public static bool IsGetSuccess
        {
            get
            {
                return GetLanguageResponse != null && GetLanguageResponse.d != null && GetLanguageResponse.d.IsSuccess;
            }
        }

        public static bool IsSaveSuccess
        {
            get
            {
                return SaveLanguageResponse != null && SaveLanguageResponse.d != null && SaveLanguageResponse.d.IsSuccess;
            }
        }

        public static string ServiceLanguage
        {
            get
            {
                string lang = TNBGlobal.APP_LANGUAGE;
                if (IsGetSuccess)
                {
                    lang = GetLanguageResponse.d.data.lang;
                }
                return lang ?? TNBGlobal.APP_LANGUAGE;
            }
        }

        public static bool IsSameAsCurrentLanguage
        {
            get
            {
                return TNBGlobal.APP_LANGUAGE == ServiceLanguage;
            }
        }

        public static Task SaveLanguagePreference()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    langPref = TNBGlobal.APP_LANGUAGE
                };
                SaveLanguageResponse = serviceManager.OnExecuteAPIV6<LanguageResponseModel>(Service_SaveLanguage, requestParameter);
            });
        }

        public static Task GetLanguagePreference()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf
                };
                GetLanguageResponse = serviceManager.OnExecuteAPIV6<LanguageResponseModel>(Service_GetLanguage, requestParameter);
            });
        }
        #endregion
    }
}
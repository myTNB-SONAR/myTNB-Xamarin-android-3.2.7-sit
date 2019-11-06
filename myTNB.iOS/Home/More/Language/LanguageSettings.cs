using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;

namespace myTNB
{
    public static class LanguageSettings
    {
        private const string LANGUAGE_KEY = "languageIndex";
        private const string DID_USER_SET_KEY = "didUserSetLanguage";

        private readonly static List<string> _supportedLanguageCode = new List<string>()
        {
            "EN", "MS"
        };

        private static int _selectedLanguageIndex;

        public static List<string> SupportedLanguage
        {
            get
            {
                return new List<string>{
                    LanguageUtility.GetCommonI18NValue(Constants.Common_English)
                    , LanguageUtility.GetCommonI18NValue(Constants.Common_Bahasa)
                };
            }
        }

        public static List<string> SupportedLanguageCode
        {
            get
            {
                return _supportedLanguageCode;
            }
        }

        public static string Title
        {
            get
            {
                return LanguageUtility.GetCommonI18NValue(Constants.Common_SetAppLanguage);
            }
        }

        public static string SectionTitle
        {
            get
            {
                return LanguageUtility.GetCommonI18NValue(Constants.Common_SetAppLanguageDescription);
            }
        }

        public static string CTATitle
        {
            get
            {
                return LanguageUtility.GetCommonI18NValue(Constants.Common_SaveChanges);
            }
        }

        public static Action<int> OnSelect
        {
            get
            {
                return OnSelectAction;
            }
        }

        public static int SelectedLanguageIndex
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetInt(value, LANGUAGE_KEY);
                sharedPreference.Synchronize();
                _selectedLanguageIndex = value;
            }
            get
            {
                return _selectedLanguageIndex;
            }
        }

        static void OnSelectAction(int index)
        {
            SetLanguage(index);
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(true, DID_USER_SET_KEY);
            sharedPreference.Synchronize();
            NotifCenterUtility.PostNotificationName("LanguageDidChange", new NSObject());
        }

        public static void SetLanguage(int index)
        {
            index = index > -1 ? index : 0;
            SelectedLanguageIndex = index;

            if (SelectedLanguageIndex < SupportedLanguageCode.Count)
            {
                string langCode = SupportedLanguageCode[index];
                if (langCode.ToUpper() == LanguageManager.Language.EN.ToString().ToUpper())
                {
                    LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, LanguageManager.Language.EN);
                }
                else
                {
                    LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, LanguageManager.Language.MS);
                }
            }
        }

        public static void SetLanguageV2(int index)
        {
            index = index > -1 ? index : 0;
            SelectedLanguageIndex = index;

            if (SelectedLanguageIndex < SupportedLanguageCode.Count)
            {
                string langCode = SupportedLanguageCode[index];
                if (langCode.ToUpper() == LanguageManager.Language.EN.ToString().ToUpper())
                {
                    LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, LanguageManager.Language.EN);
                }
                else
                {

                    LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, LanguageManager.Language.MS);
                }
            }

            LanguageUtility.SetLanguageGlobals();
        }
    }
}

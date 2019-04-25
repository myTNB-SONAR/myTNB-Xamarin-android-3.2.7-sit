using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;

namespace myTNB
{
    public static class LanguageSettings
    {
        const string LANGUAGE_KEY = "languageIndex";
        const string DID_USER_SET_KEY = "didUserSetLanguage";
        readonly static List<string> _supportedLanguage = new List<string>()
        {
            "Language_English".Translate()
            , "Language_Malay".Translate()
        };

        readonly static List<string> _supportedLanguageCode = new List<string>()
        {
            "EN", "MS"
        };
        
        static int _selectedLanguageIndex;
        static NSBundle _languageBundle;

        public static List<string> SupportedLanguage
        {
            get
            {
                return _supportedLanguage;
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
                return "Language_Title".Translate();
            }
        }

        public static Action<int> OnSelect
        {
            get
            {
                return OnSelectAction;
            }
        }

        public static int SelectedLangugageIndex
        {
            set
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetInt(value, LANGUAGE_KEY);
                sharedPreference.Synchronize();
                int index = (int)sharedPreference.IntForKey(LANGUAGE_KEY);
                _selectedLanguageIndex = value;
            }
            get
            {
                return _selectedLanguageIndex;
            }
        }

        public static NSBundle LanguageBundle
        {
            set
            {
                _languageBundle = value;
            }
            get
            {
                return _languageBundle;
            }
        }

        static void OnSelectAction(int index)
        {
            SetLanguage(index);
            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            sharedPreference.SetBool(true, DID_USER_SET_KEY);
            sharedPreference.Synchronize();
        }

        public static void SetLanguage(int index)
        {
            Debug.WriteLine("DEBUG >>> SetLanguage: " + index);
            index = index > -1 ? index : 0;
            SelectedLangugageIndex = index;
            string pathName = SupportedLanguageCode[index].ToLower();
            pathName = string.Compare(pathName, "en") == 0 ? "Base" : pathName;
            var path = NSBundle.MainBundle.PathForResource(pathName, "lproj");
            LanguageBundle = NSBundle.FromPath(path);
        }

        public static void InitializeLanguage()
        {

            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
            int index = (int)sharedPreference.IntForKey(LANGUAGE_KEY);
            string lang = NSLocale.CurrentLocale.LocaleIdentifier;
            if (!sharedPreference.BoolForKey(DID_USER_SET_KEY))
            {
                index = SupportedLanguageCode.FindIndex(x => lang.ToLower().Contains(x.ToLower()));
            }
            SetLanguage(index);
        }

        public static string GetLanguageCode()
        {
            return string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace myTNB
{
    public sealed class LanguageManager
    {
        private static readonly Lazy<LanguageManager> lazy =
        new Lazy<LanguageManager>(() => new LanguageManager());

        public static LanguageManager Instance { get { return lazy.Value; } }

        public enum Language
        {
            EN,
            MS
        }

        public enum Source
        {
            FILE,
            SITECORE
        }

        private LanguageManager()
        {
        }

        private string JSONLang = string.Empty;
        private const string SELECTOR = "_selector";
        private const string COMMON = "Common";
        public const string SelectMonth_selector = "SelectMonth_selector";
        private const string Hint = "Hint";
        private const string Error = "Error";
        private const string Tutorial = "Tutorial";
        private const string LANGUAGE_RESOURCE_PATH = "myTNB.Mobile.Resources.Language.Language_{0}.json";

        /// <summary>
        /// Sets the language to be used by the app.
        /// Advisable to call on app launch.
        /// </summary>
        /// <param name="src">src is ENUM of sources, either file or sitecore</param>
        /// <param name="lang">lang is ENUM of language, options are en and ms</param>
        public void SetLanguage(Source src = Source.FILE, Language lang = Language.EN)
        {
            if (src == Source.FILE)
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Stream stream = assembly.GetManifestResourceStream(string.Format(LANGUAGE_RESOURCE_PATH, lang));
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        JSONLang = reader.ReadToEnd();
                        Debug.WriteLine("DEBUG >> JSONLang: " + JSONLang);
                    }
                    Debug.WriteLine("DEBUG >> SUCCESS");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("DEBUG >> SetLanguageData: " + e.Message);
                }
            }
            else
            {
                //Todo: Add sitecore logic
            }
        }

        public void SetLanguage(string content)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
            {
                JSONLang = content;
            }
        }

        /// <summary>
        /// Gets the commonly used strings within the app like "OK", "Cancel".
        /// Advised to call this when app was launched.
        /// </summary>
        /// <returns>Key-value pairs of commonly used strings</returns>
        public Dictionary<string, string> GetCommonValuePairs()
        {
            return GetValuesByPage(COMMON);
        }

        /// <summary>
        /// Gets the Month selector
        /// Advised to call this when app was launched.
        /// </summary>
        /// <returns>Key-value pairs of month selector strings</returns>
        public Dictionary<string, List<SelectorModel>> GetMonthSelectorValuePairs()
        {
            return GetValues<Dictionary<string, List<SelectorModel>>>(SelectMonth_selector);
        }

        public Dictionary<string, string> GetHintValuePairs()
        {
            return GetValuesByPage(Hint);
        }

        public Dictionary<string, string> GetErrorValuePairs()
        {
            return GetValuesByPage(Error);
        }
        /// <summary>
        /// Gets the key-value pair of texts of a page.
        /// Asvisable to call on intialisation of the page.
        /// </summary>
        /// <param name="pageName">Name of the page, iOS and Android should be the same.</param>
        /// <returns>Key-value pair of page's strings</returns>
        public Dictionary<string, string> GetValuesByPage(string pageName)
        {
            return GetValues<Dictionary<string, string>>(pageName);
        }
        /// <summary>
        /// Gets the selector of a page.
        /// </summary>
        /// <param name="pageName">Name of the page, iOS and Android should be the same</param>
        /// <returns>Key-List Value of selectors used by the page</returns>
        public Dictionary<string, List<SelectorModel>> GetSelectorsByPage(string pageName)
        {
            pageName += SELECTOR;
            return GetValues<Dictionary<string, List<SelectorModel>>>(pageName);
        }

        /// <summary>
        /// Gets the selector of a page.
        /// </summary>
        /// <param name="pageName">Name of the page, iOS and Android should be the same</param>
        /// <returns>Key-List Value of selectors used by the page</returns>
        public Dictionary<string, List<PopupSelectorModel>> GetPopupSelectorsByPage(string pageName)
        {
            pageName += SELECTOR;
            return GetValues<Dictionary<string, List<PopupSelectorModel>>>(pageName);
        }
        public Dictionary<string, string> GetTutorialValuePairs()
        {
            return GetValuesByPage(Tutorial);
        }

        private T GetValues<T>(string pageName) where T : new()
        {
            T valuesDictionary = new T();
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrWhiteSpace(pageName))
            {
                return valuesDictionary;
            }
            try
            {
                var jsonObj = JObject.Parse(JSONLang);
                if (jsonObj != null)
                {
                    string value = jsonObj[pageName]?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                    {
                        valuesDictionary = JsonConvert.DeserializeObject<T>(value);
                    }
                }
                return valuesDictionary;
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG Error: ", e.Message);
            }
            return valuesDictionary;
        }
    }
}
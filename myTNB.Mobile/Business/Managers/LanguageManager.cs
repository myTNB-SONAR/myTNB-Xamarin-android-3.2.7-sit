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

        internal string JSONLang
        {
            private set;
            get;
        } = string.Empty;
        private const string SELECTOR = "_selector";
        private const string COMMON = "Common";
        private const string Hint = "Hint";
        private const string Error = "Error";
        private const string Tutorial = "Tutorial";
        private const string MarketingPopup = "MarketingPopup";
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
                        //Debug.WriteLine("[DEBUG] JSONLang: " + JSONLang);
                    }
                    Debug.WriteLine("[DEBUG] SUCCESS");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[DEBUG] Error SetLanguageData: " + e.Message);
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

        public string GetLanguageFile
        {
            get
            {
                return JSONLang ?? string.Empty;
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

        public string GetCommonValue(string key)
        {
            Dictionary<string, string> commonDictionary = GetCommonValuePairs();
            return commonDictionary != null
               && commonDictionary.ContainsKey(key)
               ? commonDictionary[key] : string.Empty;
        }

        public Dictionary<string, string> GetHintValuePairs()
        {
            return GetValuesByPage(Hint);
        }

        public Dictionary<string, string> GetErrorValuePairs()
        {
            return GetValuesByPage(Error);
        }

        public string GetErrorValue(string key)
        {
            Dictionary<string, string> errorDictionary = GetErrorValuePairs();
            return errorDictionary != null
               && errorDictionary.ContainsKey(key)
               ? errorDictionary[key] : string.Empty;
        }

        /// <summary>
        /// Takes the Tutorial Overlay's Key Value Pairs
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTutorialValuePairs()
        {
            return GetValuesByPage(Tutorial);
        }

        /// <summary>
        /// Takes the Marketing Popup Key Value Pairs
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetMarketingPopupValuePairs()
        {
            return GetValuesByPage(MarketingPopup);
        }
        /// <summary>
        /// Gets the key-value pair of texts of a page.
        /// Advisable to call on intialisation of the page.
        /// </summary>
        /// <param name="pageName">Name of the page, iOS and Android should be the same.</param>
        /// <returns>Key-value pair of page's strings</returns>
        public Dictionary<string, string> GetValuesByPage(string pageName)
        {
            return GetValues<Dictionary<string, string>>(pageName);
        }

        public string GetPageValueByKey(string pageName, string key)
        {
            Dictionary<string, string> pageDictionary = GetValuesByPage(pageName);
            return pageDictionary != null
               && pageDictionary.ContainsKey(key)
               ? pageDictionary[key] : string.Empty;
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

        private T GetValues<T>(string pageName) where T : new()
        {
            T valuesDictionary = new T();
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrWhiteSpace(pageName))
            {
                return valuesDictionary;
            }
            try
            {
                JObject jsonObj = JObject.Parse(JSONLang);
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

        public T GetValues<T>(string parentName, string itemName) where T : new()
        {
            T valuesDictionary = new T();
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))
            {
                return valuesDictionary;
            }
            try
            {
                JObject jsonObj = JObject.Parse(LanguageManager.Instance.JSONLang);
                if (jsonObj != null)
                {
                    string value = jsonObj[parentName][itemName]?.ToString() ?? string.Empty;
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

        internal JToken GetServiceConfig(string pageName, string propertyName)
        {
            try
            {
                JObject jsonObj = JObject.Parse(JSONLang);
                if (jsonObj != null
                    && jsonObj[pageName] is JToken pageJToken
                    && pageJToken != null
                    && pageJToken[propertyName] is JToken serviceToken
                    && serviceToken != null)
                {
                    return serviceToken;
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetParsedJson Error: ", e.Message);
                return null;
            }
        }

        public bool GetConfigToggleValue(ConfigPropertyEnum toggleProperty)
        {
            try
            {
                JObject jsonObj = JObject.Parse(JSONLang);
                if (jsonObj != null
                    && jsonObj["ServiceConfiguration"] is JToken pageJToken
                    && pageJToken != null
                    && pageJToken[toggleProperty.ToString()] is JToken serviceToken
                    && serviceToken != null)
                {
                    return serviceToken.ToObject<bool>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetParsedJson Error: ", e.Message);
            }
            return false;
        }

        internal int GetConfigTimeout(ConfigPropertyEnum toggleProperty)
        {
            try
            {
                JObject jsonObj = JObject.Parse(JSONLang);
                if (jsonObj != null
                    && jsonObj["ServiceConfiguration"] is JToken pageJToken
                    && pageJToken != null
                    && pageJToken[toggleProperty.ToString()] is JToken serviceToken
                    && serviceToken != null)
                {
                    return serviceToken.ToObject<int>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetParsedJson Error: ", e.Message);
            }
            return 0;
        }

        internal T GetConfigProperty<T>(ConfigPropertyEnum toggleProperty) where T : new()
        {
            T valuesDictionary = new T();
            try
            {
                JObject jsonObj = JObject.Parse(JSONLang);
                if (jsonObj != null
                    && jsonObj["ServiceConfiguration"] is JToken pageJToken
                    && pageJToken != null
                    && pageJToken[toggleProperty.ToString()] is JToken serviceToken
                    && serviceToken != null)
                {
                    return serviceToken.ToObject<T>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] GetParsedJson Error: ", e.Message);
            }
            return valuesDictionary;
        }

        public enum ConfigPropertyEnum
        {
            ForceHideDBRBanner,
            IsAboutMyBillEnquiryEnabled,
            IsUpdatePersonalDetailsEnquiryEnabled,
            IsGSLRebateEnabled,
            AccountStatementTimeout,
            ShouldShowAccountStatementToNonOwner,
            MaxAccountList,
            ResidentialRateCategory,
            IsMyHomeMarketingPopupEnable,
            ForceHidemyHomeBanner
        }

        public Dictionary<string, List<T>> GetSelectorsByPage<T>(string pageName) where T : new()
        {
            pageName += SELECTOR;
            return GetValues<Dictionary<string, List<T>>>(pageName);
        }
    }
}
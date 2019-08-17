namespace myTNB
{
    public static class LanguageUtility
    {
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
    }
}
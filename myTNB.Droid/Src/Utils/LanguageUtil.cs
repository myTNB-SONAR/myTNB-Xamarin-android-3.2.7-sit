using System;
using Android.Util;
using myTNB;

namespace myTNB_Android.Src.Utils
{
    public class LanguageUtil
    {
        private LanguageUtil()
        {
        }

		/// <summary>
		/// Gets the Error labels by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetLocalizedErrorLabel(string key)
		{
			string label = "";
			try
			{
				label = LanguageManager.Instance.GetErrorValuePairs()[key];
			}
			catch (Exception e)
			{
				Log.Debug("DEBUG Error: ", e.Message);
			}
			return label;
		}

		public static string GetDeviceLanguage()
		{
			string deviceLanguage = Java.Util.Locale.Default.Language;
			if (deviceLanguage.ToUpper() == Constants.SUPPORTED_LANGUAGES.MS.ToString())
			{
				deviceLanguage = Java.Util.Locale.Default.Language;
			}
			else
			{
				deviceLanguage = Constants.SUPPORTED_LANGUAGES.EN.ToString();
			}
			return deviceLanguage.ToUpper();
		}

        public static bool IsSupportedLanguage(string language)
        {
            bool isSupported = false;
            foreach (string supportedLanguage in Enum.GetNames(typeof(Constants.SUPPORTED_LANGUAGES)))
            {
                if (language.ToUpper() == supportedLanguage)
                {
                    isSupported = true;
                }
            }
            return isSupported;
        }

        public static bool HasSavedLanguage()
        {
            return string.IsNullOrEmpty(UserSessions.GetAppLanguage());
        }

        public static void SaveAppLanguage(string language)
		{
            string savedLanguage = Constants.DEFAULT_LANG; // Default Language is EN
            if (!string.IsNullOrEmpty(language) && IsSupportedLanguage(language))
            {
                savedLanguage = language;
            }
            UserSessions.SaveAppLanguage(savedLanguage);
            UpdateSavedLanguage(savedLanguage);
        }

        public static string GetAppLanguage()
        {
            string language = UserSessions.GetAppLanguage();
            if (string.IsNullOrEmpty(language))
            {
                string deviceLanguage = Java.Util.Locale.Default.Language;
                language = IsSupportedLanguage(deviceLanguage) ? deviceLanguage.ToUpper() : Constants.DEFAULT_LANG;
            }
            return language;
        }

		public static void UpdateSavedLanguage(string selectedLanguage)
		{
			LanguageManager.Language language;
			if (selectedLanguage == "MS")
			{
				language = LanguageManager.Language.MS;
			}
			else
			{
				language = LanguageManager.Language.EN;
			}

			//try
			//{
			//    string density = DPUtils.GetDeviceDensity(Application.Context);
			//    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, selectedLanguage.ToLower());// SiteCoreConfig.DEFAULT_LANGUAGE);
			//    //LanguageResponseModel responseModel = getItemsService.GetLanguageItems();
			//    var timestamp = getItemsService.GetLanguageTimestampItem();
			//    //SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.LANGUAGE_URL, JsonConvert.SerializeObject(responseModel.Data), "");
			//    string content = string.Empty;
			//    //WebRequest webRequest = WebRequest.Create(responseModel.Data[0].LanguageFile);
			//    //using (WebResponse response = webRequest.GetResponse())
			//    //using (Stream responseStream = response.GetResponseStream())
			//    //using (StreamReader reader = new StreamReader(responseStream))
			//    //{
			//    //    content = reader.ReadToEnd();
			//    //}

			//    //System.Diagnostics.Debug.WriteLine("Content: " + content);
			//    //LanguageManager.Instance.SetLanguage(content);
			//}
			//catch (Exception e)
			//{
			//    Utility.LoggingNonFatalError(e);
			//}

			LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, language);
		}

        public static void SetInitialAppLanguage()
        {
            string language = GetAppLanguage();
            UpdateSavedLanguage(language);
            SetIsLanguageChanged(false);
        }

        public static void SetIsLanguageChanged(bool isChanged)
        {
            UserSessions.SaveIsAppLanguageChanged(isChanged);
        }

        public static bool IsLanguageChanged()
        {
            return UserSessions.GetIsAppLanguageChanged();
        }
    }
}

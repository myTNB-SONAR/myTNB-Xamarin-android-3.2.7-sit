using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using DynatraceAndroid;
using myTNB;
using myTNB.Mobile;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.SiteCore;

namespace myTNB_Android.Src.Utils
{
    public class LanguageUtil
    {
        private LanguageUtil()
        {
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
            //Saves language in session
            UserSessions.SaveAppLanguage(savedLanguage);
            //Updates language in LanguageManager
            UpdateSavedLanguage(savedLanguage);
            //Sets language flag to true
            SetIsLanguageChanged(true);
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

            string density = DPUtils.GetDeviceDensity(Application.Context);
            GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, selectedLanguage.ToLower());
            SitecoreCmsEntity.SITE_CORE_ID siteCoreLanguageId = SitecoreCmsEntity.SITE_CORE_ID.LANGUAGE_EN;
            LanguageManager.Language language = LanguageManager.Language.EN;

            if (selectedLanguage != Constants.DEFAULT_LANG)
            {
                language = LanguageManager.Language.MS;
                siteCoreLanguageId = SitecoreCmsEntity.SITE_CORE_ID.LANGUAGE_MS;
            }

#if DEBUG
            LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, language);
            DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_LOCAL);

#else
            string currentTimestamp = SitecoreCmsEntity.GetItemTimestampById(siteCoreLanguageId);
            string currentLanguageResource = SitecoreCmsEntity.GetItemById(siteCoreLanguageId);
            string updatedTimestamp;
            string updatedLanguageResource;

            if (!string.IsNullOrEmpty(currentTimestamp))
            {
                updatedTimestamp = GetLanguageTimeStamp(getItemsService);
                if (currentTimestamp == updatedTimestamp)
                {
                    if (!string.IsNullOrEmpty(currentLanguageResource))
                    {
                        LanguageManager.Instance.SetLanguage(currentLanguageResource);
                        DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_ONLINE);
                    }
                    else
                    {
                        updatedLanguageResource = GetUpdatedLanguage(getItemsService);
                        if (!string.IsNullOrEmpty(updatedLanguageResource))
                        {
                            SitecoreCmsEntity.InsertSiteCoreItem(siteCoreLanguageId, updatedLanguageResource, currentTimestamp);
                            LanguageManager.Instance.SetLanguage(updatedLanguageResource);
                            DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_ONLINE);
                        }
                        else
                        {
                            LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, language);
                            DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_LOCAL);
                        }
                    }
                }
                else
                {
                    updatedLanguageResource = GetUpdatedLanguage(getItemsService);
                    if (!string.IsNullOrEmpty(updatedLanguageResource))
                    {
                        SitecoreCmsEntity.InsertSiteCoreItem(siteCoreLanguageId, updatedLanguageResource, updatedTimestamp);
                        LanguageManager.Instance.SetLanguage(updatedLanguageResource);
                        DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_ONLINE);
                    }
                    else
                    {
                        LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, language);
                        DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_LOCAL);
                    }
                }
            }
            else
            {
                updatedLanguageResource = GetUpdatedLanguage(getItemsService);
                updatedTimestamp = GetLanguageTimeStamp(getItemsService);
                if (!string.IsNullOrEmpty(updatedLanguageResource))
                {
                    SitecoreCmsEntity.InsertSiteCoreItem(siteCoreLanguageId, updatedLanguageResource, updatedTimestamp);
                    LanguageManager.Instance.SetLanguage(updatedLanguageResource);
                    DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_ONLINE);
                }
                else
                {
                    LanguageManager.Instance.SetLanguage(LanguageManager.Source.FILE, language);
                    DynatraceHelper.OnTrack(DynatraceConstants.SITECORE_REFER_LOCAL);
                }
            }
#endif
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

        private static string GetUpdatedLanguage(GetItemsService getItemsService)
        {
            string languageItem = string.Empty;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    LanguageResponseModel responseModel = getItemsService.GetLanguageItems();
                    WebRequest webRequest = WebRequest.Create(responseModel.Data[0].LanguageFile);
                    using (WebResponse response = webRequest.GetResponse())
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        languageItem = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).Wait();
            return languageItem;
        }

        private static string GetLanguageTimeStamp(GetItemsService getItemsService)
        {
            string timestamp = string.Empty;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    LanguageTimeStampResponseModel responseModel = getItemsService.GetLanguageTimestampItem();
                    timestamp = responseModel.Data[0].Timestamp;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).Wait();
            return timestamp;
        }

        public static async Task SaveUpdatedLanguagePreference()
        {
            try
            {
                string appLanguage = GetAppLanguage();
                //Checks if language is not set yet from app
                //If not set, get the saved language from GetLanguagePreference service
                if (!IsLanguageChanged())
                {
                    var response = await ServiceApiImpl.Instance.GetLanguagePreference(new MyTNBService.Request.BaseRequest());
                    if (response.IsSuccessResponse())
                    {
                        if (!string.IsNullOrEmpty(response.GetData().lang))
                        {
                            appLanguage = response.GetData().lang;
                            SaveAppLanguage(appLanguage);
                        }
                    }
                    else
                    {
                        SaveLanguagePreferenceResponse saveLanguagePreferenceResponse = await ServiceApiImpl.Instance.SaveLanguagePreference(new MyTNBService.Request.SaveLanguagePreferenceRequest(appLanguage));
                        UserSessions.SavedLanguagePrefResult(saveLanguagePreferenceResponse.IsSuccessResponse()
                            && saveLanguagePreferenceResponse.GetData() != null && !string.IsNullOrEmpty(saveLanguagePreferenceResponse.GetData().lang));
                    }
                }
                else
                {
                    SaveLanguagePreferenceResponse saveLanguagePreferenceResponse = await ServiceApiImpl.Instance.SaveLanguagePreference(new MyTNBService.Request.SaveLanguagePreferenceRequest(appLanguage));
                    UserSessions.SavedLanguagePrefResult(saveLanguagePreferenceResponse.IsSuccessResponse()
                        && saveLanguagePreferenceResponse.GetData() != null && !string.IsNullOrEmpty(saveLanguagePreferenceResponse.GetData().lang));
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public static void SaveLanguagePrefInBackground()
        {
            try
            {
                if (!UserSessions.IsSavedLanguagePrefResultSuccess())
                {
                    string appLanguage = GetAppLanguage();
                    Task.Factory.StartNew(async () =>
                    {
                        SaveLanguagePreferenceResponse saveLanguagePreferenceResponse = await ServiceApiImpl.Instance.SaveLanguagePreference(new MyTNBService.Request.SaveLanguagePreferenceRequest(appLanguage));
                        UserSessions.SavedLanguagePrefResult(saveLanguagePreferenceResponse.IsSuccessResponse()
                            && saveLanguagePreferenceResponse.GetData() != null && !string.IsNullOrEmpty(saveLanguagePreferenceResponse.GetData().lang));
                    });
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}

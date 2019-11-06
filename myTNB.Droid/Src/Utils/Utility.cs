﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SSMR.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace myTNB_Android.Src.Utils
{
    public class Utility
    {
        public Utility()
        {
        }


        public static bool AccountNumberValidation(int length)
        {
            return (length == 12 || length == 14);
        }

        public static bool AddAccountNumberValidation(int length)
        {
            return (length == 12);
        }

        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool isAlpha(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s,]*$");
            return rg.IsMatch(strToCheck);
        }


        public static bool IsValidMobileNumber(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                if (mobileNumber.StartsWith("+60"))
                {
                    if (mobileNumber.Substring(3).Length == 9 || mobileNumber.Substring(3).Length == 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static void LoggingNonFatalError(Exception e)
        {
            Crashlytics.Crashlytics.LogException(new Java.Lang.Throwable(e.ToString()));
        }


        public static bool IsPermissionHasCount(Permission[] grantResults)
        {
            return (grantResults != null && grantResults.Length > 0);
        }

        public static SpannableString GetFormattedURLString(ClickableSpan clickableSpan, Java.Lang.ICharSequence charSequence)
        {
            SpannableString s = new SpannableString(charSequence);
            Java.Lang.Object[] urlSpans = s.GetSpans(0, s.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
            if (urlSpans.Length != 0)
            {
                foreach (Java.Lang.Object obj in urlSpans)
                {
                    int startFAQLink = s.GetSpanStart(obj);
                    int endFAQLink = s.GetSpanEnd(obj);
                    s.RemoveSpan(obj);
                    s.SetSpan(clickableSpan, startFAQLink, endFAQLink, SpanTypes.ExclusiveExclusive);
                }
            }
            return s;
        }

        /// <summary>
        /// Gets the label based on selected language.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizedLabel(string pageId, string key)
        {
            string label = "";
            try
            {
                label = LanguageManager.Instance.GetValuesByPage(pageId)[key];
            }
            catch (Exception e)
            {
                Log.Debug("DEBUG Error: ", e.Message);
            }
            return label;
        }

        /// <summary>
        /// Gets the tooltip selector based on selected language.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static List<PopupSelectorModel> GetTooltipSelectorModel(string pageId, string keyId)
        {
            List<PopupSelectorModel> popupSelectorModels = new List<PopupSelectorModel>();
            try
            {
                popupSelectorModels = LanguageManager.Instance.GetPopupSelectorsByPage(pageId)[keyId];
            }
            catch (Exception e)
            {
                Log.Debug("DEBUG Error: ", e.Message);
            }
            return popupSelectorModels;
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

        public static void SaveSelectedLanguage(ISharedPreferences preferences, string language)
        {
            if (language == "MS")
            {
                UserSessions.SaveSelectedLanguage(preferences,"EN");
            }
            else
            {
                UserSessions.SaveSelectedLanguage(preferences, "MS");
            }
        }

        public static void ShowChangeLanguageDialog(Context context, string selectedLanguage, Action confirmAction)
        {
            MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "changeLanguageTitle_" + selectedLanguage))
                        .SetMessage(Utility.GetLocalizedLabel("Common", "changeLanguageMessage_" + selectedLanguage))
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageNo_" + selectedLanguage))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageYes_" + selectedLanguage))
                        .SetSecondaryCTAaction(()=>
                        {
                            confirmAction();
                        })
                        .Build().Show();
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
    }
}

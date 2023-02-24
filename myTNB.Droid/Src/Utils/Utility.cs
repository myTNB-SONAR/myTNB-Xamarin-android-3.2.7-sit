using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SSMR.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using myTNB_Android.Src.Base.Models;
using myTNB.Mobile.Helpers;

namespace myTNB_Android.Src.Utils
{
    public class Utility
    {
        private static bool IsPayDisableNotFromAppLaunch = false;
        private static string AppUpdateId = "";

        public static string ACCOUNT_NAME_PATTERN = @"^[a-zA-Z @/-]*$";

        public enum Masking
        {
            Address,
            Id
        }

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

        public static bool isSpecialcharacter(string strToCheck)
        {

            Regex rg = new Regex(@"^[\w@\-]+$"); ;
            return rg.IsMatch(strToCheck);
        }

        public static bool isAlpha(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsNotASCII(string strToCheck)
        {
            Regex rg = new Regex(@"[^\x00-\x7F]");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsValidAccountName(string acctName)
        {
            Regex rg = new Regex(ACCOUNT_NAME_PATTERN);
            return rg.IsMatch(acctName);
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
        /// Gets the Common labels by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizedCommonLabel(string key)
        {
            string label = "";
            try
            {
                label = LanguageManager.Instance.GetCommonValuePairs()[key];
            }
            catch (Exception e)
            {
                Log.Debug("DEBUG Error: ", e.Message);
            }
            return label;
        }

        /// <summary>
        /// Gets the Month Selector List by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<BaseKeyValueModel> GetLocalizedMonthSelectorLabel(string key)
        {
            List<BaseKeyValueModel> monthList = new List<BaseKeyValueModel>();
            try
            {
                //Dictionary<string, List<SelectorModel>> monthSelectorList =
                //if (monthSelectorList != null && monthSelectorList.Count > 0)
                //{
                List<SelectorModel> list = FilterHelper.GetMonthList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        monthList.Add(new BaseKeyValueModel()
                        {
                            Key = item.Key,
                            Value = item.Value
                        });
                    }
                }
                // }
            }
            catch (Exception e)
            {
                Log.Debug("DEBUG Error: ", e.Message);
            }
            return monthList;
        }

        /// <summary>
        /// Gets the Hint labels by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizedHintLabel(string key)
        {
            string label = "";
            try
            {
                label = LanguageManager.Instance.GetHintValuePairs()[key];
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
        /// Gets the tooltip selector based on selected language.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static List<SelectorModel> GetSelectoryByKey(string page, string key)
        {
            Dictionary<string, List<SelectorModel>> selector = GetSelectorByPage(page);
            return selector != null && selector.ContainsKey(key) ? selector[key] : new List<SelectorModel>();
        }
        public static Dictionary<string, List<SelectorModel>> GetSelectorByPage(string page)
        {
            return LanguageManager.Instance.GetSelectorsByPage(page);
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

        public static void ShowUpdateIdDialog(Activity context, string ic_no, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "updateIdTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("Common", "updateIdMessage"))
                        .SetContentGravity(Android.Views.GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "re_enter"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "confirm"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowChangeLanguageDialog(Activity context, string selectedLanguage, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Common", "changeLanguageTitle_" + selectedLanguage))
                        .SetMessage(Utility.GetLocalizedLabel("Common", "changeLanguageMessage_" + selectedLanguage))
                        .SetContentGravity(Android.Views.GravityFlags.Center)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageNo_" + selectedLanguage))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Common", "changeLanguageYes_" + selectedLanguage))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }


        //public static void ShowIdentificationUpdateProfileDialog(Activity context, Action confirmAction, Action checkboxAction, Action uncheckboxAction, Action cancelAction = null)
        //{
        //    MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.DIALOGBOX_WITH_CHECKBOX)
        //                .SetTitle(Utility.GetLocalizedLabel("DashboardHome", "titleIcUpdate"))
        //                .SetMessage(Utility.GetLocalizedLabel("DashboardHome", "bodyIcUpdate"))
        //                .SetTitleCheckBox(Utility.GetLocalizedLabel("Common", "dontShowThisAgain"))
        //                .SetContentGravity(Android.Views.GravityFlags.Left)
        //                .SetCTALabel(Utility.GetLocalizedLabel("DashboardHome", "proceed"))
        //                .SetSecondaryCTAaction(() =>
        //                {
        //                    confirmAction();
        //                })
        //                .SetCheckBoxCTaction(() =>
        //                {
        //                    checkboxAction();
        //                })
        //                .SetUnCheckBoxCTaction(() =>
        //                {
        //                    uncheckboxAction();
        //                })
        //                .Build();

        //    tooltipBuilder.SetCTAaction(() =>
        //    {
        //        if (cancelAction != null)
        //        {
        //            cancelAction();
        //            tooltipBuilder.DismissDialog();
        //        }
        //        else
        //        {
        //            tooltipBuilder.DismissDialog();
        //        }
        //    }).Show();
        //}

        public static void ShowIdentificationUpdateProfileDialog(Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.DIALOGBOX_WITH_IMAGE_ONE_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("DashboardHome", "titleIcUpdate"))
                        .SetMessage(Utility.GetLocalizedLabel("DashboardHome", "bodyIcUpdate"))
                        .SetTitleCheckBox(Utility.GetLocalizedLabel("Common", "dontShowThisAgain"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("DashboardHome", "btnIcUpdate"))
                        .SetCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();

            tooltipBuilder.SetSecondaryCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }



        public static void ShowEmailErrorDialog(Activity context, string selectedAction, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("RegisterNew", "EmailHeaderError"))
                        .SetMessage(Utility.GetLocalizedLabel("RegisterNew", "EmailBodyError"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("RegisterNew", "Reset"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("RegisterNew", "tryAgain"))
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    confirmAction();
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowEmailVerificationDialog(Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Tnb_Profile", "verifyEmailPopupTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("Tnb_Profile", "verifyEmailPopupBody"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Tnb_Profile", "gotIt"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Tnb_Profile", "resend"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowEmailVerificationLoginDialog(Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("Login", "verifyEmailPopupTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("Login", "verifyEmailPopupBody"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Login", "gotIt"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("Login", "resend"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowEmailVerificationForgotPassDialog(Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER_TWO_BUTTON)
                        .SetTitle(Utility.GetLocalizedLabel("ForgotPassword", "verifyEmailPopupTitle"))
                        .SetMessage(Utility.GetLocalizedLabel("ForgotPassword", "verifyEmailPopupBody"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("ForgotPassword", "gotIt"))
                        .SetSecondaryCTALabel(Utility.GetLocalizedLabel("ForgotPassword", "resend"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowIdentificationErrorDialog(Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(Utility.GetLocalizedLabel("OneLastThing", "IdHeaderError"))
                        .SetMessage(Utility.GetLocalizedLabel("OneLastThing", "IdBodyError"))
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static void ShowInvalidEmailPasswordErrorDialog(string errorMessageTitle, string errorMessageDetails, Activity context, Action confirmAction, Action cancelAction = null)
        {
            MyTNBAppToolTipBuilder tooltipBuilder = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                        .SetTitle(errorMessageTitle)
                        .SetMessage(errorMessageDetails)
                        .SetContentGravity(Android.Views.GravityFlags.Left)
                        .SetCTALabel(Utility.GetLocalizedLabel("Common", "ok"))
                        .SetSecondaryCTAaction(() =>
                        {
                            confirmAction();
                        })
                        .Build();
            tooltipBuilder.SetCTAaction(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                    tooltipBuilder.DismissDialog();
                }
                else
                {
                    tooltipBuilder.DismissDialog();
                }
            }).Show();
        }

        public static bool IsMDMSDownEnergyBudget()
        {
            bool isMDMSEnable = true;

            DownTimeEntity smartmeterdailyEntity = DownTimeEntity.GetByCode(Constants.Smart_Meter_Daily_SYSTEM);
            DownTimeEntity smartmeter = DownTimeEntity.GetByCode(Constants.SMART_METER_SYSTEM);
            DownTimeEntity ebdowntime = DownTimeEntity.GetByCode(Constants.EB_SYSTEM);

            //if (smartmeterdailyEntity != null && smartmeterdailyEntity.IsDown)
            //{
            //    isMDMSEnable = false;
            //}
            //else
            //{
            //    if (smartmeter != null && smartmeter != null)
            //    {
            //        if (smartmeter.IsDown || smartmeterdailyEntity.IsDown)
            //        {
            //            isMDMSEnable = false;
            //        }
            //    }
            //}

            if (smartmeterdailyEntity != null && smartmeterdailyEntity.IsDown)
            {
                isMDMSEnable = false; //smart meter daily is down
            }
            else if (smartmeter != null && smartmeter.IsDown)
            {
                isMDMSEnable = false; //smart meter is down
            }
            else
            {
                if (ebdowntime != null)
                {
                    if (ebdowntime.IsDown)
                    {
                        isMDMSEnable = false; //energy budget is down
                    }
                    else
                    {
                        isMDMSEnable = true; //energy budget is not down
                    }
                }

            }

            return isMDMSEnable;
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
            //    GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, selectedLanguage.ToLower());// LanguageUtil.GetAppLanguage());
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

        public static void SetIsPayDisableNotFromAppLaunch(bool flag)
        {
            IsPayDisableNotFromAppLaunch = flag;
        }

        public static bool IsEnablePayment()
        {
            bool isPaymentEnable = true;

            if (IsPayDisableNotFromAppLaunch)
            {
                isPaymentEnable = false;
            }
            else
            {
                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                DownTimeEntity pgCCEntity = DownTimeEntity.GetByCode(Constants.PG_CC_SYSTEM);
                DownTimeEntity pgFPXEntity = DownTimeEntity.GetByCode(Constants.PG_FPX_SYSTEM);
                DownTimeEntity pgTNGEntity = DownTimeEntity.GetByCode(Constants.PG_TNG_SYSTEM);

                if (bcrmEntity != null && bcrmEntity.IsDown)
                {
                    isPaymentEnable = false;
                }
                else
                {
                    if (pgCCEntity != null && pgFPXEntity != null && pgTNGEntity != null)
                    {
                        if (pgCCEntity.IsDown && pgFPXEntity.IsDown && pgTNGEntity.IsDown)
                        {
                            isPaymentEnable = false;
                        }
                    }
                }
            }

            return isPaymentEnable;
        }

        public static string GetAppVersionName(Context appContext)
        {
            string appVersionName = "";
            try
            {
                var name = appContext.PackageManager.GetPackageInfo(appContext.PackageName, 0).VersionName;
                var code = appContext.PackageManager.GetPackageInfo(appContext.PackageName, 0).VersionCode;
                if (name != null)
                {
                    appVersionName = GetLocalizedLabel("Profile", "appVersion") + " " + name;
                }
#if DEBUG || STUB || DEVELOP || SIT
                appVersionName = appVersionName + "(" + code + ")";
#endif
            }
            catch (Exception e)
            {
                LoggingNonFatalError(e);
            }
            return appVersionName;
        }

        public static void SetAppUpdateId(Context appContext)
        {
            AppUpdateId = "AppUpdateId";
            try
            {
                var code = appContext.PackageManager.GetPackageInfo(appContext.PackageName, 0).VersionCode;
                AppUpdateId = AppUpdateId + code;
            }
            catch (Exception e)
            {
                LoggingNonFatalError(e);
            }
        }

        public static string StringIdMasking(Masking masking, string premasking)
        {
            //proceed when is not null or empty
            if (!String.IsNullOrEmpty(premasking))
            {
                if (masking.Equals(Masking.Id))
                {
                    string lastDigit = premasking.Substring(premasking.Length - 4);
                    int index = premasking.IndexOf(lastDigit);

                    string frontMasking = premasking.Substring(0, index);
                    Regex replaceString = new Regex("\\S");
                    frontMasking = replaceString.Replace(frontMasking, "•");
                    return frontMasking + lastDigit;
                   
                }
                else
                {
                    return premasking;
                }
            }
            else
            {
                return premasking;
            }
        }

        public static string StringMasking(Masking masking, string premasking)
        {
            //proceed when is not null or empty
            if (!String.IsNullOrEmpty(premasking))
            {
                if (masking.Equals(Masking.Address))
                {

                    int commaIndex = premasking.IndexOf(',');
                    if (commaIndex != -1)
                    {

                        string postMasking = premasking.Substring(commaIndex);
                        string frontMasking = premasking.Substring(0, commaIndex);
                        Regex replaceString = new Regex("\\S");
                        frontMasking = replaceString.Replace(frontMasking, "*");
                        return frontMasking + postMasking;
                    }
                    else
                    {
                        //premasking not contain any ,
                        return premasking;
                    }
                }
                else
                {
                    return premasking;
                }
            }
            else
            {
                return premasking;
            }
        }

        public static string StringSpaceMasking(Masking masking, string premasking)
        {

            //proceed when is not null or empty
            if (!String.IsNullOrEmpty(premasking))
            {
                if (masking.Equals(Masking.Address))
                {

                    char characterToMatch = (char)ConsoleKey.Spacebar;
                    int first = premasking.IndexOf(characterToMatch);
                    //need to +1 if not the the value are the same with 1st index
                    int second = premasking.IndexOf(characterToMatch, first + 1);
                    if (second != -1)
                    {
                        string postMasking = premasking.Substring(second);
                        string frontMasking = premasking.Substring(0, second);
                        Regex replaceString = new Regex("\\S");
                        frontMasking = replaceString.Replace(frontMasking, "*");
                        return frontMasking + postMasking;
                    }
                    else
                    {
                        //premasking not contain any ,
                        return premasking;
                    }
                }
                else
                {
                    return premasking;
                }
            }
            else
            {
                return premasking;
            }
        }

        public static string GetAppUpdateId()
        {
            return AppUpdateId;
        }

        public static void ShowBCRMDOWNTooltip(Activity context, DownTimeEntity downTimeEntity, Action confirmAction)
        {
            MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_IMAGE_BUTTON_WITH_HEADER)
                .SetHeaderImage(Resource.Drawable.maintenance_bcrm)
                .SetHeaderTitle(downTimeEntity.DowntimeTextMessage)
                .SetTitle("")
                .SetMessage(downTimeEntity.DowntimeMessage)
                .SetCTALabel(Utility.GetLocalizedLabel("Common", "gotIt"))
                .SetCTAaction(() =>
                {
                    confirmAction();
                })
                .Build();
            marketingTooltip.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using UIKit;

namespace myTNB
{
    public static class WhatsNewServices
    {
        public static bool FilterExpiredWhatsNew()
        {
            bool isExpired = false;
            WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
            var list = whatsNewEntity.GetAllItems();
            if (list != null && list.Count > 0)
            {
                foreach (var whatsNew in list)
                {
                    if (WhatsNewHasExpired(whatsNew))
                    {
                        isExpired = true;
                        whatsNewEntity.DeleteItem(whatsNew.ID);
                    }
                }
            }
            return isExpired;
        }

        public static bool WhatsNewHasExpired(WhatsNewModel whatsNew)
        {
            bool res = true;
            if (whatsNew != null && whatsNew.ID.IsValid())
            {
                if (whatsNew.EndDate.IsValid())
                {
                    try
                    {
                        DateTime endDate = DateTime.ParseExact(whatsNew.EndDate, "yyyyMMddTHHmmss", DateHelper.DateCultureInfo);
                        DateTime now = DateTime.Now;
                        if (now < endDate)
                        {
                            res = false;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Parse Error: " + e.Message);
                    }
                }
            }
            return res;
        }

        public static bool GetIsRead(string id)
        {
            bool isRead = false;
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, bool> dict = new Dictionary<string, bool>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewReadFlags);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            isRead = dict[id];
                        }
                    }
                }
            }
            return isRead;
        }

        public static void SetIsRead(string id)
        {
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, bool> dict = new Dictionary<string, bool>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewReadFlags);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(cachedData);
                    if (dict != null)
                    {
                        if (!dict.ContainsKey(id))
                        {
                            dict.Add(id, true);
                            var jsonStr = JsonConvert.SerializeObject(dict);
                            sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewReadFlags);
                            sharedPreference.Synchronize();
                        }
                    }
                    else
                    {
                        dict.Add(id, true);
                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewReadFlags);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    dict.Add(id, true);
                    var jsonStr = JsonConvert.SerializeObject(dict);
                    sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewReadFlags);
                    sharedPreference.Synchronize();
                }
            }
        }

        public static bool GetIsSkipAppLaunch(string id)
        {
            bool isSkip = false;
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, bool> dict = new Dictionary<string, bool>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewSkipModelFlags);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            isSkip = dict[id];
                        }
                    }
                }
            }
            return isSkip;
        }

        public static void SetIsSkipAppLaunch(string id, bool flag)
        {
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, bool> dict = new Dictionary<string, bool>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewSkipModelFlags);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            dict.Remove(id);
                        }

                        dict.Add(id, flag);
                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewSkipModelFlags);
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        dict.Add(id, flag);
                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewSkipModelFlags);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    dict.Add(id, flag);
                    var jsonStr = JsonConvert.SerializeObject(dict);
                    sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewSkipModelFlags);
                    sharedPreference.Synchronize();
                }
            }
        }

        public static string GetWhatNewModelShowDate(string id)
        {
            string dateTime = GetCurrentDate();
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewModelShowDate);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            dateTime = dict[id];
                            return dateTime;
                        }
                    }
                }

                SetWhatNewModelExactShowDate(id, dateTime);
            }
            return dateTime;
        }

        public static void SetWhatNewModelExactShowDate(string id, string dateTime)
        {
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewModelShowDate);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            dict.Remove(id);
                        }

                        dict.Add(id, dateTime);

                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        dict.Add(id, dateTime);

                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    dict.Add(id, dateTime);

                    var jsonStr = JsonConvert.SerializeObject(dict);
                    sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                    sharedPreference.Synchronize();
                }
            }
        }

        public static void SetWhatNewModelShowDate(string id, bool isDiffDate)
        {
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewModelShowDate);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedData);
                    if (dict != null)
                    {
                        string dateTime = GetCurrentDate();

                        if (!isDiffDate)
                        {
                            dateTime = GetWhatNewModelShowDate(id);
                        }

                        if (dict.ContainsKey(id))
                        {
                            dict.Remove(id);
                        }

                        dict.Add(id, dateTime);

                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        string dateTime = GetCurrentDate();

                        if (!isDiffDate)
                        {
                            dateTime = GetWhatNewModelShowDate(id);
                        }

                        dict.Add(id, dateTime);

                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    string dateTime = GetCurrentDate();

                    if (!isDiffDate)
                    {
                        dateTime = GetWhatNewModelShowDate(id);
                    }

                    dict.Add(id, dateTime);

                    var jsonStr = JsonConvert.SerializeObject(dict);
                    sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowDate);
                    sharedPreference.Synchronize();
                }
            }
        }

        public static int GetWhatNewModelShowCount(string id)
        {
            int count = 0;
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, int> dict = new Dictionary<string, int>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewModelShowCount);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            count = dict[id];
                        }
                    }
                }
            }
            return count;
        }

        public static void SetWhatNewModelShowCount(string id, int count)
        {
            if (id.IsValid())
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                Dictionary<string, int> dict = new Dictionary<string, int>();
                string cachedData = sharedPreference.StringForKey(WhatsNewConstants.Pref_WhatsNewModelShowCount);
                if (cachedData.IsValid())
                {
                    dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(cachedData);
                    if (dict != null)
                    {
                        if (dict.ContainsKey(id))
                        {
                            dict.Remove(id);
                        }

                        dict.Add(id, count);
                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowCount);
                        sharedPreference.Synchronize();
                    }
                    else
                    {
                        dict.Add(id, count);
                        var jsonStr = JsonConvert.SerializeObject(dict);
                        sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowCount);
                        sharedPreference.Synchronize();
                    }
                }
                else
                {
                    dict.Add(id, count);
                    var jsonStr = JsonConvert.SerializeObject(dict);
                    sharedPreference.SetString(jsonStr, WhatsNewConstants.Pref_WhatsNewModelShowCount);
                    sharedPreference.Synchronize();
                }
            }
        }

        private static string GetCurrentDate()
        {
            DateTime currentDate = DateTime.Now;
            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
            return currentDate.ToString(@"yyyyMMddTHHmmss", currCult);
        }

        public static string GetPublishedDate(string publishedDate)
        {
            string strPublishedDate = string.Empty;
            try
            {
                DateTime? endDate = DateTime.ParseExact(publishedDate, "yyyyMMddTHHmmss", DateHelper.DateCultureInfo);
                strPublishedDate = endDate.Value.ToString(WhatsNewConstants.Format_Date, DateHelper.DateCultureInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Parse Error: " + e.Message);
            }
            return strPublishedDate;
        }

        public static void OpenWhatsNewDetails(string whatsNewId, UIViewController topView)
        {
            if (WhatsNewCache.WhatsNewIsAvailable)
            {
                if (topView != null && !(topView is WhatsNewDetailsViewController))
                {
                    if (whatsNewId.IsValid())
                    {
                        var whatsNew = WhatsNewEntity.GetItem(whatsNewId);
                        if (whatsNew != null)
                        {
                            if (!WhatsNewHasExpired(whatsNew))
                            {
                                WhatsNewCache.RefreshWhatsNew = true;
                                WhatsNewDetailsViewController whatsNewDetailView = new WhatsNewDetailsViewController();
                                whatsNewDetailView.WhatsNewModel = whatsNew;
                                UINavigationController navController = new UINavigationController(whatsNewDetailView)
                                {
                                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                                };
                                topView.PresentViewController(navController, true, null);

                                var entityModel = whatsNew.ToEntity();
                                entityModel.IsRead = true;
                                WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                                whatsNewEntity.UpdateItem(entityModel);
                            }
                            else
                            {
                                ShowWhatsNewExpired(topView);
                            }
                        }
                        else
                        {
                            ShowWhatsNewExpired(topView);
                        }
                    }
                    else
                    {
                        ShowWhatsNewExpired(topView);
                    }
                }
            }
            else
            {
                ShowWhatsNewUnavailable();
            }
        }

        public static void OpenWhatsNewDetailsInDetails(string whatsNewId, UIViewController topView)
        {
            if (WhatsNewCache.WhatsNewIsAvailable)
            {
                if (whatsNewId.IsValid())
                {
                    var whatsNew = WhatsNewEntity.GetItem(whatsNewId);
                    if (whatsNew != null)
                    {
                        if (!WhatsNewHasExpired(whatsNew))
                        {
                            WhatsNewCache.RefreshWhatsNew = true;
                            WhatsNewDetailsViewController whatsNewDetailView = new WhatsNewDetailsViewController();
                            whatsNewDetailView.WhatsNewModel = whatsNew;
                            UINavigationController navController = new UINavigationController(whatsNewDetailView)
                            {
                                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                            };
                            topView.PresentViewController(navController, true, null);

                            var entityModel = whatsNew.ToEntity();
                            entityModel.IsRead = true;
                            WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                            whatsNewEntity.UpdateItem(entityModel);
                        }
                        else
                        {
                            ShowWhatsNewExpired(topView);
                        }
                    }
                    else
                    {
                        ShowWhatsNewExpired(topView);
                    }
                }
                else
                {
                    ShowWhatsNewExpired(topView);
                }
            }
            else
            {
                ShowWhatsNewUnavailable();
            }
        }

        public static void ShowWhatsNewExpired(UIViewController topView)
        {
            if (topView != null)
            {
                topView.InvokeOnMainThread(() =>
                {
                    AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewExpiredTitle),
                        LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewExpiredMsg),
                        new Dictionary<string, Action> {
                            { LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewExpiredBtnText), () =>
                            {
                                if (topView is HomeTabBarController)
                                {
                                    HomeTabBarController tabBar = topView as HomeTabBarController;
                                    tabBar.SelectedIndex = 2;
                                }
                                else if (topView.TabBarController != null)
                                {
                                    topView.TabBarController.SelectedIndex = 2;
                                }
                                WhatsNewCache.RefreshWhatsNew = true;
                            } }});
                });
            }
        }

        public static void ShowWhatsNewUnavailable()
        {
            AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewUnavailableTitle),
                LanguageUtility.GetErrorI18NValue(Constants.Error_WhatsNewUnavailableMsg),
                new Dictionary<string, Action> {
                    { LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), null}});
        }
    }
}

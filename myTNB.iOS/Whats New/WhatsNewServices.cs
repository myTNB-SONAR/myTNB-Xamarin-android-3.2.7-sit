using System;
using System.Collections.Generic;
using System.Diagnostics;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
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
                    var whatsNewEndDate = DateHelper.GetDateWithoutSeparator(whatsNew.EndDate);
                    if (whatsNewEndDate != default(DateTime))
                    {
                        DateTime now = DateTime.Now.Date;
                        if (now < whatsNewEndDate)
                        {
                            res = false;
                        }
                    }
                }
            }
            return res;
        }

        public static string GetPublishedDate(string publishedDate)
        {
            string strPublishedDate = string.Empty;
            try
            {
                DateTime? pDate = DateHelper.GetDateWithoutSeparator(publishedDate);
                DateTime? pDateValue = pDate.Value.ToLocalTime();
                strPublishedDate = pDateValue.Value.ToString(WhatsNewConstants.Format_Date, DateHelper.DateCultureInfo);
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

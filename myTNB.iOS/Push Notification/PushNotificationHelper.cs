using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using Firebase.Core;
using Firebase.InstanceID;
using Foundation;
using myTNB.Dashboard;
using myTNB.Model;
using myTNB.PushNotification;
using UIKit;

namespace myTNB
{
    public static class PushNotificationHelper
    {
        private static UserNotificationResponseModel _userNotifications = new UserNotificationResponseModel();
        /// <summary>
        /// Registers the device.
        /// </summary>
        public static void RegisterDevice()
        {
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            InstanceId.Notifications.ObserveTokenRefresh((sender, e) =>
            {
                string token = InstanceId.SharedInstance.Token;
                Debug.WriteLine("FCM Token: " + token);
                DataManager.DataManager.SharedInstance.FCMToken = token;
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetString(token, "FCMToken");
                sharedPreference.Synchronize();
                ConnectToFCM();
            });

            App.Configure();
        }
        /// <summary>
        /// Connects to FIREBASE CLOUD MESSAGING.
        /// </summary>
        public static void ConnectToFCM()
        {
            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;
        }

        /// <summary>
        /// Handles the push notification.
        /// </summary>
        public static void HandlePushNotification()
        {
            try
            {
                if (DataManager.DataManager.SharedInstance.IsFromPushNotification)
                {
                    DataManager.DataManager.SharedInstance.IsFromPushNotification = false;
                    DataManager.DataManager.SharedInstance.NotificationNeedsUpdate = true;
                    if (PushNotificationCache.IsODN || PushNotificationCache.IsValidEmail)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                        PushNotificationViewController viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                        UINavigationController navController = new UINavigationController(viewController);

                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);

                        if (topVc != null)
                        {
                            if (!(topVc is DashboardHomeViewController) && !(topVc is DashboardViewController))
                            {
                                HomeTabBarController tabBar = ViewHelper.DismissControllersAndSelectTab(topVc, 0, false, true);
                                if (tabBar != null)
                                {
                                    if (tabBar.SelectedViewController is DashboardNavigationController selVc)
                                    {
                                        if (selVc != null && selVc.ViewControllers.Length > 0)
                                        {
                                            UIViewController vc = selVc.ViewControllers[0];

                                            if ((vc is DashboardHomeViewController) || (vc is DashboardViewController))
                                            {
                                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                                vc.PresentViewController(navController, true, null);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                topVc.PresentViewController(navController, true, null);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns>The notif.</returns>
        public static async Task<bool> GetNotifications(bool needsBadgeUpdate = true)
        {
            bool res = false;
            await GetUserNotifications();

            DataManager.DataManager.SharedInstance.UserNotificationResponse = _userNotifications;
            res = _userNotifications?.d?.didSucceed == true;

            if (_userNotifications != null && _userNotifications?.d != null && _userNotifications.d.IsSuccess
                && _userNotifications.d.data != null && _userNotifications.d.data.UserNotificationList != null)
            {
                DataManager.DataManager.SharedInstance.UserNotifications = _userNotifications.d.data.UserNotificationList;
                DataManager.DataManager.SharedInstance.NotificationNeedsUpdate = false;
            }
            else
            {
                DataManager.DataManager.SharedInstance.UserNotifications = new List<UserNotificationDataModel>();
            }
            if (DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                int index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.IsRead.ToLower() == "false");
                DataManager.DataManager.SharedInstance.HasNewNotification = index > -1;
            }
            else
            {
                DataManager.DataManager.SharedInstance.HasNewNotification = false;
            }
            if (needsBadgeUpdate)
            {
                UpdateApplicationBadge();
            }
            return res;
        }
        /// <summary>
        /// Gets the notification count.
        /// </summary>
        /// <returns>The notification count.</returns>
        public static int GetNotificationCount()
        {
            if (DataManager.DataManager.SharedInstance.UserNotifications != null
                && DataManager.DataManager.SharedInstance.UserNotifications?.Count > 0)
            {
                try
                {
                    int count = DataManager.DataManager.SharedInstance.UserNotifications.FindAll(x => x.IsAccountNumberExist && x.IsRead.ToLower().Equals("false")).Count;
                    return count < 0 ? 0 : count;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return 0;
                }
            }
            return 0;
        }

        private static Task GetUserNotifications()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf
                };

                //UserNotificationManager.SetData();
                //_userNotifications = Newtonsoft.Json.JsonConvert.DeserializeObject<UserNotificationResponseModel>(UserNotificationManager.GetData());
                _userNotifications = serviceManager.OnExecuteAPIV6<UserNotificationResponseModel>
                    (PushNotificationConstants.Service_GetUserNotifications, requestParameter);
            });
        }
        /// <summary>
        /// Gets the app notification types.
        /// </summary>
        /// <returns>The app notification types.</returns>
        public static Task GetAppNotificationTypes()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf
                };
                NotificationTypeResponseModel response = serviceManager.OnExecuteAPI<NotificationTypeResponseModel>
                    (PushNotificationConstants.Service_GetAppNotificationTypes, requestParameter);
                DataManager.DataManager.SharedInstance.NotificationGeneralTypes = response?.d?.data;
                NotificationPreferenceModel allNotificationItem = new NotificationPreferenceModel
                {
                    Title = LanguageUtility.GetCommonI18NValue(Constants.Common_AllNotifications),
                    Id = "all"
                };
                if (DataManager.DataManager.SharedInstance.NotificationGeneralTypes != null)
                {
                    DataManager.DataManager.SharedInstance.NotificationGeneralTypes.Insert(0, allNotificationItem);
                }
            });
        }
        /// <summary>
        /// Gets the user notification preferences.
        /// </summary>
        public static void GetUserNotificationPreferences()
        {
            Task[] taskList = {
                GetUserNotificationTypes(),
                GetUserNotificationChannels()
            };
            Task.WaitAll(taskList);
        }

        static Task GetUserNotificationTypes()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf
                };
                DataManager.DataManager.SharedInstance.NotificationTypeResponse =
                serviceManager.OnExecuteAPIV6<NotificationTypeResponseModel>
                    (PushNotificationConstants.Service_GetUserNotificationTypePreferences, requestParameter);
            });
        }

        private static Task GetUserNotificationChannels()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf
                };
                DataManager.DataManager.SharedInstance.NotificationChannelResponse =
                serviceManager.OnExecuteAPIV6<NotificationChannelResponseModel>
                    (PushNotificationConstants.Service_GetUserNotificationChannelPreferences, requestParameter);
            });
        }

        public static string GetNotificationImage()
        {
            if (DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                int index = DataManager.DataManager.SharedInstance.UserNotifications.FindIndex(x => x.IsRead.ToLower() == "false");
                DataManager.DataManager.SharedInstance.HasNewNotification = index > -1;
            }
            else
            {
                DataManager.DataManager.SharedInstance.HasNewNotification = false;
            }
            return DataManager.DataManager.SharedInstance.HasNewNotification ? "Notification-New" : "Notification";
        }

        public static void UpdateApplicationBadge()
        {
            int unreadCount = GetNotificationCount();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = unreadCount;
        }
    }
}

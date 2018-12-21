using System;
using System.Collections.Generic;
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
        static UserNotificationResponseModel _userNotifications = new UserNotificationResponseModel();
        /// <summary>
        /// Registers the device.
        /// </summary>
        public static void RegisterDevice()
        {
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            InstanceId.Notifications.ObserveTokenRefresh((sender, e) =>
            {
                var token = InstanceId.SharedInstance.Token;
                Console.WriteLine("FCM Token: " + token);
                DataManager.DataManager.SharedInstance.FCMToken = token;
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
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

                    UIStoryboard storyBoard = UIStoryboard.FromName("PushNotification", null);
                    var viewController = storyBoard.InstantiateViewController("PushNotificationViewController") as PushNotificationViewController;
                    var navController = new UINavigationController(viewController);

                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);

                    if (!(topVc is DashboardHomeViewController) && !(topVc is DashboardViewController))
                    {
                        var tabBar = ViewHelper.DismissControllersAndSelectTab(topVc, 0, false, true);

                        if (tabBar != null)
                        {
                            if (tabBar.SelectedViewController is DashboardNavigationController selVc)
                            {
                                var vc = selVc.ViewControllers[0];

                                if ((vc is DashboardHomeViewController) || (vc is DashboardViewController))
                                {
                                    vc.PresentViewController(navController, true, null);
                                }
                            }
                        }

                    }
                    else
                    {
                        topVc.PresentViewController(navController, true, null);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        public static void GetNotifications()
        {
            Task.WaitAll(new Task[] { GetUserNotifications() });
            if (_userNotifications != null && _userNotifications.d != null
               && _userNotifications.d.isError.ToLower() == "false" && _userNotifications.d.status.ToLower() == "success")
            {
                DataManager.DataManager.SharedInstance.UserNotifications = _userNotifications.d.data;
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
            int unreadCount = GetNotificationCount();
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = unreadCount;
        }
        /// <summary>
        /// Gets the notification count.
        /// </summary>
        /// <returns>The notification count.</returns>
        public static int GetNotificationCount()
        {
            if (DataManager.DataManager.SharedInstance.UserNotifications != null
               && DataManager.DataManager.SharedInstance.UserNotifications.Count > 0)
            {
                try
                {
                    int count = DataManager.DataManager.SharedInstance.UserNotifications.FindAll(x => x.IsRead.ToLower().Equals("false")).Count;
                    if (count < 0)
                    {
                        count = 0;
                    }
                    return count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return 0;
                }
            }
            return 0;
        }

        static Task GetUserNotifications()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID
                };
                _userNotifications = serviceManager.GetUserNotifications("GetUserNotifications", requestParameter);
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
                    apiKeyID = TNBGlobal.API_KEY_ID
                };
                var response = serviceManager.GetNotificationTypes("GetAppNotificationTypes", requestParameter);
                DataManager.DataManager.SharedInstance.NotificationGeneralTypes = response?.d?.data;
                NotificationPreferenceModel allNotificationItem = new NotificationPreferenceModel();
                allNotificationItem.Title = "All notifications";
                allNotificationItem.Id = "all";
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
            Task[] taskList = new Task[]{
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
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID
                };
                DataManager.DataManager.SharedInstance.NotificationTypeResponse = serviceManager.GetNotificationTypes("GetUserNotificationTypePreferences", requestParameter);
            });
        }

        static Task GetUserNotificationChannels()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email
                };
                DataManager.DataManager.SharedInstance.NotificationChannelResponse = serviceManager.GetNotificationChannels("GetUserNotificationChannelPreferences", requestParameter);
            });
        }
    }
}
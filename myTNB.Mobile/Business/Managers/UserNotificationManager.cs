using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace myTNB
{
    public sealed class UserNotificationManager
    {
        private static readonly Lazy<UserNotificationManager> lazy = new Lazy<UserNotificationManager>(() => new UserNotificationManager());
        public static UserNotificationManager Instance { get { return lazy.Value; } }

        private static string JSONData = string.Empty;
        private const string NOTIFICATION_RESOURCE_PATH = "myTNB.Mobile.Resources.Notification.GetUserNotifications.json";
        private const string NOTIFICATION_INFO_PATH = "myTNB.Mobile.Resources.Notification.NotificationInfo_{0}.json";

        public static void SetData()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(NOTIFICATION_RESOURCE_PATH);
                using (StreamReader reader = new StreamReader(stream))
                {
                    JSONData = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> JSONData: " + JSONData);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> SetData: " + e.Message);
                JSONData = string.Empty;
            }
        }

        public static string GetData()
        {
            return JSONData;
        }

        public static string GetInfo(string id)
        {
            string info = string.Empty;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(string.Format(NOTIFICATION_INFO_PATH, id));
                using (StreamReader reader = new StreamReader(stream))
                {
                    info = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> info: " + info);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> GetInfo: " + e.Message);
                info = string.Empty;
            }
            return info;
        }
    }
}
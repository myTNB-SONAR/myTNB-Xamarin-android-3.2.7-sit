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
    }
}
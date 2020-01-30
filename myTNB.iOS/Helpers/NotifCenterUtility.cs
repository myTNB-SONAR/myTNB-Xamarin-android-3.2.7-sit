using System;
using System.Diagnostics;
using Foundation;

namespace myTNB
{
    public static class NotifCenterUtility
    {
        public static void AddObserver(NSString aName, Action<NSNotification> notify)
        {
            try
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(aName);
            }
            catch (Exception e) { Debug.WriteLine("AddObserver Error: " + e.Message); }
            NSNotificationCenter.DefaultCenter.AddObserver(aName, notify);
        }

        public static void PostNotificationName(string aName, NSObject anObject)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName(aName, anObject);
        }
    }
}

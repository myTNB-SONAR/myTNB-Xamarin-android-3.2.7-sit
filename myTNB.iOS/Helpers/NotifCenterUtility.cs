using System;
using Foundation;

namespace myTNB
{
    public static class NotifCenterUtility
    {
        public static void AddObserver(NSString aName, Action<NSNotification> notify)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(aName);
            NSNotificationCenter.DefaultCenter.AddObserver(aName, notify);
        }

        public static void PostNotificationName(string aName, NSObject anObject)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName(aName, anObject);
        }
    }
}

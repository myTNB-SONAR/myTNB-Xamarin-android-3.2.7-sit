﻿using System.Collections.Generic;
using Android.OS;
using Android.Widget;
using Firebase.Analytics;

namespace myTNB_Android.Src.Utils
{
	public static class FirebaseAnalyticsUtils
	{
		private static FirebaseAnalytics mAnalytics;

		public static void SetAnalytics(this FirebaseAnalytics currentAnalytics)
		{
			mAnalytics = currentAnalytics;
		}

		public static void SetScreenName(this Android.App.Activity mActivity, string screenName)
		{
			SetAnalytics(FirebaseAnalytics.GetInstance(mActivity));
			mAnalytics.SetCurrentScreen(mActivity, screenName, mActivity.Class.SimpleName);
		}

		public static void SetFragmentScreenName(this Android.App.Fragment mFragment, string screenName)
		{
			SetAnalytics(FirebaseAnalytics.GetInstance(mFragment.Activity));
			mAnalytics.SetCurrentScreen(mFragment.Activity, screenName, mFragment.Class.SimpleName);
		}

		public static List<FirebaseCustomEvent> GetFormattedEvent(string objectList)
		{
			// Dump the event string like this format below:
			// button_name;test123||button_type;abc123
			List<FirebaseCustomEvent> mFirebaseEvents = new List<FirebaseCustomEvent>();
			string[] splittedString = objectList.Split("||");
			foreach (string objectString in splittedString)
			{
				string[] spllittedObject = objectString.Split(";");
				if (splittedString.Length == 2)
				{
					mFirebaseEvents.Add(new FirebaseCustomEvent()
					{
						ObjectType = splittedString[0].Trim(),
						ObjectValue = splittedString[1].Trim()
					});
				}
			}
			return mFirebaseEvents;
		}

		public static void LogEvent(string eventName, List<FirebaseCustomEvent> objectLogs)
		{
			Bundle mBundle = new Bundle();
			foreach (FirebaseCustomEvent objectLog in objectLogs)
			{
				mBundle.PutString(objectLog.ObjectType, objectLog.ObjectValue);
			}
			mAnalytics.LogEvent(eventName, mBundle);
		}

        public static void LogClickEvent(this Android.App.Activity mActivity, string eventName)
        {
            SetAnalytics(FirebaseAnalytics.GetInstance(mActivity));
            Bundle bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.ItemName, eventName);
            mAnalytics.LogEvent(FirebaseAnalytics.Event.SelectContent, bundle);
        }

        public static void LogFragmentClickEvent(this Android.App.Fragment mFragment, string eventName)
        {
            SetAnalytics(FirebaseAnalytics.GetInstance(mFragment.Activity));
            Bundle bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.ItemName, eventName);
            mAnalytics.LogEvent(FirebaseAnalytics.Event.SelectContent, bundle);
        }
    }

	public class FirebaseCustomEvent
	{
		public string ObjectType { get; set; }

		public string ObjectValue { get; set; }
	}
}

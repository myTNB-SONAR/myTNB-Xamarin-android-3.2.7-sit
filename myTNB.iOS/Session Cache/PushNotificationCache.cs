using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using Newtonsoft.Json;
using UserNotifications;

namespace myTNB
{
	public sealed class PushNotificationCache
	{
		private static readonly Lazy<PushNotificationCache> lazy = new Lazy<PushNotificationCache>(() => new PushNotificationCache());
		public static PushNotificationCache Instance { get { return lazy.Value; } }

		public static void SetData(UNNotificationResponse response)
		{
			if (IsValidResponse(response))
			{
				NSDictionary dictionary = response.Notification.Request.Content.UserInfo;
				if (dictionary != null)
				{
					UserNotificationDictionary = DictionaryConvert(dictionary);
				}
			}
		}

		private static Dictionary<string, string> DictionaryConvert(NSDictionary nativeDict)
		{
			try
			{
				return nativeDict.ToDictionary<KeyValuePair<NSObject, NSObject>, string, string>(
					item => (NSString)item.Key, item => item.Value.ToString());
			}
			catch (Exception e)
			{
				Debug.WriteLine("Error DictionaryConvert: " + e.Message);
				return new Dictionary<string, string>();
			}
		}

		private static bool IsValidResponse(UNNotificationResponse response)
		{
			return response != null
				&& response.Notification != null
				&& response.Notification.Request != null
				&& response.Notification.Request.Content != null
				&& response.Notification.Request.Content.UserInfo != null;
		}

		public static Dictionary<string, string> UserNotificationDictionary
		{
			private set; get;
		}

		public static bool IsODN
		{
			get
			{
				return UserNotificationDictionary != null
					&& UserNotificationDictionary.ContainsKey("Type")
					&& UserNotificationDictionary["Type"] == "ODN";
			}
		}

		public static bool IsValidEmail
		{
			get
			{
				if (DataManager.DataManager.SharedInstance.IsLoggedIn()
					&& UserNotificationDictionary.ContainsKey("Email")
					&& !string.IsNullOrEmpty(UserNotificationDictionary["Email"])
					&& !string.IsNullOrWhiteSpace(UserNotificationDictionary["Email"])
					&& DataManager.DataManager.SharedInstance.UserEntity != null
					&& DataManager.DataManager.SharedInstance.UserEntity.Count > 0
					&& DataManager.DataManager.SharedInstance.UserEntity[0] != null
					&& !string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.UserEntity[0].email)
					&& !string.IsNullOrWhiteSpace(DataManager.DataManager.SharedInstance.UserEntity[0].email))
				{
					return DataManager.DataManager.SharedInstance.UserEntity[0].email.ToLower()
						== UserNotificationDictionary["Email"].ToLower();
				}

				return false;
			}
		}
	}
}
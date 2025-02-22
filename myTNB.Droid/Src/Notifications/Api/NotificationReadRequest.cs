﻿using System.Collections.Generic;
using myTNB.AndroidApp.Src.Notifications.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.Notifications.Api
{
	public class NotificationReadRequest
	{
		[JsonProperty("ApiKeyID")]
		public string ApiKeyID { get; set; }

		[JsonProperty("Email")]
		public string Email { get; set; }

		[JsonProperty("DeviceId")]
		public string DeviceId { get; set; }

		[JsonProperty("SSPUserId")]
		public string SSPUserId { get; set; }

		[JsonProperty("UpdatedNotifications")]
		public List<NotificationData> UpdatedNotifications { get; set; }
	}
}

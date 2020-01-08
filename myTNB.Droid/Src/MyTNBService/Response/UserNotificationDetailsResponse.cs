using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationDetailsResponse
    {
		[JsonProperty(PropertyName = "d")]
		public APIResponse Data { get; set; }

		public class APIResponse : APIBaseResponse
		{
			[JsonProperty(PropertyName = "data")]
			public UserNotificationDetailsResponseData ResponseData { get; set; }

			[JsonProperty(PropertyName = "RefreshTitle")]
			public string RefreshTitle { get; set; }

			[JsonProperty(PropertyName = "RefreshMessage")]
			public string RefreshMessage { get; set; }

			[JsonProperty(PropertyName = "RefreshBtnText")]
			public string RefreshBtnText { get; set; }

            [JsonProperty(PropertyName = "IsPayEnabled")]
            public bool IsPayEnabled { get; set; }
        }

        public class UserNotificationDetailsResponseData
        {
            [JsonProperty(PropertyName = "UserNotification")]
            public NotificationDetails.Models.NotificationDetails UserNotificationDetail { get; set; }
        }
    }
}

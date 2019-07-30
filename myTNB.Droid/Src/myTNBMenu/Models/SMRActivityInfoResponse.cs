﻿using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Models
{
	public class SMRActivityInfoResponse
	{

		[JsonProperty("d")]
		public SMRActivityInfo Response { get; set; }

		public class SMRActivityInfo
        {
			[JsonProperty(PropertyName = "__type")]
			public string Type { get; set; }

			[JsonProperty(PropertyName = "ErrorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty(PropertyName = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayMessage")]
			public string DisplayMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayType")]
			public string DisplayType { get; set; }

			[JsonProperty(PropertyName = "DisplayTitle")]
			public string DisplayTitle { get; set; }

			[JsonProperty(PropertyName = "data")]
			public SMRDashboardHistory Data { get; set; }
		}
	}
}
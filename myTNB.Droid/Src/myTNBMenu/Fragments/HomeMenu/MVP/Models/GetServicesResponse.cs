﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models
{
	public class GetServicesResponse
    {

		[JsonProperty("d")]
		public GetServicesData Data { get; set; }

		public class GetServicesData
		{
			[JsonProperty(PropertyName = "ErrorCode")]
			public string ErrorCode { get; set; }

			[JsonProperty(PropertyName = "ErrorMessage")]
			public string ErrorMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayMessage")]
			public string DisplayMessage { get; set; }

			[JsonProperty(PropertyName = "DisplayType")]
			public string DisplayType { get; set; }

			[JsonProperty(PropertyName = "data")]
			public List<MyService> Data { get; set; }
		}
	}
}
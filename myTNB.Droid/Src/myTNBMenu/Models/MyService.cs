﻿using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Models
{
	public class MyService
	{
		[JsonProperty("Id")]
		public string Id { get; set; }

		[JsonProperty("MyServiceString")]
		public string MyServiceTitle { get; set; }

	}
}
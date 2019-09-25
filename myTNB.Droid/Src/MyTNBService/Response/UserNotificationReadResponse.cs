using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class UserNotificationReadResponse
    {
		[JsonProperty(PropertyName = "d")]
		public APIResponse Data { get; set; }

		public class APIResponse : APIBaseResponse
		{

		}
	}
}

﻿using System.Collections.Generic;
using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Requests
{
	public class CheckPendingPaymentRequest
    {
		[JsonProperty("accounts")]
		public List<string> AccountList { get; set; }

		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }

        [JsonProperty("deviceInf")]
        public DeviceInterface deviceInf { get; set; }
    }
}
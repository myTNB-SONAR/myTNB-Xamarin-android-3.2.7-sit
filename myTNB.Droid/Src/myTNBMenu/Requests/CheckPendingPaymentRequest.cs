using System.Collections.Generic;
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
	public class CheckPendingPaymentRequest
    {
		[JsonProperty("accounts")]
		public List<string> AccountList { get; set; }

		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }

        [JsonProperty("dvcInf")]
        public DeviceInterface dvcInf { get; set; }
    }
}
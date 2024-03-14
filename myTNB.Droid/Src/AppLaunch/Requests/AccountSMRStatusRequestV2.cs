using System.Collections.Generic;
using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Requests
{
	public class AccountSMRStatusRequestV2
	{
		[JsonProperty("contractAccounts")]
		public List<string> ContractAccounts { get; set; }

		[JsonProperty("usrInf")]
		public UserInterface UserInterface { get; set; }

		[JsonProperty("Indicator")]
		public string Indicator { get; set; }
	}
}


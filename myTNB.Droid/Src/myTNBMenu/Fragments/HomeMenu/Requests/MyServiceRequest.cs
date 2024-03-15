using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Requests
{
	public class GetServiceRequests
	{
		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }
	}
}
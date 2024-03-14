using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Fragments.HomeMenu.Requests
{
	public class GetServiceRequests
	{
		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }
	}
}
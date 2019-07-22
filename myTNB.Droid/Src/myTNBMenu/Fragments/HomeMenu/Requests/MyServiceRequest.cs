using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Requests
{
	public class GetServiceRequests
	{
		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }
	}
}
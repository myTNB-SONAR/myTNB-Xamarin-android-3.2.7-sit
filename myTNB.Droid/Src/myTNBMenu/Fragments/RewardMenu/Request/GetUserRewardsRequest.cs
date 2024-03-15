using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request
{
	public class GetUserRewardsRequest
    {
		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }
	}
}
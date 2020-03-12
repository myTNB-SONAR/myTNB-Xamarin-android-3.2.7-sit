using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request
{
	public class GetUserRewardsRequest
    {
		[JsonProperty("usrInf")]
		public UserInterface usrInf { get; set; }
	}
}
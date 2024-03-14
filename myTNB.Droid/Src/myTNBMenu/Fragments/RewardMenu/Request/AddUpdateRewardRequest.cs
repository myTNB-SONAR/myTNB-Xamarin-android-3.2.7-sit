using myTNB.Android.Src.Base.Models;
using myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.Fragments.RewardMenu.Request
{
    public class AddUpdateRewardRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }

        [JsonProperty("reward")]
        public AddUpdateRewardModel reward { get; set; }
    }
}
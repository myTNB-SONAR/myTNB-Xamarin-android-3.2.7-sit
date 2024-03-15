using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Request
{
    public class AddUpdateRewardRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }

        [JsonProperty("reward")]
        public AddUpdateRewardModel reward { get; set; }
    }
}
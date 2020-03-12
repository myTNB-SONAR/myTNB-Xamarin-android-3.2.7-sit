using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Fragments.RewardMenu.Request
{
    public class AddUpdateRewardRequest
    {
        [JsonProperty("usrInf")]
        public UserInterface usrInf { get; set; }

        [JsonProperty("reward")]
        public AddUpdateRewardModel reward { get; set; }
    }
}
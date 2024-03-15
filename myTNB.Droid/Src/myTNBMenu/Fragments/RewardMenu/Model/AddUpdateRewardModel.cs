using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Model
{
	public class AddUpdateRewardModel
    {
        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("RewardId")]
        public string RewardId { get; set; }

        [JsonProperty("Read")]
        public bool Read { get; set; }

        [JsonProperty("ReadDate")]
        public string ReadDate { get; set; }

        [JsonProperty("Favourite")]
        public bool Favourite { get; set; }

        [JsonProperty("FavUpdatedDate")]
        public string FavUpdatedDate { get; set; }

        [JsonProperty("Redeemed")]
        public bool Redeemed { get; set; }

        [JsonProperty("RedeemedDate")]
        public string RedeemedDate { get; set; }
    }
}
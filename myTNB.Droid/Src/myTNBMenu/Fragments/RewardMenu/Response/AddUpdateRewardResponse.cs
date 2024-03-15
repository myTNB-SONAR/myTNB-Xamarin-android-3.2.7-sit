using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.RewardMenu.Response
{
    public class AddUpdateRewardResponse
    {

        [JsonProperty("d")]
        public AddUpdateRewardData Data { get; set; }

        public class AddUpdateRewardData
        {
            [JsonProperty(PropertyName = "ErrorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty(PropertyName = "ErrorMessage")]
            public string ErrorMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayMessage")]
            public string DisplayMessage { get; set; }

            [JsonProperty(PropertyName = "DisplayType")]
            public string DisplayType { get; set; }
        }
    }
}
using Newtonsoft.Json;
namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostGetAutoOptInCaRequest
    {
        [JsonProperty("CaNo")]
        public string CaNo { set; get; }
        [JsonProperty("UserId")]
        public string UserId { set; get; }

    }
}

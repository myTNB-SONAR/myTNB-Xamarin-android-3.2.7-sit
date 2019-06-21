using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class AccountDueAmountRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("accNum")]
        public string AccNum { get; set; }
    }
}
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageCards.Request
{
    public class RemoveRegisteredCardRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("registeredCardId")]
        public string RegisteredCardId { get; set; }
    }
}
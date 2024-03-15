using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.ManageCards.Request
{
    public class RemoveRegisteredCardRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("registeredCardId")]
        public string RegisteredCardId { get; set; }
    }
}
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.UpdateNickname.Request
{
    public class UpdateLinkedAccountNickNameRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("sspUserId")]
        public string UserId { get; set; }

        [JsonProperty("accountNo")]
        public string AccountNo { get; set; }

        [JsonProperty("oldAccountNickName")]
        public string OldAccountNickName { get; set; }

        [JsonProperty("newAccountNickName")]
        public string NewAccountNickName { get; set; }
    }
}
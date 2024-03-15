using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Requests
{
    public class REPaymentHistoryRequest
    {
        [JsonProperty("ApiKeyID")]
        [AliasAs("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [JsonProperty("AccountNumber")]
        [AliasAs("AccountNumber")]
        public string AccountNum { get; set; }

        [JsonProperty("IsOwner")]
        [AliasAs("IsOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("Email")]
        [AliasAs("Email")]
        public string Email { get; set; }

        public REPaymentHistoryRequest()
        {
        }


    }
}
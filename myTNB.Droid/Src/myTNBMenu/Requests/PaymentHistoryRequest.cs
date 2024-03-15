using myTNB.AndroidApp.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.myTNBMenu.Requests
{
    public class PaymentHistoryRequest : BaseRequest
    {

        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("isOwner")]
        [AliasAs("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

        public PaymentHistoryRequest(string apiKeyID) : base(apiKeyID)
        {
        }


    }
}
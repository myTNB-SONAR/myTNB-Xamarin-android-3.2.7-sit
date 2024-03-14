using myTNB.Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.myTNBMenu.Requests
{
    public class BillHistoryRequest : BaseRequest
    {
        public BillHistoryRequest(string apiKeyID) : base(apiKeyID)
        {
        }

        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("isOwner")]
        [AliasAs("isOwner")]
        public bool IsOwner { get; set; }

        [JsonProperty("email")]
        [AliasAs("email")]
        public string Email { get; set; }

    }
}
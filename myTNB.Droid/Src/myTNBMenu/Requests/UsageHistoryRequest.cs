using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class UsageHistoryRequest : BaseRequest
    {
        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        public UsageHistoryRequest(string apiKeyID) : base(apiKeyID)
        {

        }
    }
}
using myTNB_Android.Src.Base.Models;
using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class SMUsageHistoryRequest : BaseRequest
    {
        // Lin Siong TODO: new api format request

        [JsonProperty("accNum")]
        [AliasAs("accNum")]
        public string AccountNum { get; set; }

        [JsonProperty("userEmail")]
        [AliasAs("userEmail")]
        public string userEmail { get; set; }

        [JsonProperty("sspUserId")]
        [AliasAs("sspUserId")]
        public string sspUserId { get; set; }

        [JsonProperty("isOwner")]
        [AliasAs("isOwner")]
        public bool isOwner { get; set; }


        [JsonProperty("metercode")]
        [AliasAs("metercode")]
        public string MeterCode { get; set; }

        public SMUsageHistoryRequest(string apiKeyID) : base(apiKeyID)
        {

        }
    }
}
using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Requests
{
    public class GetInstallationDetailsRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("accNum")]
        public string AccNum { get; set; }
    }
}

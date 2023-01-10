using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR.GetBillRendering
{
    public class GetBillRenderingTenantRequest
    {
        [JsonProperty("CaNos")]
        public List<string> CaNos { set; get; }
        [JsonProperty("UserId")]
        public string UserId { set; get; }

    }
}

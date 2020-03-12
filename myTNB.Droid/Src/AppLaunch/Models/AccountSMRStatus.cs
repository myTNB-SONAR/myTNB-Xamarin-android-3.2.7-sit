using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class AccountSMRStatus
    {
        [JsonProperty(PropertyName = "ContractAccount")]
        [AliasAs("ContractAccount")]
        public string ContractAccount { get; set; }

        [JsonProperty(PropertyName = "IsTaggedSMR")]
        [AliasAs("IsTaggedSMR")]
        public string IsTaggedSMR { get; set; }

        [JsonProperty(PropertyName = "IsPeriodOpen")]
        [AliasAs("IsPeriodOpen")]
        public string IsPeriodOpen { get; set; }

    }
}
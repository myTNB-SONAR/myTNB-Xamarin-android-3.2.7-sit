using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class AccountSMRStatus : SMREligibilityModel
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

    public class SMREligibilityModel
    {
        [JsonProperty(PropertyName = "SMREligibility")]
        public string SMREligibility { get; set; }
    }
}
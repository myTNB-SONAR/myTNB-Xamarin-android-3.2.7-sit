using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class GetEligibilityResponse : BaseResponse<GetEligibilityModel>
    {

    }

    public class GetEligibilityModel
    {
        [JsonProperty("eligibileFeatures")]
        public EligibileFeaturesModel EligibileFeatures { set; get; }
        [JsonProperty("dbr")]
        public DBRModel DBR { set; get; }

        [JsonProperty("eb")]
        public EBModel EB { set; get; }
    }

    public class EligibileFeaturesModel
    {
        [JsonProperty("eligibleFeatureDetails")]
        public List<EligibileFeatureDetailsModel> EligibleFeatureDetails { set; get; }
    }

    public class EligibileFeatureDetailsModel
    {
        [JsonProperty("feature")]
        public string Feature { set; get; }
        [JsonProperty("enabled")]
        public bool Enabled { set; get; }
        [JsonProperty("targetGroup")]
        public bool TargetGroup { set; get; }
    }

    public class DBRModel
    {
        [JsonProperty("contractAccounts")]
        public List<ContractAccountsModel> ContractAccounts { set; get; }
    }

    public class EBModel
    {
        [JsonProperty("contractAccounts")]
        public List<ContractAccountsModel> ContractAccounts { set; get; }
    }

    public class ContractAccountsModel
    {
        [JsonProperty("contractAccount")]
        public string ContractAccount { set; get; }
        [JsonProperty("acted")]
        public bool Acted { set; get; }
        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { set; get; }
    }
}
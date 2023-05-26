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
        public BaseCAListModel DBR { set; get; }
        [JsonProperty("eb")]
        public BaseCAListModel EB { set; get; }
        [JsonProperty("br")]
        public BaseCAListModel BR { set; get; }
        [JsonProperty("sd")]
        public BaseCAListModel SD { set; get; }
        [JsonProperty("tng")]
        public BaseCAListModel TNG { set; get; }
        [JsonProperty("myHome")]
        public BaseCAListModel MyHome { set; get; }
        [JsonProperty("ds")]
        public BaseCAListModel DS { set; get; }
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
        [JsonProperty("validateCriteria")]
        public bool ValidateCriteria { set; get; }
    }

    public class BaseCAListModel
    {
        [JsonProperty("contractAccounts")]
        public List<ContractAccountsModel> ContractAccounts { set; get; }
    }

    /*
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
    */

    public class ContractAccountsModel
    {
        [JsonProperty("contractAccount")]
        public string ContractAccount { set; get; }
        [JsonProperty("acted")]
        public bool Acted { set; get; }
        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { set; get; }
        public string BusinessArea { set; get; }
    }
}
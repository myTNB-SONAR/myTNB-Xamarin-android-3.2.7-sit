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
        [JsonProperty("br")]
        public DBRModel BR { set; get; }
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
        [JsonProperty("criteria")]
        public EligibilityCriteriaModel Criteria { set; get; }
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

    public class EligibilityCriteriaModel
    {
        [JsonProperty("ownerType")]
        public List<string> OwnerType { set; get; }
        [JsonProperty("tariffType")]
        public List<string> TariffType { set; get; }
        [JsonProperty("caType")]
        public List<string> CaType { set; get; }

        [JsonIgnore]
        internal bool IsOwner
        {
            get
            {
                return OwnerType.Contains("Owner");
            }
        }
        [JsonIgnore]
        internal bool IsNonOwner
        {
            get
            {
                return OwnerType.Contains("NonOwner");
            }
        }
        [JsonIgnore]
        internal bool IsSmartMeterCA
        {
            get
            {
                return CaType.Contains("SmartMeter");
            }
        }
        [JsonIgnore]
        internal bool IsNormalCA
        {
            get
            {
                return CaType.Contains("Normal");
            }
        }
        [JsonIgnore]
        internal bool IsRenewableEnergyCA
        {
            get
            {
                return CaType.Contains("RenewableEnergy");
            }
        }
        [JsonIgnore]
        internal bool IsSelfMeterReadingCA
        {
            get
            {
                return CaType.Contains("SelfMeterReading");
            }
        }
    }
}
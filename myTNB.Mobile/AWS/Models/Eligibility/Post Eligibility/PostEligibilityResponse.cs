using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models
{
    public class PostEligibilityResponse : BaseResponse<PostEligibilityModel>
    {

    }

    public class PostEligibilityModel
    {
        [JsonProperty("eligibileFeaturesList")]
        public EligibileFeaturesModel EligibileFeaturesList { set; get; }
        [JsonProperty("featureCAList")]
        public List<FeatureCAModel> FeatureCAList { set; get; }
        [JsonProperty("featureBAList")]
        public List<FeatureCAModel> FeatureBAList { set; get; }
    }

    public class FeatureCAModel : ContractAccountsModel
    {
        [JsonProperty("featureName")]
        public string FeatureName { set; get; }
    }
}
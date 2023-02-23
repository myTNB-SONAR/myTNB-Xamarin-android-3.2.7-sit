using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.DBR
{
    public class PostBREligibilityIndicatorsResponse : BaseResponse<List<PostBREligibilityIndicatorsModel>>
    {

    }

    public class PostBREligibilityIndicatorsModel
    {
        [JsonProperty("caNo")]
        public string caNo { get; set; }
        [JsonProperty("isOwnerOverRule")]
        public bool IsOwnerOverRule { get; set; } = false;
        [JsonProperty("isOwnerAlreadyOptIn")]
        public bool IsOwnerAlreadyOptIn { get; set; } = false;
        [JsonProperty("isTenantAlreadyOptIn")]
        public bool IsTenantAlreadyOptIn { get; set; } = false;
    }
}
using System;
using System.Collections.Generic;
using myTNB.Mobile.API.Base;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.MoreIcon
{
    public class MoreIconResponse: BaseResponse<GetMoreIconModel>
    {
    }

    public class GetMoreIconModel
    {
        [JsonProperty("userId")]
        public string userId { set; get; }
        [JsonProperty("email")]
        public string email { set; get; }
        [JsonProperty("modifiedDate")]
        public DateTime? modifiedDate { set; get; }

        public List<FeatureIcon> featureIcon { set; get; }
    }

    public class FeatureIcon
    {
        [JsonProperty("serviceName")]
        public string serviceName { set; get; }
        [JsonProperty("serviceId")]
        public string serviceId { set; get; }
        [JsonProperty("isLocked")]
        public bool isLocked { set; get; }
        [JsonProperty("isAvailable")]
        public bool isAvailable { set; get; }
    }
}


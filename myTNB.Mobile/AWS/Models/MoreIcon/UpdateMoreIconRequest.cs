using System;
using System.Collections.Generic;
using myTNB.Mobile.API.Models.Home.PostServices;
using Newtonsoft.Json;

namespace myTNB.Mobile.AWS.Models.MoreIcon
{
    public class UpdateMoreIconRequest
    {
        public UserInfoExtra usrInf { set; get; }
        public DeviceInfoExtra deviceInf { set; get; }
        public MoreIconModel content { set; get; }
    }

    public class MoreIconModel
    {
        [JsonProperty("userId")]
        public string userId { set; get; }
        [JsonProperty("email")]
        public string email { set; get; }
        public List<Features> featureIcon { set; get; }
    }

    public class Features
    {
        public string serviceName { set; get; }
        [JsonProperty("serviceId")]
        public string serviceId { set; get; }
        [JsonProperty("isLocked")]
        public bool isLocked { set; get; }
        [JsonProperty("isAvailable")]
        public bool isAvailable { set; get; }
    }
}


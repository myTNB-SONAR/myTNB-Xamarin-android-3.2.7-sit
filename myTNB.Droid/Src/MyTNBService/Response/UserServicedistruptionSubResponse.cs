using System;
using myTNB.AndroidApp.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class UserServicedistruptionSubResponse : BaseResponse<UserServicedistruptionSubResponse.UserServicedistruptionSubResponseData>
    {
        public UserServicedistruptionSubResponseData GetData()
        {
            return Response.Data;
        }
        public class UserServicedistruptionSubResponseData
        {
            [JsonProperty("subscriptionStatus")]
            public bool ? subscriptionStatus { get; set; }
        }
    }
}

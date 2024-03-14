using System;
using myTNB.Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB.Android.Src.MyTNBService.Response
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

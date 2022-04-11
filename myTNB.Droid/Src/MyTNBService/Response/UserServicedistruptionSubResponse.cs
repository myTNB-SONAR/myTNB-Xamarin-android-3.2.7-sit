using System;
using myTNB_Android.Src.Base.Response;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
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

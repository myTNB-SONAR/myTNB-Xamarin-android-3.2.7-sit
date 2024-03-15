using System;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class PhoneVerifyStatusResponse : BaseResponse<PhoneVerifyStatusResponse.ResponseData>
    {
        public ResponseData GetData()
		{
            return Response.Data;
		}

        public class ResponseData
		{
            [JsonProperty(PropertyName = "PhoneNumber")]
            public string PhoneNumber { get; set; }

            [JsonProperty(PropertyName = "IsPhoneVerified")]
            public bool IsPhoneVerified { get; set; }
        }
    }
}

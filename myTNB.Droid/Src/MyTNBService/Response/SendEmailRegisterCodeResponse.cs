using System;
using Newtonsoft.Json;
using Refit;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class SendEmailRegisterCodeResponse : BaseResponse<SendEmailRegisterCodeResponse.ResponseData>
    {
        public ResponseData GetDataAll()
        {
            return Response.Data;
        }

        public class ResponseData
        {
            [JsonProperty(PropertyName = "registeredDate")]
            [AliasAs("registeredDate")]
            public string ? registeredDate { get; set; }

            [JsonProperty(PropertyName = "isActive")]
            [AliasAs("isActive")]
            public bool isActive { get; set; }

            [JsonProperty(PropertyName = "IsVerified")]
            [AliasAs("IsVerified")]
            public bool IsVerified { get; set; }
        }
    }
}

using System;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class AccountToCustomerResponse : BaseResponse<AccountToCustomerResponse.ResponseData>
    {
        public class ResponseData
        {
            [JsonProperty(PropertyName = "data")]
            public string Data { get; set; }
        }
    }
}

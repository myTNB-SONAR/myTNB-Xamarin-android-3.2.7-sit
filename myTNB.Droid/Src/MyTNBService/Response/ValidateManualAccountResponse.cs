using System;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class ValidateManualAccountResponse : BaseResponse<ValidateManualAccountResponse.ResponseData>
    {
        public ResponseData GetData()
        {
            return Response.Data;
        }

        public class ResponseData
        {
            [JsonProperty(PropertyName = "accNum")]
            public string accNum { get; set; }

            [JsonProperty(PropertyName = "accountTypeId")]
            public string accountTypeId { get; set; }

            [JsonProperty(PropertyName = "accountStAddress")]
            public string accountStAddress { get; set; }

            [JsonProperty(PropertyName = "icNum")]
            public string icNum { get; set; }

            [JsonProperty(PropertyName = "isOwned")]
            public string isOwned { get; set; }

            [JsonProperty(PropertyName = "accountNickName")]
            public string accountNickName { get; set; }

            [JsonProperty(PropertyName = "accountCategoryId")]
            public string accountCategoryId { get; set; }

            [JsonProperty(PropertyName = "accountOwnerName")]
            public string accountOwnerName { get; set; }

            [JsonProperty(PropertyName = "smartMeterCode")]
            public string smartMeterCode { get; set; }

            [JsonProperty(PropertyName = "isTaggedSMR")]
            public string isTaggedSMR { get; set; }

            [JsonProperty(PropertyName = "isEmail")]
            public string emailOwner { get; set; }

            [JsonProperty(PropertyName = "isMobileNum")]
            public string mobileNoOwner { get; set; }
        }
    }
}

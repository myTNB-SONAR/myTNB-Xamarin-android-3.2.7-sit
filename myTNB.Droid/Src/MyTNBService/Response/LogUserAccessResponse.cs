using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class LogUserAccessResponse : BaseResponse<List<LogUserAccessResponse.LogUserAccessResponseData>>
    {
        public List<LogUserAccessResponseData> GetData()
        {
            return Response.Data;
        }

        public class LogUserAccessResponseData
        {
            [JsonProperty("AccountNo")]
            public string AccountNo { get; set; }

            [JsonProperty("Action")]
            public string Action { get; set; }

            [JsonProperty("ActivityLogID")]
            public string ActivityLogID { get; set; }

            [JsonProperty("CreateBy")]
            public string CreateBy { get; set; }

            [JsonProperty("CreatedDate")]
            public DateTime CreatedDate { get; set; }

            [JsonProperty("IsApplyEBilling")]
            public bool IsApplyEBilling { get; set; }

            [JsonProperty("IsHaveAccess")]
            public bool IsHaveAccess { get; set; }

            [JsonProperty("UserID")]
            public string UserID { get; set; }

            [JsonProperty("UserName")]
            public string UserName { get; set; }

            [JsonProperty("IsPreRegister")]
            public bool IsPreRegister { get; set; }

        }
    }
}

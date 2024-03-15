using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.MyTNBService.Response
{
    public class ManageAccessAccountListResponse : BaseResponse<List<ManageAccessAccountListResponse.CustomerAccountData>>
    {
        public List<CustomerAccountData> GetData()
        {
            return Response.Data;
        }

        public class CustomerAccountData
        {
            [JsonProperty(PropertyName = "accountDescription")]
            public string AccountDescription { get; set; }

            [JsonProperty(PropertyName = "accountNumber")]
            public string AccountNumber { get; set; }

            [JsonProperty(PropertyName = "accountId")]
            public string AccountId { get; set; }

            [JsonProperty(PropertyName = "IsApplyEBilling")]
            public bool IsApplyEBilling { get; set; }

            [JsonProperty(PropertyName = "IsHaveAccess")]
            public bool IsHaveAccess { get; set; }

            [JsonProperty(PropertyName = "IsOwnedAccount")]
            public bool IsOwnedAccount { get; set; }

            [JsonProperty(PropertyName = "IsPreRegister")]
            public bool IsPreRegister { get; set; }

            [JsonProperty(PropertyName = "email")]
            public string Email { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "userId")]
            public string UserId { get; set; }

        }
    }
}

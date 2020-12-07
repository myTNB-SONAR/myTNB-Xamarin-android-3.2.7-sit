using System.Collections.Generic;
using Newtonsoft.Json;

namespace myTNB_Android.Src.MyTNBService.Response
{
    public class ManageAccessAccountListResponse : BaseResponse<List<ManageAccessAccountListResponse.CustomerAccountData>>
    {
        public List<CustomerAccountData> GetData()
        {
            return Response.Data;
        }

        public class CustomerAccountData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "accountDescription")]
            public string AccountDescription { get; set; }

            [JsonProperty(PropertyName = "accountNumber")]
            public string AccountNumber { get; set; }

            [JsonProperty(PropertyName = "accountId")]
            public string AccountId { get; set; }

            [JsonProperty(PropertyName = "IdentificationNo")]
            public string IdentificationNo { get; set; }

            [JsonProperty(PropertyName = "IsApplyEBilling")]
            public bool IsApplyEBilling { get; set; }

            [JsonProperty(PropertyName = "IsHaveAccess")]
            public bool IsHaveAccess { get; set; }

            [JsonProperty(PropertyName = "IsHaveAccessSpecified")]
            public bool IsHaveAccessSpecified { get; set; }

            [JsonProperty(PropertyName = "MobileNo")]
            public string MobileNo { get; set; }

            [JsonProperty(PropertyName = "PremiseAddress")]
            public string PremiseAddress { get; set; }

            [JsonProperty(PropertyName = "IsOwnedAccount")]
            public bool IsOwnedAccount { get; set; }

            [JsonProperty(PropertyName = "Postcode")]
            public string Postcode { get; set; }

            [JsonProperty(PropertyName = "State")]
            public string State { get; set; }

            [JsonProperty(PropertyName = "isOwnerAccountSpecified")]
            public string isOwnerAccountSpecified { get; set; }

            [JsonProperty(PropertyName = "OwnerName")]
            public string OwnerName { get; set; }

            [JsonProperty(PropertyName = "email")]
            public string email { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string name { get; set; }

            [JsonProperty(PropertyName = "userId")]
            public string userId { get; set; }

            [JsonProperty(PropertyName = "activitylogId")]
            public string activitylogId { get; set; }
        }
    }
}

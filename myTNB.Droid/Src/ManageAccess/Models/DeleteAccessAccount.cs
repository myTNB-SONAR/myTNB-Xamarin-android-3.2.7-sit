using System.Collections.Generic;
using myTNB.Android.Src.Database;
using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.ManageAccess.Models
{
    public class DeleteAccessAccount
    {

        [JsonProperty("accountId")]
        public string UserAccountId { get; set; }

        [JsonProperty("IsApplyEBilling")]
        public bool IsApplyEBilling { get; set; }

        [JsonProperty("IsHaveAccess")]
        public bool IsHaveAccess { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }

        [JsonProperty("IsPreRegister")]
        public bool IsPreRegister { get; set; }

        [JsonProperty("tenantNickname")]
        public string tenantNickname { get; set; }
    }
}
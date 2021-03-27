using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.Login.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "__type")]
        [AliasAs("__type")]
        public string __type { get; set; }

        [JsonProperty(PropertyName = "userID")]
        [AliasAs("userID")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        [AliasAs("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "userName")]
        [AliasAs("userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        [AliasAs("email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "identificationNo")]
        [AliasAs("identificationNo")]
        public string IdentificationNo { get; set; }

        [JsonProperty(PropertyName = "mobileNo")]
        [AliasAs("mobileNo")]
        public string MobileNo { get; set; }


        [JsonProperty(PropertyName = "dateCreated")]
        [AliasAs("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastLoginDate")]
        [AliasAs("lastLoginDate")]
        public string LastLoginDate { get; set; }

        [JsonProperty(PropertyName = "isPhoneVerified")]
        [AliasAs("isPhoneVerified")]
        public bool isPhoneVerified { get; set; }

        [JsonProperty(PropertyName = "isActivated")]
        [AliasAs("isActivated")]
        public bool IsActivated { get; set; }


        public User(string __type, string userId, string displayName, string userName, string email, string dateCreated, string lastLoginDate)
        {
            this.__type = __type ?? "";
            UserId = userId ?? "";
            DisplayName = displayName ?? "";
            UserName = userName ?? "";
            Email = email ?? "";
            DateCreated = dateCreated ?? "";
            LastLoginDate = lastLoginDate ?? "";

        }

        public override string ToString()
        {
            return __type + " " + UserId + " " + DisplayName + " " + UserName + " " + Email + " " + DateCreated + " " + LastLoginDate;
        }
    }
}
using Newtonsoft.Json;
using Refit;

namespace myTNB.Android.Src.XEmailRegistrationForm.Models
{
    public class UserAll
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

        [JsonProperty(PropertyName = "isRegistered")]
        [AliasAs("isRegistered")]
        public bool IsRegistered { get; set; }

        [JsonProperty(PropertyName = "statusCode")]
        [AliasAs("statusCode")]
        public string StatusCode { get; set; }

        [JsonProperty(PropertyName = "IsSuccess")]
        [AliasAs("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty(PropertyName = "UpdatedDate")]
        [AliasAs("updatedDate")]
        public string UpdatedDate { get; set; }


        public UserAll(string __type, string userId, string displayName, string userName, string email, string dateCreated, string lastLoginDate, string statusCode, bool isRegistered, bool isSuccess, string updatedDate)
        {
            this.__type = __type ?? "";
            UserId = userId ?? "";
            DisplayName = displayName ?? "";
            UserName = userName ?? "";
            Email = email ?? "";
            DateCreated = dateCreated ?? "";
            LastLoginDate = lastLoginDate ?? "";
            StatusCode = statusCode ?? "";
            IsRegistered = isRegistered;
            IsSuccess = isSuccess;
            UpdatedDate = updatedDate ?? "";

        }

        public override string ToString()
        {
            return __type + " " + UserId + " " + DisplayName + " " + UserName + " " + Email + " " + DateCreated + " " + LastLoginDate + " " + StatusCode + " " + IsSuccess + " " + UpdatedDate;
        }
    }
}
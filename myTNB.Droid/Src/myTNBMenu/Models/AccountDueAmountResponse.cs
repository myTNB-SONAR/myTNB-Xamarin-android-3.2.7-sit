using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.Models
{
    public class AccountDueAmountResponse
    {

        [JsonProperty("d")]
        public AccountDueAmountData Data { get; set; }

        public class AccountDueAmountData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "data")]
            public AccountDueAmount Data { get; set; }
        }
    }
}
﻿using Newtonsoft.Json;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class AccountDetailsResponse
    {
        [JsonProperty(PropertyName = "d")]
        public AccountDetailsData Data { get; set; }

        public class AccountDetailsData
        {
            [JsonProperty(PropertyName = "__type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "data")]
            public AccountDetails AccountData { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "isError")]
            public bool IsError { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }
        }
    }
}
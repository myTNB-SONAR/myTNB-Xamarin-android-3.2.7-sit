using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace myTNB_Android.Src.AddAccount.Models
{
    public class ValidateManualAccountResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ValidationResponse validation { get; set; }

        public class ValidationResponse
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
            public ValidatedAccountData Data { get; set; }
        }
    }

    public class ValidatedAccountData
    {
        [JsonProperty(PropertyName = "accountStAddress")]
        public string accountStAddress { get; set; }

        [JsonProperty(PropertyName = "accountCategoryId")]
        public string accountCategoryId { get; set; }
    }
}
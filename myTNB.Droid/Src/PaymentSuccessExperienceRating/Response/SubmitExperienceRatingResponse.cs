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

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.Response
{
    public class SubmitExperienceRatingResponse
    {
        [JsonProperty(PropertyName = "d")]
        public RatingResponse Data { get; set; }

        public class RatingResponse
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
            public RatingData RatingData { get; set; }
        }

        public class RatingData
        {
            [JsonProperty(PropertyName = "Id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "Email")]
            public string Email { get; set; }

            [JsonProperty(PropertyName = "Rate")]
            public string Rate { get; set; }

            [JsonProperty(PropertyName = "Message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "RatingFor")]
            public string RatingFor { get; set; }

            [JsonProperty(PropertyName = "DateCreated")]
            public string DateCreated { get; set; }

            [JsonProperty(PropertyName = "IsRead")]
            public bool IsRead { get; set; }

            [JsonProperty(PropertyName = "IsDeleted")]
            public bool IsDeleted { get; set; }
        }
    }
}
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

namespace myTNB_Android.Src.PaymentSuccessExperienceRating.Request
{
    public class SubmitExperienceRatingRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("rating")]
        public string rating { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("ratingFor")]
        public string ratingFor { get; set; }
    }
}
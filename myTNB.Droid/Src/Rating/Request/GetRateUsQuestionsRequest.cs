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
using Refit;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Rating.Request
{
    public class GetRateUsQuestionsRequest
    {
        [AliasAs("ApiKeyID")]
        [JsonProperty("ApiKeyID")]
        public string ApiKeyID { get; set; }

        [AliasAs("QuestionCategoryId")]
        [JsonProperty("QuestionCategoryId")]
        public string QuestionCategoryId { get; set; }
    }
}
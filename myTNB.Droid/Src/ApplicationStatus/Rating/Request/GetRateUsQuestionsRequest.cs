using Newtonsoft.Json;
using Refit;

namespace myTNB_Android.Src.ApplicationStatusRating.Request
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
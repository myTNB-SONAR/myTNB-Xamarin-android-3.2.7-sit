using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class FeedbackCategory
    {
        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [JsonProperty("FeedbackCategoryDesc")]
        public string FeedbackCategoryDesc { get; set; }
    }
}
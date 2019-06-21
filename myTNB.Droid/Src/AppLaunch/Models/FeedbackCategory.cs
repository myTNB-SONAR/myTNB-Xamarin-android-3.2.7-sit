using Newtonsoft.Json;

namespace myTNB_Android.Src.AppLaunch.Models
{
    public class FeedbackCategory
    {
        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }
    }
}
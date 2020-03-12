using myTNB_Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Base.Models
{
    public class SubmittedFeedback
    {
        [JsonProperty("ServiceReqNo")]
        public string FeedbackId { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("FeedbackMessage")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("FeedbackCategoryName")]
        public string FeedbackCategoryName { get; set; }

        [JsonProperty("FeedbackCategoryId")]
        public string FeedbackCategoryId { get; set; }

        [JsonProperty("FeedbackNameInListView")]
        public string FeedbackNameInListView { get; set; }

        public static SubmittedFeedback Copy(SubmittedFeedbackEntity entity)
        {
            return new SubmittedFeedback()
            {
                FeedbackId = entity.Id,
                DateCreated = entity.DateCreated,
                FeedbackMessage = entity.FeedbackMessage,
                FeedbackCategoryId = entity.FeedbackCategoryId,
                FeedbackCategoryName = entity.FeedbackCategoryName,
                FeedbackNameInListView = entity.FeedbackNameInListView
            };
        }
    }
}
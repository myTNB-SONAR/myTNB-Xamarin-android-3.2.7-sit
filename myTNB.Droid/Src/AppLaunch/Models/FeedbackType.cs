using myTNB.AndroidApp.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.AppLaunch.Models
{
    public class FeedbackType
    {
        [JsonProperty("FeedbackTypeId")]
        public string FeedbackTypeId { get; set; }

        [JsonProperty("FeedbackTypeName")]
        public string FeedbackTypeName { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        internal static FeedbackType Copy(FeedbackTypeEntity entity)
        {
            return new FeedbackType()
            {
                FeedbackTypeId = entity.Id,
                FeedbackTypeName = entity.Name,
                IsSelected = entity.IsSelected
            };
        }
    }
}
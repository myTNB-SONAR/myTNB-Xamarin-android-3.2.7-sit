using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class FeedbackState
    {
        [JsonProperty("StateId")]
        public string StateId { get; set; }

        [JsonProperty("StateName")]
        public string StateName { get; set; }

        [JsonProperty("IsSelected", Required = Newtonsoft.Json.Required.Default)]
        public bool IsSelected { get; set; }

        internal static FeedbackState Copy(FeedbackStateEntity entity)
        {
            return new FeedbackState()
            {
                StateId = entity.Id,
                StateName = entity.Name,
                IsSelected = entity.IsSelected
            };
        }
    }
}
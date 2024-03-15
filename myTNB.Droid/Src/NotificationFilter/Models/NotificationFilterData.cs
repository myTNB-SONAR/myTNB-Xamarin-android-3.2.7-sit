using myTNB.AndroidApp.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.NotificationFilter.Models
{
    public class NotificationFilterData
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("IsSelected")]
        public bool IsSelected { get; set; }

        public static NotificationFilterData Get(NotificationFilterEntity entity)
        {
            return new NotificationFilterData()
            {
                Id = entity.Id,
                Title = entity.Title,
                IsSelected = entity.IsSelected
            };
        }
    }
}
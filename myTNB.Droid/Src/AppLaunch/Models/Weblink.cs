using myTNB.Android.Src.Database.Model;
using Newtonsoft.Json;

namespace myTNB.Android.Src.AppLaunch.Models
{
    public class Weblink
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("OpenWith")]
        public string OpenWith { get; set; }

        public static Weblink Copy(WeblinkEntity weblinkEntity)
        {
            return new Weblink()
            {
                Id = weblinkEntity.Id,
                Code = weblinkEntity.Code,
                Title = weblinkEntity.Title,
                Url = weblinkEntity.Url,
                IsActive = weblinkEntity.IsActive,
                DateCreated = weblinkEntity.DateCreated,
                OpenWith = weblinkEntity.OpenWith
            };
        }
    }
}
using Newtonsoft.Json;

namespace myTNB_Android.Src.Base.Models
{
    public class AttachedImage
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ViewType")]
        public int ViewType { get; set; }

        [JsonProperty("IsLoading")]
        public bool IsLoading { get; set; }

        [JsonProperty("FileName")]
        public string FileName { get; set; }
    }
}
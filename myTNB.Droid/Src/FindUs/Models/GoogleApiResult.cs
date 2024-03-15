using Newtonsoft.Json;

namespace myTNB.AndroidApp.Src.FindUs.Models
{
    public class GoogleApiResult
    {

        [JsonProperty(PropertyName = "geometry")]
        public Geometry geometry { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string icon { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "place_id")]
        public string place_id { get; set; }

        [JsonProperty(PropertyName = "vicinity")]
        public string vicinity { get; set; }

        [JsonProperty(PropertyName = "opening_hours")]
        public OpenningHours openingHours { get; set; }

        public class Geometry
        {

            [JsonProperty(PropertyName = "location")]
            public Location location { get; set; }

            [JsonProperty(PropertyName = "viewport")]
            public ViewPort viewPort { get; set; }

        }

        public class Location
        {
            [JsonProperty(PropertyName = "lat")]
            public double lat { get; set; }

            [JsonProperty(PropertyName = "lng")]
            public double lng { get; set; }
        }

        public class OpenningHours
        {
            [JsonProperty(PropertyName = "open_now")]
            public bool openNow { get; set; }
        }

        public class ViewPort
        {
            [JsonProperty(PropertyName = "lat")]
            public Northeast northeast { get; set; }

            [JsonProperty(PropertyName = "lng")]
            public Southwest southwest { get; set; }
        }

        public class Northeast
        {
            [JsonProperty(PropertyName = "lat")]
            public double lat { get; set; }

            [JsonProperty(PropertyName = "lng")]
            public double lng { get; set; }
        }

        public class Southwest
        {
            [JsonProperty(PropertyName = "lat")]
            public double lat { get; set; }

            [JsonProperty(PropertyName = "lng")]
            public double lng { get; set; }
        }
    }
}
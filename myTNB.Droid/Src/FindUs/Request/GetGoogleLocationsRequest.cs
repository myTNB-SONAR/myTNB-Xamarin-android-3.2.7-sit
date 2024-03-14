using Refit;

namespace myTNB.Android.Src.FindUs.Request
{
    public class GetGoogleLocationsRequest
    {
        [AliasAs("key")]
        public string key { get; set; }

        [AliasAs("location")]
        public string location { get; set; }

        [AliasAs("radius")]
        public string radius { get; set; }

        [AliasAs("keyword")]
        public string keyword { get; set; }

        [AliasAs("type")]
        public string type { get; set; }

        public GetGoogleLocationsRequest(string key, string location, string radius, string keyword, string type)
        {
            this.key = key;
            this.location = location;
            this.radius = radius;
            this.keyword = keyword;
            this.type = type;

        }
    }


}
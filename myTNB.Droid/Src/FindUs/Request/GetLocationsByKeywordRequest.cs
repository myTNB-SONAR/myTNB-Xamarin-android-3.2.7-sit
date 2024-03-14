using Refit;

namespace myTNB.Android.Src.FindUs.Request
{
    public class GetLocationsByKeywordRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("latitude")]
        public string latitude { get; set; }

        [AliasAs("longitude")]
        public string longitude { get; set; }

        [AliasAs("locationType")]
        public string locationType { get; set; }

        [AliasAs("keyword")]
        public string keyword { get; set; }

        public GetLocationsByKeywordRequest(string apiKeyID, string latitude, string longitude, string locationType, string keyword)
        {
            this.apiKeyID = apiKeyID;
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;
            this.keyword = keyword;

        }
    }
}
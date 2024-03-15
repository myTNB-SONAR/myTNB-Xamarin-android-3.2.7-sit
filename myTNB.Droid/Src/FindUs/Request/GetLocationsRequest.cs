using Refit;

namespace myTNB.AndroidApp.Src.FindUs.Request
{
    public class GetLocationsRequest
    {
        [AliasAs("apiKeyID")]
        public string apiKeyID { get; set; }

        [AliasAs("latitude")]
        public string latitude { get; set; }

        [AliasAs("longitude")]
        public string longitude { get; set; }

        [AliasAs("locationType")]
        public string locationType { get; set; }

        public GetLocationsRequest(string apiKeyID, string latitude, string longitude, string locationType)
        {
            this.apiKeyID = apiKeyID;
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;

        }

    }
}
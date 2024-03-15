using System;
namespace myTNB.AndroidApp.Src.MyTNBService.Request
{
    public class GetLocationListRequest : BaseRequest
    {
        public string latitude, longitude, locationType;

        public GetLocationListRequest(string latitude, string longitude, string locationType)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;
        }
    }
}

using System;
namespace myTNB.Android.Src.MyTNBService.Request
{
    public class GetLocationListByKeywordRequest : BaseRequest
    {
        public string latitude, longitude, locationType, keyword;

        public GetLocationListByKeywordRequest(string latitude, string longitude, string locationType, string keyword)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;
            this.keyword = keyword;
        }
    }
}

using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class CountryTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<CountryTimeStamp> Data { set; get; }
    }

    public class CountryTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }

    public class CountryResponseModel
    {
        public string Status { set; get; }
        public List<CountryDataModel> Data { set; get; }
    }

    public class CountryDataModel
    {
        public string CountryFile { set; get; }
        public string ID { set; get; }
    }
}
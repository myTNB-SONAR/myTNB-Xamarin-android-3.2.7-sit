using System.Collections.Generic;

namespace myTNB
{
    public class Countries
    {
        public List<CountryModel> CountryList { set; get; } = new List<CountryModel>();
    }

    public class CountryModel
    {
        public string CountryCode { set; get; } = string.Empty;
        public string CountryName { set; get; } = string.Empty;
        public string CountryISDCode { set; get; } = string.Empty;
    }

}
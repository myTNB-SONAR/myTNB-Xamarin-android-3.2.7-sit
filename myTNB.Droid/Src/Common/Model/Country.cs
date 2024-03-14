using System;
namespace myTNB.Android.Src.Common.Model
{
    public class Country
    {
        public string code, name, isd;

        public Country(string code, string name, string isd)
        {
            this.code = code;
            this.name = name;
            this.isd = isd;
        }
    }
}

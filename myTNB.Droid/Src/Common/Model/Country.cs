using System;
namespace myTNB_Android.Src.Common.Model
{
    public class Country
    {
        private string code, name, isd;

        public Country(string code, string name, string isd)
        {
            this.code = code;
            this.name = name;
            this.isd = isd;
        }
    }
}

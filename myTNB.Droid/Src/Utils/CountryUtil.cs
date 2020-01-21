using System;
namespace myTNB_Android.Src.Utils
{
    public class CountryUtil
    {
        private static readonly Lazy<CountryUtil>
            lazy = new Lazy<CountryUtil>(() => new CountryUtil());
        public static CountryUtil Instance { get { return lazy.Value; } }
        private CountryUtil()
        {
        }


    }
}

using Foundation;

namespace myTNB
{
    public static class CountryUtility
    {
        private static readonly string CountryContentKey = "CountryContent";

        public static string CountryContent
        {
            set
            {
                if (value.IsValid())
                {
                    NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                    sharedPreference.SetString(value, CountryContentKey);
                    sharedPreference.Synchronize();
                }
            }
            get
            {
                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                string content = sharedPreference.StringForKey(CountryContentKey);
                return content ?? string.Empty;
            }
        }
    }
}
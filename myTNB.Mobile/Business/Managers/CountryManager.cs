using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace myTNB
{
    public sealed class CountryManager
    {
        private static readonly Lazy<CountryManager> lazy = new Lazy<CountryManager>(() => new CountryManager());

        public static CountryManager Instance { get { return lazy.Value; } }

        private CountryManager() { }

        private string CountryJSON = string.Empty;
        private const string COUNTRY_RESOURCE_PATH = "myTNB.Mobile.Resources.Country.CountryList.json";

        public void SetCountries()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(COUNTRY_RESOURCE_PATH);
                using (StreamReader reader = new StreamReader(stream))
                {
                    CountryJSON = reader.ReadToEnd();
                    Debug.WriteLine("DEBUG >> CountryJSON: " + CountryJSON);
                }
                Debug.WriteLine("DEBUG >> SUCCESS");
            }
            catch (Exception e)
            {
                Debug.WriteLine("DEBUG >> SetLanguageData: " + e.Message);
            }
        }

        public void SetCountries(string content)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
            {
                CountryJSON = content;
            }
        }

        public List<CountryModel> GetCountryList()
        {
            Countries Countries = (string.IsNullOrEmpty(CountryJSON) || string.IsNullOrWhiteSpace(CountryJSON)) ? new Countries()
                : JsonConvert.DeserializeObject<Countries>(CountryJSON);
            if (Countries != null && Countries.CountryList != null)
            {
                return Countries.CountryList;
            }

            return new List<CountryModel>();
        }

        public CountryModel GetCountryInfo(string countryCode)
        {
            List<CountryModel> countryList = GetCountryList();
            if (countryList != null && countryList.Count > 0)
            {
                int index = countryList.FindIndex(x => x.CountryCode.ToUpper() == countryCode.ToUpper());
                if (index > -1)
                {
                    return countryList[index];
                }
            }
            return new CountryModel();
        }
    }
}
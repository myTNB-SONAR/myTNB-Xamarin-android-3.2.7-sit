using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public string CountryString
        {
            get
            {
                return CountryJSON;
            }
        }

        public List<CountryModel> GetCountryList()
        {
            if (string.IsNullOrEmpty(CountryJSON) || string.IsNullOrWhiteSpace(CountryJSON))
            {
                SetCountries();
            }
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

        public Dictionary<string, List<CountryModel>> GetCountryDictionary()
        {
            Dictionary<string, List<CountryModel>> countryDictionary = new Dictionary<string, List<CountryModel>>(); ;
            List<CountryModel> countryList = GetCountryList();
            if (countryList != null && countryList.Count > 0)
            {
                for (int i = 0; i < countryList.Count; i++)
                {
                    CountryModel countryInfo = countryList[i];
                    string countryName = countryInfo.CountryName;
                    string firstChar = countryName[0].ToString().ToUpper();
                    if (countryDictionary.ContainsKey(firstChar))
                    {
                        List<CountryModel> list = countryDictionary[firstChar];
                        list.Add(countryInfo);
                    }
                    else
                    {
                        countryDictionary.Add(firstChar, new List<CountryModel> { countryInfo });
                    }
                }
            }

            countryDictionary = OrderDictionary(countryDictionary);
            return countryDictionary;
        }

        private Dictionary<string, List<CountryModel>> OrderDictionary(Dictionary<string, List<CountryModel>> dictionary)
        {
            Dictionary<string, List<CountryModel>> newdictionary = dictionary.OrderBy(d => d.Key)
                .ToDictionary(d => d.Key, d => (d.Value.OrderBy(c => c.CountryName)).ToList());
            return newdictionary;
        }

        public CountryModel GetCountryISDCode(string countryISDCode)
        {
            List<CountryModel> countryList = GetCountryList();

            var obj = countryList.Where(x => x.CountryISDCode.Equals(countryISDCode.Trim().Substring(0, 3)));
            if (obj.Count() == 0)
            {
                obj = countryList.Where(x => x.CountryISDCode.Equals(countryISDCode.Trim().Substring(0, 4)));
                if (obj.Count() == 0)
                {
                    obj = countryList.Where(x => x.CountryISDCode.Equals(countryISDCode.Trim().Substring(0, 2)));
                }
            }
            return obj.FirstOrDefault();
        }
    }
}
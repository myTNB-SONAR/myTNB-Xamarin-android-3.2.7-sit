using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.AndroidApp.Src.Common.Model;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;

namespace myTNB.AndroidApp.Src.Utils
{
    public class CountryUtil
    {
        private static readonly Lazy<CountryUtil>
            lazy = new Lazy<CountryUtil>(() => new CountryUtil());
        public static CountryUtil Instance { get { return lazy.Value; } }
        private Country defaultCountry;
        private CountryUtil()
        {
        }

        /// <summary>
        /// Sets the country list and default country
        /// </summary>
        public void SetCountryList()
        {
            string density = DPUtils.GetDeviceDensity(Application.Context);
            GetItemsService getItemsService = new GetItemsService(SiteCoreConfig.OS, density, SiteCoreConfig.SITECORE_URL, Constants.DEFAULT_LANG);

            string currentTimestamp = SitecoreCmsEntity.GetItemTimestampById(SitecoreCmsEntity.SITE_CORE_ID.COUNTRY);
            string siteCoreTimestamp = GetTimestampFromSitecore(getItemsService);

            if (string.IsNullOrEmpty(currentTimestamp))
            {
                if (!string.IsNullOrEmpty(siteCoreTimestamp))
                {
                    SetCountryListFromSitecore(getItemsService, siteCoreTimestamp);
                }
                else
                {
                    SetCountryListFromCache();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(siteCoreTimestamp) && (currentTimestamp != siteCoreTimestamp))
                {
                    SetCountryListFromSitecore(getItemsService, siteCoreTimestamp);
                }
                else
                {
                    SetCountryListFromCache();
                }
            }

            if (defaultCountry == null)
            {
                myTNB.CountryModel countryModel = CountryManager.Instance.GetCountryList().Find(model =>
                {
                    return model.CountryCode.ToLower() == Constants.DEFAULT_COUNTRY_CODE.ToLower();
                });
                if (countryModel != null)
                {
                    defaultCountry = new Country(countryModel.CountryCode, countryModel.CountryName, countryModel.CountryISDCode);
                }
                else
                {
                    defaultCountry = new Country("MY","Malaysia","+60");
                }
                
            }
        }

        public List<Country> GetCountryList()
        {
            List<myTNB.CountryModel> countryModeList = CountryManager.Instance.GetCountryList();
            List<Country> countryList = new List<Country>();
            Country country;

            countryModeList.ForEach(model =>
            {
                country = new Country(model.CountryCode,model.CountryName,model.CountryISDCode.Replace(" ", string.Empty));
                countryList.Add(country);
            });

            //Sort Alphabetical by Country Name
            countryList = countryList.OrderBy(countryItem => countryItem.name).ToList();

            return countryList;
        }


        public  Country GetCountryFromPhoneNumber(string PhoneNum)
        {

            var listofCountry = GetCountryList();


            var obj = listofCountry.Where(x => x.isd.Equals(PhoneNum.Trim().Substring(0, 3)));
            if (obj.Count() == 0)
            {
                obj = listofCountry.Where(x => x.isd.Equals(PhoneNum.Trim().Substring(0, 4)));
                if (obj.Count() == 0)
                {
                    obj = listofCountry.Where(x => x.isd.Equals(PhoneNum.Trim().Substring(0, 2)));
                }
            }
            return obj.FirstOrDefault();
               
        }


        public Country GetDefaultCountry()
        {
            return new Country("MY", "Malaysia", "+60");
        }

        public int GetFlagImageResource(Context context, string countryCode)
        {
            string resourceName;
            //handling for reserve word 'do'
            if (countryCode.ToLower() == "do")
            {
                resourceName = countryCode.ToLower() + "flag";
            }
            else
            {
                resourceName = countryCode.ToLower();
            }

            int flagResource = context.Resources.GetIdentifier(resourceName, "drawable", context.PackageName);

            if (flagResource == 0)
            {
                flagResource = context.Resources.GetIdentifier("noflag", "drawable", context.PackageName);
            }

            return flagResource;
        }

        private void SetCountryListFromSitecore(GetItemsService getItemsService, string timeStamp)
        {
            string countryListJson = GetCountryListFromSitecore(getItemsService);
            if (!string.IsNullOrEmpty(countryListJson))
            {
                CountryManager.Instance.SetCountries(countryListJson);
                SitecoreCmsEntity.InsertSiteCoreItem(SitecoreCmsEntity.SITE_CORE_ID.COUNTRY, countryListJson, timeStamp);
            }
            else
            {
                SetCountryListFromCache();
            }
        }

        /// <summary>
        /// Set Country List from Cache
        /// If no items from SitecoreCmsEntity, use the local json file.
        /// </summary>
        private void SetCountryListFromCache()
        {
            string countryListJsonCache = SitecoreCmsEntity.GetItemById(SitecoreCmsEntity.SITE_CORE_ID.COUNTRY);
            if (!string.IsNullOrEmpty(countryListJsonCache))
            {
                CountryManager.Instance.SetCountries(countryListJsonCache);
            }
            else
            {
                CountryManager.Instance.SetCountries();
            }
        }


        private string GetTimestampFromSitecore(GetItemsService getItemsService)
        {
            string countryTimestamp = string.Empty;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    CountryTimeStampResponseModel responseModel = getItemsService.GetCountryTimestampItem();
                    countryTimestamp = responseModel.Data[0].Timestamp;
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).Wait();
            return countryTimestamp;
        }

        private string GetCountryListFromSitecore(GetItemsService getItemsService)
        {
            string countryItem = string.Empty;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    CountryResponseModel responseModel = getItemsService.GetCountryItems();
                    WebRequest webRequest = WebRequest.Create(responseModel.Data[0].CountryFile);
                    using (WebResponse response = webRequest.GetResponse())
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        countryItem = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }).Wait();
            return countryItem;
        }
    }
}

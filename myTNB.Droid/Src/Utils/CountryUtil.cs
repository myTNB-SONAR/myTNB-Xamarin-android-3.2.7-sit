using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using myTNB;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SiteCore;
using myTNB_Android.Src.SitecoreCMS.Model;

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

        /// <summary>
        /// Sets the country list
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
        }

        public List<Country> GetCountryList()
        {
            List<myTNB.CountryModel> countryModeList = CountryManager.Instance.GetCountryList();
            List<Country> countryList = new List<Country>();
            Country country;

            countryModeList.ForEach(model =>
            {
                country = new Country(model.CountryCode,model.CountryName,model.CountryISDCode);
                countryList.Add(country);
            });

            return countryList;
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

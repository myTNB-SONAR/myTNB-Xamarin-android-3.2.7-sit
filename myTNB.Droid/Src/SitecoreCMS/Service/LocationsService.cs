using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    internal class LocationsService
    {
        internal List<LocationsModel> GetLocations(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemById(Constants.Sitecore.ItemID.Locations, PayloadType.Full, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateGetLocationsChildren(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<LocationsModel>> GenerateGetLocationsChildren(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
        {
            List<LocationsModel> list = new List<LocationsModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                LocationsModel listlItem = new LocationsModel
                {
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Title),
                    GeneralText = item.GetValueFromField(Constants.Sitecore.Fields.Shared.GeneralText),
                    PaymentTypesTitle = item.GetFieldValueFromMultilist(Constants.Sitecore.Fields.Locations.PaymentTypes, Constants.Sitecore.Fields.Shared.Title, websiteUrl, language),
                    Latitude = item.GetValueFromField(Constants.Sitecore.Fields.Locations.Latitude),
                    Longitude = item.GetValueFromField(Constants.Sitecore.Fields.Locations.Longitude),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
    }
}
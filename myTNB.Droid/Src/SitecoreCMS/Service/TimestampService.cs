using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    internal class TimestampService
    {
        internal TimestampModel GetTimestamp(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Timestamp, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<TimestampModel> GenerateTimestamp(ScItemsResponse itemsResponse)
        {
            TimestampModel listItem = new TimestampModel();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                listItem.Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField);
                listItem.ID = item.Id;
            }

            return listItem;
        }
    }
}
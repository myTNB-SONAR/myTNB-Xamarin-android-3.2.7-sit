using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    internal class FullRTEPagesService
    {
        internal List<FullRTEPagesModel> GetFullRTEPages(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FullRTEPages, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateFullRTEPagesChildren(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<FullRTEPagesModel>> GenerateFullRTEPagesChildren(ScItemsResponse itemsResponse)
        {
            List<FullRTEPagesModel> list = new List<FullRTEPagesModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                FullRTEPagesModel listlItem = new FullRTEPagesModel
                {
                    GeneralText = item.GetValueFromField(Constants.Sitecore.Fields.Shared.GeneralText),
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Title),
                    PublishedDate = item.GetDateValueFromField(Constants.Sitecore.Fields.FullRTEPages.PublishedDate),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
    }
}
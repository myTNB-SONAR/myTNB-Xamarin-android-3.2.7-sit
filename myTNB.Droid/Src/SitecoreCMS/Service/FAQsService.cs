using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Models;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCM.Services
{
    internal class FAQsService
    {
        internal List<FAQsModel> GetFAQsService(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateFAQsChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<FAQsModel>> GenerateFAQsChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<FAQsModel> list = new List<FAQsModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                FAQsModel listlItem = new FAQsModel
                {
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language),
                    Question = item.GetValueFromField(Constants.Sitecore.Fields.FAQs.Question),
                    Answer = item.GetValueFromField(Constants.Sitecore.Fields.FAQs.Answer),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
        internal FAQsParentModel GetTimestamp(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<FAQsParentModel> GenerateTimestamp(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
        {
            FAQsParentModel listlItem = new FAQsParentModel();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                listlItem.Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField);
                listlItem.ID = item.Id;
            }

            return listlItem;
        }
    }
}
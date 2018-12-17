using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Services;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCM.Services
{
    internal class PromotionsV2Service
    {
        internal List<PromotionsModelV2> GetPromotionsService(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.PromotionsV2, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, websiteUrl, language);
            var item = req.Result;
            var list = GeneratePromotionChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<PromotionsModelV2>> GeneratePromotionChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<PromotionsModelV2> list = new List<PromotionsModelV2>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                PromotionsModelV2 listlItem = new PromotionsModelV2
                {
                    GeneralLinkUrl = item.GetItemIdFromLinkField(Constants.Sitecore.Fields.PromotionsV2.GeneralLink),
                    SubText = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.SubText),
                    Text = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.Text),
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.Title),
                    HeaderContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.HeaderContent),
                    BodyContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.BodyContent),
                    FooterContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.FooterContent),
                    PortraitImage = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.PromotionsV2.PortraitImage, OS, imageSize, websiteUrl, language),
                    LandscapeImage = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.PromotionsV2.LandscapeImage, OS, imageSize, websiteUrl, language),
                    PromoStartDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PromoStartDate),
                    PromoEndDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PromoEndDate),
                    PublishedDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PublishedDate),
                    IsPromoExpired = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.isPromoExpired),
                    ShowAtAppLaunch = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.ShowAtAppLaunch),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
        internal PromotionParentModelV2 GetTimestamp(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.PromotionsV2, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<PromotionParentModelV2> GenerateTimestamp(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
        {
            PromotionParentModelV2 listlItem = new PromotionParentModelV2();

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
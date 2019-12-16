using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Service
{
    internal class PromotionsV2Service
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal PromotionsV2Service(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<PromotionsModelV2> GetPromotionsService()
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.PromotionsV2, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = GeneratePromotionChildren(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<PromotionsModelV2>> GeneratePromotionChildren(ScItemsResponse itemsResponse)
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
                    HeaderContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.HeaderContent),
                    SubText = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.SubText),
                    Text = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.Text),
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.Title),
                    BodyContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.BodyContent),
                    FooterContent = item.GetValueFromField(Constants.Sitecore.Fields.PromotionsV2.FooterContent),
                    PortraitImage = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.PromotionsV2.PortraitImage, _websiteURL, false),
                    LandscapeImage = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.PromotionsV2.LandscapeImage, _websiteURL, false),
                    PromoStartDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PromoStartDate),
                    PromoEndDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PromoEndDate),
                    PublishedDate = item.GetDateValueFromField(Constants.Sitecore.Fields.PromotionsV2.PublishedDate),
                    IsPromoExpired = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.PromotionsV2.isPromoExpired),
                    ShowAtAppLaunch = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.PromotionsV2.ShowAtAppLaunch),
                    ID = item.Id,
                };
                Debug.WriteLine("debug: insert success");

                list.Add(listlItem);
            }

            return list;
        }
        internal PromotionParentModelV2 GetTimestamp()
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.PromotionsV2, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = GenerateTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<PromotionParentModelV2> GenerateTimestamp(ScItemsResponse itemsResponse)
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
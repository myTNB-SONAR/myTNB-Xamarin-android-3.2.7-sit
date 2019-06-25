using myTNB.SitecoreCM.Models;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Services;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCM.Services
{
    internal class PromotionsService
    {
        internal List<PromotionsModel> GetPromotionsService(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Promotions, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, websiteUrl, language);
            var item = req.Result;
            var list = GeneratePromotionChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<PromotionsModel>> GeneratePromotionChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<PromotionsModel> list = new List<PromotionsModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                PromotionsModel listlItem = new PromotionsModel
                {
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language),
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Title),
                    Text = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Text),
                    SubText = item.GetValueFromField(Constants.Sitecore.Fields.Shared.SubText),
                    CampaignPeriod = item.GetValueFromField(Constants.Sitecore.Fields.Promotions.CampaignPeriod),
                    Prizes = item.GetValueFromField(Constants.Sitecore.Fields.Promotions.Prizes),
                    HowToWin = item.GetValueFromField(Constants.Sitecore.Fields.Promotions.HowToWin),
                    FooterNote = item.GetValueFromField(Constants.Sitecore.Fields.Promotions.FooterNote),
                    PublishedDate = item.GetDateValueFromField(Constants.Sitecore.Fields.Promotions.PublishedDate),
                    //GeneralLinkText = item.GetTextFromLinkField(Constants.Sitecore.Fields.Shared.GeneralLink),
                    GeneralLinkUrl = item.GetItemIdFromLinkField(Constants.Sitecore.Fields.Shared.GeneralLink),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
        internal PromotionParentModel GetTimestamp(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Promotions, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<PromotionParentModel> GenerateTimestamp(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
        {
            PromotionParentModel listlItem = new PromotionParentModel();

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
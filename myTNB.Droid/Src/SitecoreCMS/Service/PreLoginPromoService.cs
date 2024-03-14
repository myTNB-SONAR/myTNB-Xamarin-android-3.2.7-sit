using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    internal class PreLoginPromoService
    {
        internal PreLoginPromoModel GetPreLoginPromo(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemById(Constants.Sitecore.ItemID.PreLoginPromo, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GeneratePreLoginPromo(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<PreLoginPromoModel> GeneratePreLoginPromo(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            PreLoginPromoModel listlItem = new PreLoginPromoModel();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                listlItem.GeneralLinkText = item.GetTextFromLinkField(Constants.Sitecore.Fields.Shared.GeneralLink);
                listlItem.GeneralLinkUrl = item.GetItemIdFromLinkField(Constants.Sitecore.Fields.Shared.GeneralLink);
                listlItem.Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language);
                listlItem.ID = item.Id;
            }

            return listlItem;
        }
    }
}
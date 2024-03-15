using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTNB.SitecoreCMS.Services
{
    internal class WalkthroughScreenService
    {
        internal List<WalkthroughScreensModel> GetWalkthroughScreens(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.WalkthroughScreens, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateWalkthroughScreenChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<WalkthroughScreensModel>> GenerateWalkthroughScreenChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<WalkthroughScreensModel> list = new List<WalkthroughScreensModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                WalkthroughScreensModel listlItem = new WalkthroughScreensModel
                {
                    Text = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Text),
                    SubText = item.GetValueFromField(Constants.Sitecore.Fields.Shared.SubText),
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
    }
}
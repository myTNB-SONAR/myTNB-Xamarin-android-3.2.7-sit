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
    internal class EnergyTipsService
    {
        internal List<EnergyTipsModel> GetEnergyTips(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemById(Constants.Sitecore.ItemID.EnergyTips, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, websiteUrl, language);
            var item = req.Result;
            var list = GenerateEnergyTipsChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<EnergyTipsModel>> GenerateEnergyTipsChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<EnergyTipsModel> list = new List<EnergyTipsModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                EnergyTipsModel listlItem = new EnergyTipsModel
                {
                    Text = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Text),
                    SubText = item.GetValueFromField(Constants.Sitecore.Fields.Shared.SubText),
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language),
                    CategoryTitle = item.GetFieldValueFromDropLink(Constants.Sitecore.Fields.EnergyTips.Category, Constants.Sitecore.Fields.Shared.Title, websiteUrl, language),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }

            return list;
        }
    }
}
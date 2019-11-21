using System;
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
    internal class AppLaunchImageService
    {
        internal List<AppLaunchImageModel> GetAppLaunchImageService(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.AppLaunchImage, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, websiteUrl, language);
            var item = req.Result;
            var list = GenerateAppLaunchImageChildren(item, OS, imageSize, websiteUrl, language);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<AppLaunchImageModel>> GenerateAppLaunchImageChildren(ScItemsResponse itemsResponse, string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            List<AppLaunchImageModel> list = new List<AppLaunchImageModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                AppLaunchImageModel listlItem = new AppLaunchImageModel
                {
                    ID = item.Id,
                    Title = item.GetValueFromField(Constants.Sitecore.Fields.Shared.Title),
                    Description = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunchImage.Description),
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, OS, imageSize, websiteUrl, language),
                    StartDateTime = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunchImage.StartDateTime),
                    EndDateTime = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunchImage.EndDateTime),
                    ShowForSeconds = item.GetValueFromField(Constants.Sitecore.Fields.AppLaunchImage.ShowForSeconds)
                };
                Debug.WriteLine("debug: insert success");

                list.Add(listlItem);
            }

            return list;
        }

        internal AppLaunchImageTimestamp GetTimestamp(string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.AppLaunchImage, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<AppLaunchImageTimestamp> GenerateTimestamp(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
        {
            AppLaunchImageTimestamp listlItem = new AppLaunchImageTimestamp();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];

                    if (item == null)
                        continue;

                    listlItem.Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField);
                    listlItem.ID = item.Id;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: AppLaunchImageService/GenerateTimestamp " + e.Message);
            }
            return listlItem;
        }
    }
}

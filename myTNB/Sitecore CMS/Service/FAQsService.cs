﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Service
{
    public class FAQsService
    {
        internal List<FAQsModel> GetFAQsService(string OS, string imageSize, string websiteUrl = null, string language = "en")
        {
            SitecoreService sitecoreService = new SitecoreService();

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, websiteUrl, language);
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
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
            var item = req.Result;
            var list = GenerateTimestamp(item, websiteUrl, language);
            var itemList = list.Result;
            return itemList;
        }

        async Task<FAQsParentModel> GenerateTimestamp(ScItemsResponse itemsResponse, string websiteUrl = null, string language = "en")
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
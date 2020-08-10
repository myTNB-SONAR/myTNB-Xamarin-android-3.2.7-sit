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
    public class FAQsService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal FAQsService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<FAQsModel> GetFAQsItems()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.FiveSecondTimeSpan);

            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = GenerateFAQsChildren(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        public async Task<IEnumerable<FAQsModel>> GenerateFAQsChildren(ScItemsResponse itemsResponse)
        {
            List<FAQsModel> list = new List<FAQsModel>();

            for (int i = 0; i < itemsResponse.ResultCount; i++)
            {
                ISitecoreItem item = itemsResponse[i];

                if (item == null)
                    continue;

                FAQsModel listlItem = new FAQsModel
                {
                    Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Shared.Image, _os, _imgSize, _websiteURL, _language),
                    Question = item.GetValueFromField(Constants.Sitecore.Fields.FAQs.Question),
                    Answer = item.GetValueFromField(Constants.Sitecore.Fields.FAQs.Answer),
                    ID = item.Id,
                };

                list.Add(listlItem);
            }
            return list;
        }

        internal FAQsParentModel GetTimestamp()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.FiveSecondTimeSpan);
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FAQs, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = GenerateTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<FAQsParentModel> GenerateTimestamp(ScItemsResponse itemsResponse)
        {
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }
                    return new FAQsParentModel
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in HelpService/GenerateTimestamp: " + e.Message);
            }
            return new FAQsParentModel();
        }
    }
}
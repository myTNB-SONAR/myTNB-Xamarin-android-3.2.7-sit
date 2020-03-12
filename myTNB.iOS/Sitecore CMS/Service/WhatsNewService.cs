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
    internal class WhatsNewService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal WhatsNewService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal WhatsNewTimestamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.WhatsNew
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<WhatsNewTimestamp> ParseToTimestamp(ScItemsResponse itemsResponse)
        {
            WhatsNewTimestamp whatsNewTimestamp = new WhatsNewTimestamp();
            await Task.Run(() =>
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
                        whatsNewTimestamp.Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField);
                        whatsNewTimestamp.ID = item.Id;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in WhatsNewService/GenerateTimestamp: " + e.Message);
                }
            });
            return whatsNewTimestamp;
        }

        internal List<WhatsNewCategoryModel> GetCategoryItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.WhatsNew
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToCategoryItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<WhatsNewModel> GetChildItems(ISitecoreItem categoryItem)
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(categoryItem.Path
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = GenerateWhatsNewChildren(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<WhatsNewCategoryModel>> ParseToCategoryItems(ScItemsResponse itemsResponse)
        {
            List<WhatsNewCategoryModel> list = new List<WhatsNewCategoryModel>();
            await Task.Run(() =>
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
                        list.Add(new WhatsNewCategoryModel
                        {
                            ID = GetValidID(item.Id),
                            CategoryName = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Category),
                            WhatsNewItems = GetChildItems(item)
                        });
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in WhatsNewService/ParseToCategoryItems: " + e.Message);
                }
            });
            return list;
        }

        private async Task<IEnumerable<WhatsNewModel>> GenerateWhatsNewChildren(ScItemsResponse itemsResponse)
        {
            List<WhatsNewModel> list = new List<WhatsNewModel>();
            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < itemsResponse.ResultCount; i++)
                    {
                        ISitecoreItem item = itemsResponse[i];

                        if (item == null)
                            continue;

                        WhatsNewModel listlItem = new WhatsNewModel
                        {
                            ID = GetValidID(item.Id),
                            Title = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Title),
                            TitleOnListing = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.TitleOnListing),
                            Description = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Description),
                            Image = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.Image, _websiteURL, false),
                            StartDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.StartDate),
                            EndDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.EndDate),
                            PublishDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PublishDate)
                        };

                        list.Add(listlItem);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in WhatsNewService/GenerateWhatsNewChildren: " + e.Message);
                }
            });
            return list;
        }

        private string GetValidID(string str)
        {
            string whatsNewId = str;

            try
            {
                var startStr = str.Substring(str.IndexOf('{') + 1);
                if (startStr.IsValid())
                {
                    whatsNewId = startStr?.Split('}')[0];
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetValidID: " + e.Message);
            }

            return whatsNewId;
        }
    }
}

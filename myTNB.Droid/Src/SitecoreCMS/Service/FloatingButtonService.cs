using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Services;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;


namespace myTNB.AndroidApp.Src.SitecoreCMS.Service
{
	public class FloatingButtonService
	{
        private string _os, _imgSize, _websiteURL, _language;
        internal FloatingButtonService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<FloatingButtonModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FloatingButton
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal FloatingButtonTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FloatingButton
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<FloatingButtonModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<FloatingButtonModel> list = new List<FloatingButtonModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new FloatingButtonModel
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButton.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButton.Description),
                        Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.FloatingButton.Image, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        StartDateTime = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButton.StartDateTime),
                        EndDateTime = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButton.EndDateTime),
                        ShowForSeconds = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButton.ShowForSeconds),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in FloatingButtonService/GetChildren: " + e.Message);
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<FloatingButtonTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new FloatingButtonTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in FloatingButtonService/GenerateTimestamp: " + e.Message);
            }
            return new FloatingButtonTimeStamp();
        }
    }
}


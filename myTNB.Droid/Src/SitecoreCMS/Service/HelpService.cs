using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Services;
using myTNB.Android.Src.SiteCore;
using myTNB.Android.Src.SitecoreCMS.Model;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
    internal class HelpService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal HelpService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<HelpModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Help
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal HelpTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Help
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<HelpModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<HelpModel> list = new List<HelpModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new HelpModel
                    {
                        TopicBGImage = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Help.TopicBGImage, _os, _imgSize, _websiteURL, _language),
                        BGStartColor = item.GetValueFromField(Constants.Sitecore.Fields.Help.BGStartColor),
                        BGEndColor = item.GetValueFromField(Constants.Sitecore.Fields.Help.BGEndColor),
                        BGGradientDirection = item.GetValueFromField(Constants.Sitecore.Fields.Help.BGGradientDirection),
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.Help.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.Help.Description),
                        TopicBodyTitle = item.GetValueFromField(Constants.Sitecore.Fields.Help.TopicBodyTitle),
                        TopicBodyContent = item.GetValueFromField(Constants.Sitecore.Fields.Help.TopicBodyContent),
                        CTA = item.GetValueFromField(Constants.Sitecore.Fields.Help.CTA),
                        Tags = item.GetValueFromField(Constants.Sitecore.Fields.Help.Tags),
                        TargetItem = item.GetValueFromField(Constants.Sitecore.Fields.Help.TargetItem),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in HelpService/GetChildren: " + e.Message);
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<HelpTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new HelpTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ShowNeedHelp = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.Timestamp.ShowNeedHelpField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in HelpService/GenerateTimestamp: " + e.Message);
            }
            return new HelpTimeStamp();
        }
    }
}
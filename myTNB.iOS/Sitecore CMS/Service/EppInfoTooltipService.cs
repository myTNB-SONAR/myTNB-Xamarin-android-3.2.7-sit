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

//Created by Syahmi ICS 05052020

namespace myTNB.SitecoreCMS.Service
{
    public class EppInfoTooltipService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal EppInfoTooltipService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<EppTooltipModelEntity> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.EppToolTip //EppInfo Item Path
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal EppTooltipTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.EppToolTip //Timestamp Checking Item Path
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<EppTooltipModelEntity>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<EppTooltipModelEntity> list = new List<EppTooltipModelEntity>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new EppTooltipModelEntity
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.EppInfoTooltip.Title),
                        PopUpTitle = item.GetValueFromField(Constants.Sitecore.Fields.EppInfoTooltip.PopUpTitle),
                        PopUpBody = item.GetValueFromField(Constants.Sitecore.Fields.EppInfoTooltip.PopUpBody),
                        Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.EppInfoTooltip.Image, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in EppInfoTooltipService/GetChildren: " + e.Message);
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<EppTooltipTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new EppTooltipTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField), //Timestamp checking
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in EppInfoTooltipService/GenerateTimestamp: " + e.Message);
            }
            return new EppTooltipTimeStamp();
        }
    }
}

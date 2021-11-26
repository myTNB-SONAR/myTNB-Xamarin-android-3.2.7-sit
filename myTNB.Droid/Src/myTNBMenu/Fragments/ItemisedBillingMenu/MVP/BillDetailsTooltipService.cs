using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB_Android.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Service
{
    public class BillDetailsTooltipService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal BillDetailsTooltipService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<BillsTooltipModelEntity> GetItems(BillsTooltipVersionEnum version)
        {
            string itemPath;
            switch (version)
            {
                case BillsTooltipVersionEnum.V1:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltip;
                    break;
                case BillsTooltipVersionEnum.V2:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltipV2;
                    break;
                default:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltip;
                    break;
            }

            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(itemPath, PayloadType.Content, new List<ScopeType> { ScopeType.Children },
                SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal BillsTooltipTimeStamp GetTimeStamp(BillsTooltipVersionEnum version)
        {
            string itemPath;
            switch (version)
            {
                case BillsTooltipVersionEnum.V1:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltip;
                    break;
                case BillsTooltipVersionEnum.V2:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltipV2;
                    break;
                default:
                    itemPath = Constants.Sitecore.ItemPath.BillDetailsTooltip;
                    break;
            }

            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(itemPath, PayloadType.Content, new List<ScopeType> { ScopeType.Self },
                SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<BillsTooltipModelEntity>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<BillsTooltipModelEntity> list = new List<BillsTooltipModelEntity>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new BillsTooltipModelEntity
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.BillDetailsTooltip.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.BillDetailsTooltip.Description),
                        Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.BillDetailsTooltip.Image, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in BillDetailsTooltipService/GetChildren: " + e.Message);
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<BillsTooltipTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new BillsTooltipTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in BillDetailsTooltipService/GenerateTimestamp: " + e.Message);
            }
            return new BillsTooltipTimeStamp();
        }
    }
}

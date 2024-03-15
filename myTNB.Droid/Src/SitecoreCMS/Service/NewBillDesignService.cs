using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Services;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Diagnostics;
using myTNB.SitecoreCMS;
using System.Linq;

namespace myTNB.AndroidApp.Src.SitecoreCMS.Service
{
    public class NewBillDesignService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal NewBillDesignService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal NewBillDesignTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.NewBillDesignDiscoverMore
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<NewBillDesignTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new NewBillDesignTimeStamp
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
            return new NewBillDesignTimeStamp();
        }

        internal List<NewBillDesignModelEntity> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.NewBillDesignDiscoverMore
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<NewBillDesignModelEntity>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<NewBillDesignModelEntity> list = new List<NewBillDesignModelEntity>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new NewBillDesignModelEntity
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.Description),
                        Image1 = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.Image1, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        Image2 = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.Image2, _os, _imgSize, _websiteURL, _language).Replace(" ", "%20"),
                        IsHeader = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.IsHeader),
                        IsFooter = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.IsFooter),
                        IsZoomable = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.IsZoomable),
                        ID = item.Id,
                        ShouldTrack = item.GetCheckBoxValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.ShouldTrack),
                        DynatraceTag = item.GetValueFromField(Constants.Sitecore.Fields.NewBillDesignDiscoverMore.DynatraceTag)
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in BillDetailsTooltipService/GetChildren: " + e.Message);
            }
            return list;
        }
    }
}
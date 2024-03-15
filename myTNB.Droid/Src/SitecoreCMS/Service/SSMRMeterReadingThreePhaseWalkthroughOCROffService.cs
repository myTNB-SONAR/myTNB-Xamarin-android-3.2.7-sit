using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.SiteCore;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
    public class SSMRMeterReadingThreePhaseWalkthroughOCROffService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal SSMRMeterReadingThreePhaseWalkthroughOCROffService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<SSMRMeterReadingModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.SSMRMeterReadingThreePhaseWalkthroughOCROff
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal SSMRMeterReadingTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.SSMRMeterReadingThreePhaseWalkthroughOCROff
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<SSMRMeterReadingModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<SSMRMeterReadingModel> list = new List<SSMRMeterReadingModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new SSMRMeterReadingModel
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.SSMRMeterReadingWalkthrough.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.SSMRMeterReadingWalkthrough.Description),
                        Image = item.GetImageUrlFromMediaField(_imgSize, _websiteURL),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in SSMRMeterReadingThreePhaseWalkthroughOCROffService/GetChildren: " + e.Message);
            }
            return list;
        }

        private async Task<SSMRMeterReadingTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new SSMRMeterReadingTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in SSMRMeterReadingThreePhaseWalkthroughOCROffService/GenerateTimestamp: " + e.Message);
            }
            return new SSMRMeterReadingTimeStamp();
        }
    }
}
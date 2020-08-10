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
    public class ApplySSMRWalkthroughService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal ApplySSMRWalkthroughService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<ApplySSMRModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.FiveSecondTimeSpan);
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.ApplySSMRWalkthrough
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal ApplySSMRTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.FiveSecondTimeSpan);
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.ApplySSMRWalkthrough
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<ApplySSMRModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<ApplySSMRModel> list = new List<ApplySSMRModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }

                    list.Add(new ApplySSMRModel
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.ApplySSMRWalkthrough.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.ApplySSMRWalkthrough.Description),
                        Image = item.GetImageUrlFromMediaField(_imgSize, _websiteURL),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in ApplySSMRWalkthroughService/GetChildren: " + e.Message);
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<ApplySSMRTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new ApplySSMRTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in ApplySSMRWalkthroughService/GenerateTimestamp: " + e.Message);
            }
            return new ApplySSMRTimeStamp();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.Utils;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.SitecoreCMS.Model;
using Sitecore.MobileSDK.API.Exceptions;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
	public class FloatingButtonMarketingService
	{
        private string _os, _imgSize, _websiteURL, _language;
        public bool IsChildItemError = false;
        internal FloatingButtonMarketingService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<FloatingButtonMarketingModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FloatingButtonMarketing
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.TenSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToChildrenItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<FloatingButtonMarketingModel> GetChildrenItems(string path)
        {
            try
            {
                SitecoreService sitecoreService = new SitecoreService();
                var req = sitecoreService.GetItemByPath(path
                    , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.TenSecondTimeSpan, _websiteURL, _language);
                var item = req.Result;
                var list = ParseToChildrenItems(item);
                var itemList = list.Result;
                return itemList.ToList();
            }
            catch (SitecoreMobileSdkException ex)
            {
                IsChildItemError = true;
                Utility.LoggingNonFatalError(ex);
            }
            catch (Exception e)
            {
                IsChildItemError = true;
                Utility.LoggingNonFatalError(e);
            }
            return new List<FloatingButtonMarketingModel>();
        }

        internal FloatingButtonMarketingTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.FloatingButtonMarketing
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }



        private async Task<IEnumerable<FloatingButtonMarketingModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<FloatingButtonMarketingModel> list = new List<FloatingButtonMarketingModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.DisplayName))
                    {
                        continue;
                    }

                    FloatingButtonMarketingModel newItem = new FloatingButtonMarketingModel();

                    newItem = new FloatingButtonMarketingModel
                    {
                       
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButtonMarketing.Title),
                        ButtonTitle = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButtonMarketing.ButtonTitle),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.FloatingButtonMarketing.Description),
                        Infographic_FullView_URL = item.GetFileURLFromFieldName(Constants.Sitecore.Fields.FloatingButtonMarketing.Infographic_FullView_URL, _websiteURL),
                        ID = item.Id
                    };

                    
                    if (newItem.Description.Contains("<img"))
                    {
                        string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                        System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(newItem.Description, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                        foreach (System.Text.RegularExpressions.Match m in matchesImgSrc)
                        {
                            string href = m.Groups[1].Value;
                            if (!href.Contains("http"))
                            {
                                href = item.GetImageUrlFromExtractedUrl(m.Groups[1].Value, _websiteURL);
                            }
                            newItem.Description = newItem.Description.Replace(m.Groups[1].Value, href);
                        }
                    }

                    list.Add(newItem);
                }

                IsChildItemError = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in FloatingButtonMarketingService/GetChildren: " + e.Message);
                IsChildItemError = true;
            }
            return list;
        }

        private int GetIntFromStringValue(string val)
        {
            int parsedValue;
            Int32.TryParse(val, out parsedValue);
            return parsedValue;
        }

        private async Task<FloatingButtonMarketingTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new FloatingButtonMarketingTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in FloatingButtonMarketingService/GenerateTimestamp: " + e.Message);
            }
            return new FloatingButtonMarketingTimeStamp();
        }
    }
}


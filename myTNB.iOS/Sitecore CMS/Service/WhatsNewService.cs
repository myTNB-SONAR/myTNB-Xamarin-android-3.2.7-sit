using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            await Task.Run(async() =>
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
                            PublishDate = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PublishDate),
                            Image_DetailsView = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.Image_DetailsView, _websiteURL, false),
                            Styles_DetailsView = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Styles_DetailsView),
                            PortraitImage_PopUp = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.PortraitImage_PopUp, _websiteURL, false),
                            PopUp_HeaderImage = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.WhatsNew.PopUp_HeaderImage, _websiteURL, false),
                            PopUp_Text_Content = item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Content),
                        };

                        try
                        {
                            listlItem.ShowEveryCountDays_PopUp = !string.IsNullOrEmpty(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowEveryCountDays_PopUp)) ? int.Parse(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowEveryCountDays_PopUp)) : 0;
                        }
                        catch (Exception ex)
                        {
                            listlItem.ShowEveryCountDays_PopUp = 0;
                        }
                        try
                        {
                            listlItem.ShowForTotalCountDays_PopUp = !string.IsNullOrEmpty(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowForTotalCountDays_PopUp)) ? int.Parse(item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowForTotalCountDays_PopUp)) : 0;
                        }
                        catch (Exception ex)
                        {
                            listlItem.ShowForTotalCountDays_PopUp = 0;
                        }
                        try
                        {
                            listlItem.ShowAtAppLaunchPopUp = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowAtAppLaunchPopUp).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.ShowAtAppLaunchPopUp).ToUpper().Trim() == "TRUE") ? true : false;
                        }
                        catch (Exception ex)
                        {
                            listlItem.ShowAtAppLaunchPopUp = false;
                        }

                        try
                        {
                            listlItem.PopUp_Text_Only = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Only).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.PopUp_Text_Only).ToUpper().Trim() == "TRUE") ? true : false;
                        }
                        catch (Exception ex)
                        {
                            listlItem.PopUp_Text_Only = false;
                        }

                        try
                        {
                            listlItem.Donot_Show_In_WhatsNew = (item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Donot_Show_In_WhatsNew).ToUpper().Trim() == "1" || item.GetValueFromField(Constants.Sitecore.Fields.WhatsNew.Donot_Show_In_WhatsNew).ToUpper().Trim() == "TRUE") ? true : false;
                        }
                        catch (Exception ex)
                        {
                            listlItem.Donot_Show_In_WhatsNew = false;
                        }

                        if (listlItem.Description.Contains("<img"))
                        {
                            string urlRegex = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                            System.Text.RegularExpressions.MatchCollection matchesImgSrc = System.Text.RegularExpressions.Regex.Matches(listlItem.Description, urlRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                            foreach (System.Text.RegularExpressions.Match m in matchesImgSrc)
                            {
                                string href = m.Groups[1].Value;
                                if (!href.Contains("http"))
                                {
                                    href = item.GetImageUrlFromExtractedUrl(m.Groups[1].Value, _websiteURL);
                                    listlItem.Description = listlItem.Description.Replace(m.Groups[1].Value, href);
                                }
                            }
                        }

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

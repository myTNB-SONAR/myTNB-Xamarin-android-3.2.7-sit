using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Utils;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
    public class RewardsService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal RewardsService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal List<RewardsCategoryModel> GetItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<RewardsModel> GetChildrenItems(string path)
        {
            try
            {
                SitecoreService sitecoreService = new SitecoreService();
                var req = sitecoreService.GetItemByPath(path
                    , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
                var item = req.Result;
                var list = ParseToChildrenItems(item);
                var itemList = list.Result;
                return itemList.ToList();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return new List<RewardsModel>();
        }

        internal RewardsTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<IEnumerable<RewardsCategoryModel>> ParseItems(ScItemsResponse itemsResponse)
        {
            List<RewardsCategoryModel> list = new List<RewardsCategoryModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }
                    list.Add(new RewardsCategoryModel
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Title),
                        DisplayName = item.DisplayName,
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Description),
                        RewardList = string.IsNullOrEmpty(item.Path) ? new List<RewardsModel>() : GetChildrenItems(item.Path),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GetMainChildren: " + e.Message);
            }
            return list;
        }

        private async Task<IEnumerable<RewardsModel>> ParseToChildrenItems(ScItemsResponse itemsResponse)
        {
            List<RewardsModel> list = new List<RewardsModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null)
                    {
                        continue;
                    }
                         // Image = item.GetImageUrlFromMediaField(_imgSize, _websiteURL),
                    list.Add(new RewardsModel
                    {
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Title),
                        DisplayName = item.DisplayName,
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Description),
                        Image = item.GetImageUrlFromItemWithSize(Constants.Sitecore.Fields.Rewards.Image, _os, _imgSize, _websiteURL, _language),
                        ID = item.Id
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GetChildren: " + e.Message);
            }
            return list;
        }

        private async Task<RewardsTimeStamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new RewardsTimeStamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GenerateTimestamp: " + e.Message);
            }
            return new RewardsTimeStamp();
        }
    }
}
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
    internal class RewardsService
    {
        private string _os, _imgSize, _websiteURL, _language;
        internal RewardsService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal RewardsTimestamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<RewardsTimestamp> ParseToTimestamp(ScItemsResponse itemsResponse)
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
                    return new RewardsTimestamp
                    {
                        Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField),
                        ID = item.Id
                    };
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in LanguageService/GenerateTimestamp: " + e.Message);
            }
            return new RewardsTimestamp();
        }

        internal List<RewardsCategoryModel> GetCategoryItems()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = ParseToCategoryItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<RewardsModel> GetChildItems(ISitecoreItem categoryItem)
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(categoryItem.Path
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, _websiteURL, _language);
            var item = req.Result;
            var list = GenerateRewardsChildren(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        private async Task<IEnumerable<RewardsCategoryModel>> ParseToCategoryItems(ScItemsResponse itemsResponse)
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
                        ID = item.Id,
                        CategoryName = item.DisplayName,
                        Rewards = GetChildItems(item)
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/ParseToCategoryItems: " + e.Message);
            }
            return list;
        }

        private async Task<IEnumerable<RewardsModel>> GenerateRewardsChildren(ScItemsResponse itemsResponse)
        {
            List<RewardsModel> list = new List<RewardsModel>();

            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];

                    if (item == null)
                        continue;

                    RewardsModel listlItem = new RewardsModel
                    {
                        ID = item.Id,
                        RewardName = item.DisplayName,
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Description),
                        Image = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.Rewards.Image, _websiteURL, false)
                    };

                    list.Add(listlItem);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GenerateRewardsChildren: " + e.Message);
            }

            return list;
        }
    }
}

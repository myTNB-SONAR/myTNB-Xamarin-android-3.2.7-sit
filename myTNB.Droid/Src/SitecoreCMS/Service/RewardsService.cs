using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Extensions;
using myTNB.SitecoreCMS.Model;
using myTNB.AndroidApp.Src.SiteCore;
using myTNB.AndroidApp.Src.Utils;
using Sitecore.MobileSDK.API.Exceptions;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request.Parameters;

namespace myTNB.SitecoreCMS.Services
{
    public class RewardsService
    {
        private string _os, _imgSize, _websiteURL, _language;
        public bool IsChildItemError = false;
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
                , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.TenSecondTimeSpan, _websiteURL, _language);
            var item = req.Result;
            var list = ParseItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<RewardsModel> GetChildrenItems(string path, RewardsCategoryModel rewardCategory)
        {
            try
            {
                SitecoreService sitecoreService = new SitecoreService();
                var req = sitecoreService.GetItemByPath(path
                    , PayloadType.Content, new List<ScopeType> { ScopeType.Children }, SiteCoreConfig.TenSecondTimeSpan, _websiteURL, _language);
                var item = req.Result;
                var list = ParseToChildrenItems(item, rewardCategory);
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
            return new List<RewardsModel>();
        }

        internal RewardsTimeStamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService();
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content, new List<ScopeType> { ScopeType.Self }, SiteCoreConfig.FiveSecondTimeSpan, _websiteURL, _language);
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
                IsChildItemError = false;

                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.Path) || string.IsNullOrEmpty(item.DisplayName))
                    {
                        continue;
                    }

                    RewardsCategoryModel rewardCategory = new RewardsCategoryModel()
                    {
                        CategoryName = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardCategory),
                        ID = item.Id
                    };
                    List<RewardsModel> rewardList = GetChildrenItems(item.Path, rewardCategory);

                    if (IsChildItemError)
                    {
                        break;
                    }
                    else
                    {
                        if (rewardList != null && rewardList.Count > 0)
                        {
                            rewardCategory.RewardList = rewardList;
                            list.Add(rewardCategory);
                        }
                    }
                }

                if (IsChildItemError)
                {
                    return new List<RewardsCategoryModel>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GetMainChildren: " + e.Message);
            }
            return list;
        }

        private async Task<IEnumerable<RewardsModel>> ParseToChildrenItems(ScItemsResponse itemsResponse, RewardsCategoryModel rewardCategory)
        {
            List<RewardsModel> list = new List<RewardsModel>();
            try
            {
                for (int i = 0; i < itemsResponse.ResultCount; i++)
                {
                    ISitecoreItem item = itemsResponse[i];
                    if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.DisplayName))
                    {
                        continue;
                    }
                    list.Add(new RewardsModel
                    {
                        CategoryName = rewardCategory.CategoryName,
                        CategoryID = rewardCategory.ID,
                        TitleOnListing = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.TitleOnListing),
                        PeriodLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.PeriodLabel),
                        LocationLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.LocationLabel),
                        TandCLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.TandCLabel),
                        StartDate = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.StartDate),
                        EndDate = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.EndDate),
                        Title = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Title),
                        Description = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Description),
                        Image = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.Rewards.Image, _websiteURL, false),
                        RewardUseWithinTime = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseWithinTime),
                        RewardUseTitle = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseTitle),
                        RewardUseDescription = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseDescription),
                        ID = item.Id
                    });
                }

                IsChildItemError = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in RewardsService/GetChildren: " + e.Message);
                IsChildItemError = true;
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
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
        internal bool HasChildItemError { private set; get; }
        internal RewardsService(string os, string imageSize, string websiteUrl = null, string language = "en")
        {
            _os = os;
            _imgSize = imageSize;
            _websiteURL = websiteUrl;
            _language = language;
        }

        internal RewardsTimestamp GetTimeStamp()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.FiveSecondTimeSpan);
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content
                , new List<ScopeType> { ScopeType.Self }
                , _websiteURL
                , _language);
            var item = req.Result;
            var list = ParseToTimestamp(item);
            var itemList = list.Result;
            return itemList;
        }

        private async Task<RewardsTimestamp> ParseToTimestamp(ScItemsResponse itemsResponse)
        {
            RewardsTimestamp rewardsTimestamp = new RewardsTimestamp();
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
                        rewardsTimestamp.Timestamp = item.GetValueFromField(Constants.Sitecore.Fields.Timestamp.TimestampField);
                        rewardsTimestamp.ID = item.Id;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in LanguageService/GenerateTimestamp: " + e.Message);
                }
            });
            return rewardsTimestamp;
        }

        internal List<RewardsCategoryModel> GetCategoryItems()
        {
            SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.TenSecondTimeSpan);
            var req = sitecoreService.GetItemByPath(Constants.Sitecore.ItemPath.Rewards
                , PayloadType.Content
                , new List<ScopeType> { ScopeType.Children }
                , _websiteURL, _language);
            var item = req.Result;
            var list = ParseToCategoryItems(item);
            var itemList = list.Result;
            return itemList.ToList();
        }

        internal List<RewardsModel> GetChildItems(ISitecoreItem categoryItem)
        {
            try
            {
                SitecoreService sitecoreService = new SitecoreService(Constants.TimeOut.TenSecondTimeSpan);
                var req = sitecoreService.GetItemByPath(categoryItem.Path
                    , PayloadType.Content
                    , new List<ScopeType> { ScopeType.Children }
                    , _websiteURL, _language);
                var item = req.Result;
                var list = GenerateRewardsChildren(item);
                var itemList = list.Result;
                return itemList.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("What's New GetChildItems Error: " + ex.Message);
                HasChildItemError = true;
            }
            return new List<RewardsModel>();
        }

        private async Task<IEnumerable<RewardsCategoryModel>> ParseToCategoryItems(ScItemsResponse itemsResponse)
        {
            List<RewardsCategoryModel> list = new List<RewardsCategoryModel>();
            await Task.Run(() =>
            {
                try
                {
                    HasChildItemError = false;
                    for (int i = 0; i < itemsResponse.ResultCount; i++)
                    {
                        ISitecoreItem item = itemsResponse[i];
                        if (item == null)
                        {
                            continue;
                        }

                        List<RewardsModel> childItems = GetChildItems(item);
                        if (HasChildItemError)
                        {
                            break;
                        }
                        else
                        {
                            list.Add(new RewardsCategoryModel
                            {
                                ID = GetValidID(item.Id),
                                CategoryName = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Category),
                                Rewards = childItems
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in RewardsService/ParseToCategoryItems: " + e.Message);
                }
            });
            return list;
        }

        private async Task<IEnumerable<RewardsModel>> GenerateRewardsChildren(ScItemsResponse itemsResponse)
        {
            List<RewardsModel> list = new List<RewardsModel>();
            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < itemsResponse.ResultCount; i++)
                    {
                        ISitecoreItem item = itemsResponse[i];

                        if (item == null)
                            continue;

                        RewardsModel listlItem = new RewardsModel
                        {
                            ID = GetValidID(item.Id),
                            RewardName = item.DisplayName,
                            Title = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Title),
                            TitleOnListing = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.TitleOnListing),
                            Description = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.Description),
                            Image = item.GetImageUrlFromMediaField(Constants.Sitecore.Fields.Rewards.Image, _websiteURL, false),
                            PeriodLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardPeriodText),
                            LocationLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.LocationsText),
                            TandCLabel = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.TermsAndConditions),
                            StartDate = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.StartDateTime),
                            EndDate = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.EndDateTime),
                            RewardUseWithinTime = item.GetIntValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseWithinTime),
                            RewardUseTitle = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseTitle),
                            RewardUseDescription = item.GetValueFromField(Constants.Sitecore.Fields.Rewards.RewardUseDescription)
                        };

                        list.Add(listlItem);
                        HasChildItemError = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception in RewardsService/GenerateRewardsChildren: " + e.Message);
                    HasChildItemError = true;
                }
            });
            return list;
        }

        private string GetValidID(string str)
        {
            string rewardId = str;

            try
            {
                var startStr = str.Substring(str.IndexOf('{') + 1);
                if (startStr.IsValid())
                {
                    rewardId = startStr?.Split('}')[0];
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception in GetValidID: " + e.Message);
            }

            return rewardId;
        }
    }
}

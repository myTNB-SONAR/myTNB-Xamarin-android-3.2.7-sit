using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using Firebase.DynamicLinks;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SitecoreCMS.Services;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB
{
    public static class RewardsServices
    {
        public enum RewardProperties
        {
            Read,
            Favourite,
            Redeemed
        }

        private static Dictionary<string, string> RewardsDictionary = new Dictionary<string, string>(){
            { RewardProperties.Read.ToString(), "ReadDate"}
            , { RewardProperties.Favourite.ToString(), "FavUpdatedDate"}
            , { RewardProperties.Redeemed.ToString(), "RedeemedDate"}
        };

        private static GetUserRewardsResponseModel _userRewards;
        private static UpdateRewardsResponseModel _updateRewards;

        public static void UpdateRewardsSitecorCache(RewardsItemModel updatedReward = null)
        {
            List<RewardsModel> rewardsList = new RewardsEntity().GetAllItems();
            if (rewardsList == null) { return; }
            if (updatedReward != null && updatedReward.RewardId.IsValid()
                && _updateRewards != null && _updateRewards.d != null && _updateRewards.d.IsSuccess)
            {
                int index = rewardsList.FindIndex(x => x.ID == updatedReward.RewardId);
                if (index > -1 && index < rewardsList.Count && rewardsList[index] != null)
                {
                    RewardsModel reward = rewardsList[index];
                    reward.IsRead = updatedReward.Read;
                    reward.IsSaved = updatedReward.Favourite;
                    reward.IsUsed = updatedReward.Redeemed;
                    UpdateRewardItem(reward);
                }
            }
            else
            {
                if (_userRewards != null && _userRewards.d != null && _userRewards.d.IsSuccess
                    && _userRewards.d.data != null && _userRewards.d.data.UserRewards != null)
                {
                    for (int i = 0; i < _userRewards.d.data.UserRewards.Count; i++)
                    {
                        RewardsItemModel userReward = _userRewards.d.data.UserRewards[i];
                        int index = rewardsList.FindIndex(x => x.ID == userReward.RewardId);
                        if (index > -1 && index < rewardsList.Count && rewardsList[index] != null)
                        {
                            RewardsModel reward = rewardsList[index];
                            reward.IsRead = userReward.Read;
                            reward.IsSaved = userReward.Favourite;
                            reward.IsUsed = userReward.Redeemed;
                            UpdateRewardItem(reward);
                        }
                    }
                }
            }
        }

        public static void UpdateRewardItem(RewardsModel reward)
        {
            RewardsEntity rewardsEntity = new RewardsEntity();
            rewardsEntity.UpdateEntity(reward);
        }

        public static UIImage ConvertToGrayScale(UIImage image)
        {
            if (image != null)
            {
                try
                {
                    RectangleF imageRect = new RectangleF(PointF.Empty, (System.Drawing.SizeF)image.Size);
                    var colorSpace = CGColorSpace.CreateDeviceGray();
                    using (var context = new CGBitmapContext(IntPtr.Zero, (int)imageRect.Width, (int)imageRect.Height, 8, 0, colorSpace, CGImageAlphaInfo.None))
                    {
                        context.DrawImage(imageRect, image.CGImage);
                        var grayImage = context.ToImage();
                        using (var maskContext = new CGBitmapContext(null, (int)imageRect.Width, (int)imageRect.Height, 8, 0, null, CGImageAlphaInfo.Only))
                        {
                            maskContext.DrawImage(imageRect, image.CGImage);
                            using (var mask = maskContext.ToImage())
                            {
                                return UIImage.FromImage(grayImage.WithMask(mask), image.CurrentScale, image.Orientation);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in ConvertToGrayScale: " + e.Message);
                    return image;
                }
            }
            return image;
        }

        public static bool RewardHasExpired(RewardsModel reward)
        {
            bool res = true;
            if (reward != null && reward.ID.IsValid())
            {
                if (reward.EndDate.IsValid())
                {
                    try
                    {
                        DateTime endDate = DateTime.ParseExact(reward.EndDate, "yyyyMMddTHHmmss", DateHelper.DateCultureInfo);
                        DateTime now = DateTime.Now;
                        if (now < endDate)
                        {
                            res = false;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Parse Error: " + e.Message);
                    }
                }
            }
            return res;
        }

        public static void OpenRewardDetails(string rewardId, UIViewController topView)
        {
            if (!AppLaunchMasterCache.IsRewardsDisabled)
            {
                if (topView != null && !(topView is RewardDetailsViewController))
                {
                    if (rewardId.IsValid())
                    {
                        var reward = RewardsEntity.GetItem(rewardId);
                        if (reward != null)
                        {
                            if (!RewardHasExpired(reward))
                            {
                                RewardsCache.RefreshReward = true;
                                RewardDetailsViewController rewardDetailView = new RewardDetailsViewController();
                                DateTime? rDate = RewardsCache.GetRedeemedDate(reward.ID);
                                string rDateStr = string.Empty;
                                if (rDate != null)
                                {
                                    try
                                    {
                                        DateTime? rDateValue = rDate.Value.ToLocalTime();
                                        rDateStr = rDateValue.Value.ToString(RewardsConstants.Format_Date, DateHelper.DateCultureInfo);
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine("Error in ParseDate: " + e.Message);
                                    }
                                }
                                rewardDetailView.RedeemedDate = rDateStr;
                                rewardDetailView.RewardModel = reward;
                                UINavigationController navController = new UINavigationController(rewardDetailView)
                                {
                                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                                };
                                topView.PresentViewController(navController, true, null);
                            }
                            else
                            {
                                ShowRewardExpired(topView);
                            }
                        }
                        else
                        {
                            ShowRewardExpired(topView);
                        }
                    }
                    else
                    {
                        ShowRewardExpired(topView);
                    }
                }
            }
            else
            {
                ShowRewardUnavailable();
            }
        }

        public static void ShowRewardExpired(UIViewController topView)
        {
            if (topView != null)
            {
                topView.InvokeOnMainThread(() =>
                {
                    AlertHandler.DisplayCustomAlert(LanguageUtility.GetCommonI18NValue(Constants.Common_RewardNotAvailableTitle),
                LanguageUtility.GetCommonI18NValue(Constants.Common_RewardNotAvailableDesc),
                new Dictionary<string, Action> {
                {LanguageUtility.GetCommonI18NValue(Constants.Common_ShowMoreRewards), () =>
                {
                    if (topView is HomeTabBarController)
                    {
                        HomeTabBarController tabBar = topView as HomeTabBarController;
                        tabBar.SelectedIndex = 3;
                    }
                    else if (topView.TabBarController != null)
                    {
                        topView.TabBarController.SelectedIndex = 3;
                    }
                    RewardsCache.RefreshReward = true;
                }}});
                });
            }
        }

        public static void ShowRewardUnavailable()
        {
            AlertHandler.DisplayCustomAlert(LanguageUtility.GetErrorI18NValue(Constants.Error_RewardsUnavailableTitle),
                    LanguageUtility.GetErrorI18NValue(Constants.Error_RewardsUnavailableMsg),
                    new Dictionary<string, Action> {
                        { LanguageUtility.GetCommonI18NValue(Constants.Common_GotIt), null} });
        }

        #region Rewards Services
        public static async Task<GetUserRewardsResponseModel> GetUserRewards()
        {
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                serviceManager.usrInf
            };
            GetUserRewardsResponseModel response = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<GetUserRewardsResponseModel>("GetUserRewards", requestParameter);
            });
            _userRewards = response;
            RewardsCache.AddGetUserRewardsResponseData(response);
            UpdateRewardsSitecorCache();
            return response;
        }

        public static async Task<UpdateRewardsResponseModel> UpdateRewards(RewardsModel sitecoreReward, RewardProperties prop, bool value)
        {
            DateTime date = DateTime.UtcNow;
            RewardsItemModel reward = null;

            if (_userRewards != null && _userRewards.d != null && _userRewards.d.IsSuccess
                && _userRewards.d.data != null && _userRewards.d.data.UserRewards != null)
            {
                int index = _userRewards.d.data.UserRewards.FindIndex(x => x.RewardId == sitecoreReward.ID);
                if (index > -1)
                {
                    reward = _userRewards.d.data.UserRewards[index];
                }
            }

            if (reward == null)
            {
                string email = DataManager.DataManager.SharedInstance.User.Email ?? string.Empty;
                if (string.IsNullOrEmpty(email) && DataManager.DataManager.SharedInstance.UserEntity != null &&
                    DataManager.DataManager.SharedInstance.UserEntity.Count > 0 &&
                    DataManager.DataManager.SharedInstance.UserEntity[0] != null)
                {
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email ?? string.Empty;
                }
                reward = new RewardsItemModel
                {
                    Email = email,//email,//"reward001@gmail.com",
                    RewardId = sitecoreReward.ID,
                    Read = sitecoreReward.IsRead,
                    Favourite = sitecoreReward.IsSaved,
                    Redeemed = sitecoreReward.IsUsed
                };
            }

            try
            {
                typeof(RewardsItemModel).GetProperty(prop.ToString()).SetValue(reward, value);
                if (value)
                {
                    typeof(RewardsItemModel).GetProperty(RewardsDictionary[prop.ToString()]).SetValue(reward, date);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in UpdateRewards: " + e.Message);
            }

            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                serviceManager.usrInf,
                /*usrInf = new
                {
                    eid = "reward001@gmail.com",
                    sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                    did = DataManager.DataManager.SharedInstance.UDID,
                    ft = "token",
                    lang = TNBGlobal.APP_LANGUAGE,
                    sec_auth_k1 = TNBGlobal.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                },*/
                reward
            };
            UpdateRewardsResponseModel response = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<UpdateRewardsResponseModel>("UpdateRewards", requestParameter);
            });
            _updateRewards = response;
            UpdateRewardsSitecorCache(reward);
            return response;
        }

        public static bool FilterExpiredRewards()
        {
            bool isExpired = false;
            RewardsEntity rewardsEntity = new RewardsEntity();
            var list = rewardsEntity.GetAllItems();
            if (list != null && list.Count > 0)
            {
                foreach (var reward in list)
                {
                    if (RewardHasExpired(reward))
                    {
                        isExpired = true;
                        rewardsEntity.DeleteItem(reward.ID);
                    }
                }
            }
            return isExpired;
        }

        public static async Task<bool> RewardListHasUpdates()
        {
            bool needsUpdate = true;
            await Task.Run(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                , DataManager.DataManager.SharedInstance.ImageSize
                , TNBGlobal.SITECORE_URL
                , TNBGlobal.APP_LANGUAGE);

                RewardsTimestampResponseModel timeStamp = iService.GetRewardsTimestampItem();

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new RewardsTimestampResponseModel();
                    timeStamp.Data = new List<RewardsTimestamp> { new RewardsTimestamp { Timestamp = string.Empty } };
                }

                NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                string currentTS = sharedPreference.StringForKey("SiteCoreRewardsTimeStamp");

                if (currentTS != null && currentTS.Equals(timeStamp.Data[0].Timestamp))
                {
                    needsUpdate = false;
                }
            });

            return needsUpdate;
        }

        public static Task GetLatestRewards()
        {
            return Task.Factory.StartNew(() =>
            {
                GetItemsService iService = new GetItemsService(TNBGlobal.OS
                    , DataManager.DataManager.SharedInstance.ImageSize
                    , TNBGlobal.SITECORE_URL
                    , TNBGlobal.APP_LANGUAGE);

                RewardsTimestampResponseModel timeStamp = iService.GetRewardsTimestampItem();

                if (timeStamp == null || timeStamp.Data == null || timeStamp.Data.Count == 0
                     || string.IsNullOrEmpty(timeStamp.Data[0].Timestamp)
                     || string.IsNullOrWhiteSpace(timeStamp.Data[0].Timestamp))
                {
                    timeStamp = new RewardsTimestampResponseModel();
                    timeStamp.Data = new List<RewardsTimestamp> { new RewardsTimestamp { Timestamp = string.Empty } };
                }

                RewardsEntity rewardsEntity = new RewardsEntity();
                rewardsEntity.DeleteTable();
                rewardsEntity.CreateTable();

                RewardsResponseModel rewardsResponse = iService.GetRewardsItems();
                if (rewardsResponse != null)
                {
                    RewardsCache.RewardIsAvailable = true;
                    if (rewardsResponse.Data != null && rewardsResponse.Data.Count > 0)
                    {
                        List<RewardsModel> rewardsData = new List<RewardsModel>();
                        List<RewardsCategoryModel> categoryList = new List<RewardsCategoryModel>(rewardsResponse.Data);
                        foreach (var category in categoryList)
                        {
                            List<RewardsModel> rewardsList = new List<RewardsModel>(category.Rewards);
                            if (rewardsList.Count > 0)
                            {
                                foreach (var reward in rewardsList)
                                {
                                    if (!RewardHasExpired(reward))
                                    {
                                        reward.CategoryID = category.ID;
                                        reward.CategoryName = category.CategoryName;
                                        rewardsData.Add(reward);
                                    }
                                }
                            }
                        }
                        rewardsEntity.InsertListOfItems(rewardsData);
                        try
                        {
                            NSUserDefaults sharedPreference = NSUserDefaults.StandardUserDefaults;
                            sharedPreference.SetString(timeStamp.Data[0].Timestamp, "SiteCoreRewardsTimeStamp");
                            sharedPreference.Synchronize();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error in sharedPreference: " + e.Message);
                        }
                    }
                }
                else
                {
                    RewardsCache.RewardIsAvailable = false;
                }
            });
        }
        #endregion
    }
}

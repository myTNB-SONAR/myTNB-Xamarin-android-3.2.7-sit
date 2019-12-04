﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;

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

        public static void UpdateRewardsCache()
        {
            List<RewardsModel> rewardsList = new RewardsEntity().GetAllItems();
            if (rewardsList != null && _userRewards != null && _userRewards.d != null && _userRewards.d.IsSuccess
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

        public static void UpdateRewardItem(RewardsModel reward)
        {
            RewardsEntity rewardsEntity = new RewardsEntity();
            rewardsEntity.UpdateEntity(reward);
        }

        #region Rewards Services
        public static async Task<GetUserRewardsResponseModel> GetUserRewards()
        {
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                // serviceManager.usrInf,
                usrInf = new
                {
                    eid = "khanwh2@gmail.com",
                    sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                    did = DataManager.DataManager.SharedInstance.UDID,
                    ft = "token",
                    lang = TNBGlobal.APP_LANGUAGE,
                    sec_auth_k1 = TNBGlobal.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                }
            };
            GetUserRewardsResponseModel response = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<GetUserRewardsResponseModel>("GetUserRewards", requestParameter);
            });
            _userRewards = response;
            UpdateRewardsCache();
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
                reward = new RewardsItemModel
                {
                    Email = "khanwh2@gmail.com",
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
                // serviceManager.usrInf,
                usrInf = new
                {
                    eid = "khanwh2@gmail.com",
                    sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                    did = DataManager.DataManager.SharedInstance.UDID,
                    ft = "token",
                    lang = TNBGlobal.APP_LANGUAGE,
                    sec_auth_k1 = TNBGlobal.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                },
                reward
            };
            UpdateRewardsResponseModel response = await Task.Run(() =>
            {
                return serviceManager.OnExecuteAPIV6<UpdateRewardsResponseModel>("AddUpdateRewards", requestParameter);
            });
            return response;
        }
        #endregion
    }
}
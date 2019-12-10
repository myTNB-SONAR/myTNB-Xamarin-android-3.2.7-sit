using System;
using System.Collections.Generic;
using Foundation;

namespace myTNB
{
    public sealed class RewardsCache
    {
        private static readonly Lazy<RewardsCache> lazy = new Lazy<RewardsCache>(() => new RewardsCache());
        public static RewardsCache Instance { get { return lazy.Value; } }
        private static Dictionary<string, NSData> ImageDictionary = new Dictionary<string, NSData>();

        private static GetUserRewardsResponseModel userRewardsResponse = new GetUserRewardsResponseModel();
        private static List<RewardsItemModel> rewardsList = new List<RewardsItemModel>();

        public static void AddGetUserRewardsResponseData(GetUserRewardsResponseModel resp)
        {
            if (userRewardsResponse == null)
            {
                userRewardsResponse = new GetUserRewardsResponseModel();
            }
            userRewardsResponse = resp;
            if (userRewardsResponse != null && userRewardsResponse.d != null &&
                userRewardsResponse.d.data != null && userRewardsResponse.d.IsSuccess)
            {
                rewardsList = userRewardsResponse.d.data.UserRewards;
            }
        }

        public static GetUserRewardsResponseModel GetUserRewardsResponse()
        {
            if (userRewardsResponse != null)
            {
                return userRewardsResponse;
            }
            return new GetUserRewardsResponseModel();
        }

        public static List<RewardsItemModel> GetUserRewardsList()
        {
            if (rewardsList != null)
            {
                return rewardsList;
            }
            return new List<RewardsItemModel>();
        }

        public static DateTime? GetRedeemedDate(string rewardId)
        {
            var reward = GetUserRewardsList().Find(x => x.RewardId == rewardId);
            if (reward != null)
            {
                if (reward.RedeemedDate != null)
                {
                    return reward.RedeemedDate.Value;
                }
            }
            return null;
        }

        public static void SaveImage(string key, NSData data)
        {
            if (ImageDictionary == null)
            {
                ImageDictionary = new Dictionary<string, NSData>();
            }
            if (ImageDictionary.ContainsKey(key))
            {
                ImageDictionary[key] = data;
            }
            else
            {
                ImageDictionary.Add(key, data);
            }
        }

        public static NSData GetImage(string key)
        {
            if (ImageDictionary.ContainsKey(key))
            {
                return ImageDictionary[key];
            }
            return null;
        }

        public static void Clear()
        {
            if (ImageDictionary != null)
            {
                ImageDictionary.Clear();
            }
        }
    }
}
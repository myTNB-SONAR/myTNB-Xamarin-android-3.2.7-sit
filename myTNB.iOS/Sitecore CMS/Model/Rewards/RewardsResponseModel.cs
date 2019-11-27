using System;
using System.Collections.Generic;

namespace myTNB.SitecoreCMS.Model
{
    public class RewardsResponseModel
    {
        public string Status { set; get; }
        public List<RewardsCategoryModel> Data { set; get; }
    }

    public class RewardsCategoryModel
    {
        public string ID { set; get; }
        public string CategoryName { set; get; }
        public List<RewardsModel> Rewards { set; get; }
    }

    public class RewardsModel
    {
        public string CategoryID { set; get; }
        public string CategoryName { set; get; }
        public string ID { set; get; }
        public string RewardName { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
    }

    public class RewardsTimestampResponseModel
    {
        public string Status { set; get; }
        public List<RewardsTimestamp> Data { set; get; }
    }

    public class RewardsTimestamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}

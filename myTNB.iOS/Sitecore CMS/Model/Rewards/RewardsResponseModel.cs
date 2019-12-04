using System;
using System.Collections.Generic;
using myTNB.SQLite.SQLiteDataManager;
using SQLite;

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
        [PrimaryKey]
        public string ID { set; get; }
        public string CategoryID { set; get; }
        public string CategoryName { set; get; }
        public string RewardName { set; get; }
        public string Title { set; get; }
        public string TitleOnListing { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string PeriodLabel { set; get; }
        public string LocationLabel { set; get; }
        public string TandCLabel { set; get; }
        public string StartDate { set; get; }
        public string EndDate { set; get; }
        public int RewardUseWithinTime { set; get; }
        public string RewardUseTitle { set; get; }
        public string RewardUseDescription { set; get; }
        public bool IsSaved { set; get; }
        public bool IsRead { set; get; }
        public bool IsUsed { set; get; }

        public RewardsEntity ToEntity()
        {
            var entity = new RewardsEntity
            {
                CategoryID = CategoryID,
                CategoryName = CategoryName,
                ID = ID,
                RewardName = RewardName,
                Title = Title,
                TitleOnListing = TitleOnListing,
                Description = Description,
                Image = Image,
                PeriodLabel = PeriodLabel,
                LocationLabel = LocationLabel,
                TandCLabel = TandCLabel,
                StartDate = StartDate,
                EndDate = EndDate,
                RewardUseWithinTime = RewardUseWithinTime,
                RewardUseTitle = RewardUseTitle,
                RewardUseDescription = RewardUseDescription,
                IsSaved = IsSaved,
                IsRead = IsRead,
                IsUsed = IsUsed
            };

            return entity;
        }
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

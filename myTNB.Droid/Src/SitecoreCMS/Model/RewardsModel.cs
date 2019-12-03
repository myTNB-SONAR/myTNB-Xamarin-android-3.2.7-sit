using System.Collections.Generic;
using Android.Graphics;

namespace myTNB.SitecoreCMS.Model
{
    public class RewardsTimeStampResponseModel
    {
        public string Status { set; get; }
        public List<RewardsTimeStamp> Data { set; get; }
    }

    public class RewardsResponseModel
    {
        public string Status { set; get; }
        public List<RewardsCategoryModel> Data { set; get; }
    }

    public class RewardsCategoryModel
    {
        public string ID { set; get; }
        public string CategoryName { set; get; }
        public List<RewardsModel> RewardList { set; get; }
    }

    public class RewardsModel
    {
        public string CategoryID { set; get; }
        public string CategoryName { set; get; }
        public string ID { set; get; }
        public string RewardName { set; get; }
        public string Title { set; get; }
        public string TitleOnListing { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ImageB64 { set; get; }
        public Bitmap ImageBitmap { set; get; }
        public string PeriodLabel { set; get; }
        public string LocationLabel { set; get; }
        public string TandCLabel { set; get; }
        public string StartDate { set; get; }
        public string EndDate { set; get; }
        public bool IsSaved { set; get; }
        public string IsSavedDateTime { set; get; }
        public bool Read { set; get; }
        public bool IsUsed { set; get; }
        public string IsUsedDateTime { set; get; }

    }

    public class RewardsTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
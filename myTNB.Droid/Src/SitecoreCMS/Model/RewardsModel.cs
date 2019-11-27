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
        public List<RewardsModel> Data { set; get; }
    }

    public class RewardsCategoryModel
    {
        public string Title { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
        public List<RewardsModel> RewardList { set; get; }
        public string ID { set; get; }
    }

    public class RewardsModel
    {
        public string Title { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }
        public string ImageB64 { set; get; }
        public string CategoryID { set; get; }
        public Bitmap ImageBitmap { set; get; }
        public bool Read { set; get; }
        public bool IsUsed { set; get; }

        public string ID { set; get; }
    }

    public class RewardsTimeStamp
    {
        public string Timestamp { set; get; }
        public string ID { set; get; }
    }
}
using System;
namespace myTNB
{
    public static class RewardsConstants
    {
        //PageName
        public static string PageName_Rewards = "Rewards";
        public static string PageName_RewardDetails = "RewardDetails";

        //Tags
        public static int Tag_SelectedCategory = 100;
        public static int Tag_CategoryLabel = 101;

        //nfloat
        public static nfloat RewardsCellHeight = ScaleUtility.GetScaledHeight(177F);

        //Cell
        public static string Cell_Rewards = "rewardsTableViewCell";

        //I18N Keys
        public static string I18N_Title = "title";
        public static string I18N_RewardPeriod = "rewardPeriod";
        public static string I18N_Location = "location";
        public static string I18N_TNC = "tnc";
        public static string I18N_Save = "save";
        public static string I18N_Unsave = "unsave";
        public static string I18N_UseNow = "useNow";

        //Img
        public static string Img_HeartIcon = "Heart-Icon";
        public static string Img_ShareIcon = "IC-Header-Share";
        public static string Img_HeartUnsavedGreenIcon = "Heart-Unsaved-Green-Icon";
        public static string Img_HeartSavedGreenIcon = "Heart-Saved-Green-Icon";
        public static string Img_HeartSaveIcon = "Heart-Save-Icon";
        public static string Img_HeartUnsaveIcon = "Heart-Unsave-Icon";
        public static string Img_RewardPeriodIcon = "Reward-Period-Icon";
        public static string Img_RewardLocationIconn = "Reward-Location-Icon";
        public static string Img_RewardTCIcon = "Reward-TC-Icon";
        public static string Img_RewardDefaultBanner = "Reward_Default-Banner";
    }
}
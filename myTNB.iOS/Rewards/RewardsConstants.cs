using System;
using System.Collections.Generic;

namespace myTNB
{
    public static class RewardsConstants
    {
        //PageName
        public static string PageName_Rewards = "Rewards";
        public static string PageName_RewardDetails = "RewardDetails";
        public static string PageName_SavedRewards = "SavedRewards";

        //Tags
        public static int Tag_SelectedCategory = 100;
        public static int Tag_CategoryLabel = 101;
        public static int Tag_ViewContainer = 1000;
        public static int Tag_TableView = 2000;
        public static int Tag_DetailRewardImage = 3000;
        public static int Tag_DetailRewardTitle = 3001;
        public static int Tag_DetailRewardView = 3002;
        public static int Tag_DetailRewardPeriodImage = 3003;
        public static int Tag_DetailLocationImage = 3004;
        public static int Tag_DetailTCImage = 3005;

        //Format
        public static string Format_Date = "dd MMM yyyy, h:mm tt.";

        //nfloat
        public static nfloat RewardsCellHeight = ScaleUtility.GetScaledHeight(177F);

        //Preference Key
        public static string Pref_RewardsTutorialOverlay = "RewardsTutorialOverlayV2.1.0";
        public static string Pref_RewardsDetailTutorialOverlay = "RewardsDetailTutorialOverlayV2.1.0";

        //Cell
        public static string Cell_Rewards = "rewardsTableViewCell";
        public static string Cell_SavedRewards = "savedRewardsTableViewCell";

        //Event Names
        public static string EVENT_Refresh = "Refresh";

        //I18N Keys
        public static string I18N_Title = "title";
        public static string I18N_ViewAll = "viewAll";
        public static string I18N_RewardPeriod = "rewardPeriod";
        public static string I18N_Location = "location";
        public static string I18N_TNC = "tnc";
        public static string I18N_Save = "save";
        public static string I18N_Unsave = "unsave";
        public static string I18N_UseNow = "useNow";
        public static string I18N_UseNowPopupTitle = "useNowPopupTitle";
        public static string I18N_UseNowPopupMessage = "useNowPopupMessage";
        public static string I18N_UseLater = "useLater";
        public static string I18N_Confirm = "confirm";
        public static string I18N_NoRewards = "noRewards";
        public static string I18N_RewardUsedPrefix = "rewardUsedPrefix";
        public static string I18N_RewardUsed = "rewardUsed";
        public static string I18N_RedeemRewardNote = "redeemRewardNote";
        public static string I18N_Used = "used";
        public static string I18N_EmptySavedReward = "emptyDesc";
        public static string I18N_TutorialRewardTitle = "tutorialRewardTitle";
        public static string I18N_TutorialRewardDesc = "tutorialRewardDesc";
        public static string I18N_TutorialCategoryTitle = "tutorialCategoryTitle";
        public static string I18N_TutorialCategoryDesc = "tutorialCategoryDesc";
        public static string I18N_TutorialSaveTitle = "tutorialSaveTitle";
        public static string I18N_TutorialSaveDesc = "tutorialSaveDesc";
        public static string I18N_TutorialUseNowTitle = "tutorialUseNowTitle";
        public static string I18N_TutorialUseNowDesc = "tutorialUseNowDesc";

        //Img
        public static string Img_HeartIcon = "Heart-Icon";
        public static string Img_ShareIcon = "IC-Header-Share";

        public static string Img_HeartUnsavedGreenIcon = "Heart-Unsaved-Green-Icon";
        public static string Img_HeartSavedGreenIcon = "Heart-Saved-Green-Icon";

        public static string Img_HeartSaveIcon = "Heart-Save-Icon";
        public static string Img_HeartUnsaveIcon = "Heart-Unsave-Icon";
        public static string Img_RewardPeriodIcon = "Reward-Period-Icon";
        public static string Img_RewardLocationIcon = "Reward-Location-Icon";
        public static string Img_RewardTCIcon = "Reward-TC-Icon";
        public static string Img_RewardPeriodIconUsed = "Reward-Period-Icon-Used";
        public static string Img_RewardLocationIconUsed = "Reward-Location-Icon-Used";
        public static string Img_RewardTCIconUsed = "Reward-TC-Icon-Used";
        public static string Img_RewardDefaultBanner = "Reward_Default-Banner";
        public static string Img_UseRewardBanner = "Use-Reward-Banner";
        public static string Img_EmptyRewardIcon = "Empty-Reward-Icon";
        public static string Img_EmptySavedRewardIcon = "Empty-Saved-Reward-Icon";
        public static string IMG_Refresh = "SSMR-Refresh";
    }
}
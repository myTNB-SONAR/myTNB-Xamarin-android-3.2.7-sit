using System.Collections.Generic;

namespace myTNB.Android.Src.Utils
{
    public class RewardsMenuUtils
    {
        private static bool isUpdateNeeded = false;
        private static string UpdateExclude = "";
        private static List<string> updatedList = new List<string>();
        private static bool refreshAll = false;
        private static bool isRewardLoading = false;
        private static bool touchDisable = false;

        public static void OnSetUpdateList(string mUpdateExclude)
        {
            UpdateExclude = mUpdateExclude;
            isUpdateNeeded = true;
            updatedList = new List<string>();
        }

        public static bool OnCheckIsUpdateNeed(string mPageQueryString)
        {
            bool isNeedRefresh = false;
            if (isUpdateNeeded && UpdateExclude != mPageQueryString && !updatedList.Contains(mPageQueryString))
            {
                updatedList.Add(mPageQueryString);
                isNeedRefresh = true;
            }
            return isNeedRefresh;
        }

        public static void OnResetUpdateList()
        {
            isUpdateNeeded = false;
            UpdateExclude = "";
            updatedList = new List<string>();
            refreshAll = false; 
        }

        public static void OnSetRefreshAll(bool flag)
        {
            refreshAll = flag;
        }

        public static bool GetRefreshAll()
        {
            return refreshAll;
        }

        public static void OnSetRewardLoading(bool flag)
        {
            isRewardLoading = flag;
        }

        public static bool GetRewardLoading()
        {
            return isRewardLoading;
        }

        public static void OnSetTouchDisable(bool flag)
        {
            touchDisable = flag;
        }

        public static bool GetTouchDisable()
        {
            return touchDisable;
        }

    }
}
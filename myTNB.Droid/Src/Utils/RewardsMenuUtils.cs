using System.Collections.Generic;

namespace myTNB_Android.Src.Utils
{
    public class RewardsMenuUtils
    {
        private static bool isUpdateNeeded = false;
        private static string UpdateExclude = "";
        private static List<string> updatedList = new List<string>();

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
        }

    }
}
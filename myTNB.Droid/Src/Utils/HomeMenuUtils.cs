using System;
using System.Threading;

namespace myTNB.AndroidApp.Src.Utils
{
    public class HomeMenuUtils
    {
        private static bool isLoadedHomeMenu = false;
        private static bool isMyServiceExpanded = false;
        private static int trackCurrentLoadMoreCount = 0;
        private static bool isQuery = false;
        private static string queryString = "";
        private static bool isRestartHomeMenu = false;
        private static bool isShowRearrangeAccountSuccessfulNeed = false;

        public static void ResetAll()
        {
            isLoadedHomeMenu = false;
            isMyServiceExpanded = false;
            trackCurrentLoadMoreCount = 0;
            isQuery = false;
            isRestartHomeMenu = false;
            queryString = "";
        }

        public static void ResetMyService()
        {
            isLoadedHomeMenu = false;
            isMyServiceExpanded = false;
            isRestartHomeMenu = false;
        }

        public static void SetIsShowRearrangeAccountSuccessfulNeed(bool flag)
        {
            isShowRearrangeAccountSuccessfulNeed = flag;
        }

        public static bool GetIsShowRearrangeAccountSuccessfulNeed()
        {
            return isShowRearrangeAccountSuccessfulNeed;
        }

        public static void SetIsLoadedHomeMenu(bool flag)
        {
            isLoadedHomeMenu = flag;
        }

        public static bool GetIsLoadedHomeMenu()
        {
            return isLoadedHomeMenu;
        }

        public static void SetIsRestartHomeMenu(bool flag)
        {
            isRestartHomeMenu = flag;
        }

        public static bool GetIsRestartHomeMenu()
        {
            return isRestartHomeMenu;
        }

        public static void SetTrackCurrentLoadMoreCount(int count)
        {
            trackCurrentLoadMoreCount = count;
        }

        public static int GetTrackCurrentLoadMoreCount()
        {
            return trackCurrentLoadMoreCount;
        }

        public static void SetIsMyServiceExpanded(bool flag)
        {
            isMyServiceExpanded = flag;
        }

        public static bool GetIsMyServiceExpanded()
        {
            return isMyServiceExpanded;
        }

        public static void SetIsQuery(bool flag)
        {
            isQuery = flag;
        }

        public static bool GetIsQuery()
        {
            return isQuery;
        }

        public static void SetQueryWord(string word)
        {
            queryString = word;
        }

        public static string GetQueryWord()
        {
            return queryString;
        }
    }
}

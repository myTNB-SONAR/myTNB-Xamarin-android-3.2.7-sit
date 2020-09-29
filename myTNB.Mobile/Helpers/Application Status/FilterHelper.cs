using System;
using System.Collections.Generic;

namespace myTNB.Mobile.Helpers
{
    public static class FilterHelper
    {
        private const string MIN_YEAR = "2016";

        public static List<string> GetFromYearList(string minimumYear)
        {
            List<string> yrList = GetYearList(minimumYear ?? MIN_YEAR
               , DateTime.Now.Year.ToString());
            return yrList;
        }

        public static List<string> GetToYearList(string minimumYear)
        {
            List<string> yrList = GetYearList(minimumYear ?? MIN_YEAR
                , DateTime.Now.Year.ToString());
            return yrList;
        }

        public static List<string> GetYearList(string minimumYear, string maximumYear)
        {
            List<string> yrList = new List<string>();
            int currentYear = int.Parse(maximumYear);
            int minYear = int.Parse(minimumYear ?? MIN_YEAR);

            for (int i = minYear; i <= currentYear; i++)
            {
                yrList.Add(i.ToString());
            }
            return yrList;
        }

    }
}

using System;
using System.Collections.Generic;

namespace myTNB.Mobile.Helpers
{
    public static class FilterHelper
    {
        /// <summary>
        /// Default year. Fallback
        /// </summary>
        private const string MIN_YEAR = "2016";
        private const string DATE_FORMAT = "{0}/{1}/{2}";

        /// <summary>
        /// Returns the From Year List
        /// </summary>
        /// <param name="minimumYear"></param>
        /// <returns></returns>
        public static List<string> GetFromYearList(string minimumYear)
        {
            List<string> yrList = GetYearList(minimumYear ?? MIN_YEAR
               , DateTime.Now.Year.ToString());
            return yrList;
        }

        /// <summary>
        /// Returns the To Year List
        /// </summary>
        /// <param name="minimumYear"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns Month Selector from language file
        /// </summary>
        /// <returns></returns>
        public static List<SelectorModel> GetMonthList()
        {
            Dictionary<string, List<SelectorModel>> selector = LanguageManager.Instance.GetSelectorsByPage("SelectCreationDate");
            List<SelectorModel> monthSelector = selector.ContainsKey("months") ? selector["months"] : null;
            return monthSelector;
        }

        /// <summary>
        /// Returns formatted date for request.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="isFromDate"></param>
        /// <returns></returns>
        public static string FormatCreatedDate(int year, int month, bool isFromDate)
        {
            if (isFromDate)
            {
                return FormatDate(year, month, 1);
            }
            else
            {
                int day = DateTime.DaysInMonth(year, month);
                return FormatDate(year, month, day);
            }
        }

        private static string FormatDate(int year, int month, int day)
        {
            string date = string.Format(DATE_FORMAT, year.ToString(), month.ToString(), day.ToString());
            return date;
        }
    }

    public class DateFilterModel
    {
        public int Month { set; get; } = -1;
        public int YearIndex { set; get; } = -1;
        public int Year { set; get; }
    }
}
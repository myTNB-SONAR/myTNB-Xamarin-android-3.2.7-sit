using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Charts;
using Android.Util;

namespace myTNB_Android.Src.Utils.Custom.Charts
{
    public class DayAxisValueFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        protected String[] mMonths = new String[]{
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        private BarLineChartBase chart;

        public DayAxisValueFormatter(BarLineChartBase chart)
        {
            this.chart = chart;
        }


        public string GetFormattedValue(float value, AxisBase axis)
        {

            Log.Debug("DayAxisValueFormatter" , string.Format("Value is {0}" , value));


            int monthIndex = Math.Abs((int)value);

            //int year = determineYear(days);

            //int month = determineMonth(days);
            //String monthName = mMonths[month % mMonths.Length];
            //String yearName = year.ToString();

            //if (chart.VisibleXRange > 30 * 6)
            //{

            //    return monthName + " " + yearName;
            //}
            //else
            //{

            //    int dayOfMonth = determineDayOfMonth(days, month + 12 * (year - 2016));

            //    String appendix = "th";

            //    switch (dayOfMonth)
            //    {
            //        case 1:
            //            appendix = "st";
            //            break;
            //        case 2:
            //            appendix = "nd";
            //            break;
            //        case 3:
            //            appendix = "rd";
            //            break;
            //        case 21:
            //            appendix = "st";
            //            break;
            //        case 22:
            //            appendix = "nd";
            //            break;
            //        case 23:
            //            appendix = "rd";
            //            break;
            //        case 31:
            //            appendix = "st";
            //            break;
            //    }

            //    return dayOfMonth == 0 ? "" : dayOfMonth + appendix + " " + monthName;

            return mMonths[monthIndex];
            //}
        }


        private int getDaysForMonth(int month, int year)
        {

            // month is 0-based

            if (month == 1)
            {
                bool is29Feb = false;

                if (year < 1582)
                    is29Feb = (year < 1 ? year + 1 : year) % 4 == 0;
                else if (year > 1582)
                    is29Feb = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

                return is29Feb ? 29 : 28;
            }

            if (month == 3 || month == 5 || month == 8 || month == 10)
                return 30;
            else
                return 31;
        }

        private int determineMonth(int dayOfYear)
        {

            int month = -1;
            int days = 0;

            while (days < dayOfYear)
            {
                month = month + 1;

                if (month >= 12)
                    month = 0;

                int year = determineYear(days);
                days += getDaysForMonth(month, year);
            }

            return Math.Max(month, 0);
        }

        private int determineDayOfMonth(int days, int month)
        {

            int count = 0;
            int daysForMonths = 0;

            while (count < month)
            {

                int year = determineYear(daysForMonths);
                daysForMonths += getDaysForMonth(count % 12, year);
                count++;
            }

            return days - daysForMonths;
        }

        private int determineYear(int days)
        {

            if (days <= 366)
                return 2016;
            else if (days <= 730)
                return 2017;
            else if (days <= 1094)
                return 2018;
            else if (days <= 1458)
                return 2019;
            else
                return 2020;

        }
    }
}
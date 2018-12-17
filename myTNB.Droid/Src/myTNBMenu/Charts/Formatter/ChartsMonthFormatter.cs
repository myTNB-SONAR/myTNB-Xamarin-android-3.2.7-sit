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
using myTNB_Android.Src.myTNBMenu.Models;
using MikePhil.Charting.Charts;

namespace myTNB_Android.Src.myTNBMenu.Charts.Formatter
{
    public class ChartsMonthFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        private UsageHistoryData.ByMonthData byMonthData;
        private BarLineChartBase chart;

        public ChartsMonthFormatter(UsageHistoryData.ByMonthData byMonthData, BarLineChartBase chart)
        {
            this.byMonthData = byMonthData;
            this.chart = chart;
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            int invertedIndex = (int)value ; 

            return byMonthData.Months[Math.Abs(invertedIndex)].Month;
        }
    }
}
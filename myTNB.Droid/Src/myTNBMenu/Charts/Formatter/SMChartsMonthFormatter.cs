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
    public class SMChartsMonthFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        private List<SMUsageHistoryData.ByMonthData> byMonthData;
        private BarChart chart;
        private int index;

        public SMChartsMonthFormatter(List<SMUsageHistoryData.ByMonthData> byMonthData, BarChart chart, int i)
        {
            this.byMonthData = byMonthData;
            this.chart = chart;
            this.index = i;
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            int invertedIndex = (int)value ; 

            return byMonthData[index].Months[Math.Abs(invertedIndex)].Month;
        }
    }
}
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.myTNBMenu.Charts.Formatter
{
    public class SMChartsMonthFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        private SMUsageHistoryData.ByMonthData byMonthData;
        private BarChart chart;

        public SMChartsMonthFormatter(SMUsageHistoryData.ByMonthData byMonthData, BarChart chart)
        {
            this.byMonthData = byMonthData;
            this.chart = chart;
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            int invertedIndex = (int)value;
            if (invertedIndex >= byMonthData.Months.Count)
            {
                invertedIndex = byMonthData.Months.Count - 1;
            }

            if (invertedIndex == -1)
            {
                return "";
            }
            else
            {
                return byMonthData.Months[Math.Abs(invertedIndex)].Month;
            }
        }
    }
}
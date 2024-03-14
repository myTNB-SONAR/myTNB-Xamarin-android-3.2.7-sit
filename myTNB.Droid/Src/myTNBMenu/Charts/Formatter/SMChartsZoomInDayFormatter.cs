using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.Android.Src.myTNBMenu.Models;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.myTNBMenu.Charts.Formatter
{
    public class SMChartsZoomInDayFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        private List<string> byDayData;
        private BarChart chart;

        public SMChartsZoomInDayFormatter(List<string> byDayData, BarChart chart)
        {
            this.byDayData = byDayData;
            this.chart = chart;
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            int invertedIndex = (int)value;

            if (invertedIndex >= byDayData.Count)
            {
                invertedIndex = byDayData.Count - 1;
            }

            if (invertedIndex == -1)
            {
                return "";
            }
            else
            {
                return byDayData[Math.Abs(invertedIndex)];
            }
        }
    }
}
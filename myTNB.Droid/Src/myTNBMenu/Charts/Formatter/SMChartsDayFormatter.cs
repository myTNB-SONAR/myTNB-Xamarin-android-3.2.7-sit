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
    public class SMChartsDayFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        internal List<SMUsageHistoryData.ByDayData> DayData { get; set; }
        internal BarChart Chart { get; set; }
        internal int ParentIndex { get; set; }
        internal int Index { get; set; }

        public SMChartsDayFormatter(List<SMUsageHistoryData.ByDayData> byDayData, BarChart chart, int parentIndex, int index)
        {
            this.DayData = byDayData;
            this.Chart = chart;
            this.ParentIndex = parentIndex;
            this.Index = index;
        }


        public string GetFormattedValue(float value, AxisBase axis)
        {
            int index = (int)value;
            string day = DayData[ParentIndex].Days[index].Day;
            return day;
        }
    }
}
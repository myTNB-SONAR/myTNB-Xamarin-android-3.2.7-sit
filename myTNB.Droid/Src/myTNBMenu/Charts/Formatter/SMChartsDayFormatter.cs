using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.Android.Src.myTNBMenu.Models;
using System.Collections.Generic;

namespace myTNB.Android.Src.myTNBMenu.Charts.Formatter
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
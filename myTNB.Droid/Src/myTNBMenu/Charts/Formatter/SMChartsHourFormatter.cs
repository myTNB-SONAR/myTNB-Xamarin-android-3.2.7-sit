using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.Android.Src.myTNBMenu.Models;
using System.Collections.Generic;

namespace myTNB.Android.Src.myTNBMenu.Charts.Formatter
{
    public class SMChartsHourFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        internal List<SMUsageHistoryData.ByDayData> DayData { get; set; }
        internal BarChart Chart { get; set; }
        internal int ParentIndex { get; set; }



        public string GetFormattedValue(float value, AxisBase axis)
        {
            int index = (int)value;
            string month = DayData[ParentIndex].Days[index].Month;
            string day = DayData[ParentIndex].Days[index].Day;
            return month + "" + day;
        }
    }
}
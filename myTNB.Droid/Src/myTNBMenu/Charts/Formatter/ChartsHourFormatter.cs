using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.myTNBMenu.Charts.Formatter
{
    public class ChartsHourFormatter : Java.Lang.Object, IAxisValueFormatter
    {

        internal List<UsageHistoryData.ByDayData> DayData { get; set; }
        internal BarLineChartBase Chart { get; set; }
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
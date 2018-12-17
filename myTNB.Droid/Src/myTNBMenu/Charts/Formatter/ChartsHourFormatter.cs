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
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using myTNB.Android.Src.myTNBMenu.Models;
using System;

namespace myTNB.Android.Src.myTNBMenu.Charts.Formatter
{
	public class ChartsKWhFormatter : Java.Lang.Object, IAxisValueFormatter
	{

		private UsageHistoryData.ByMonthData byMonthData;
		private BarLineChartBase chart;

		public ChartsKWhFormatter(UsageHistoryData.ByMonthData byMonthData, BarLineChartBase chart)
		{
			this.byMonthData = byMonthData;
			this.chart = chart;
		}

		public string GetFormattedValue(float value, AxisBase axis)
		{
			int invertedIndex = (int)value;

			return byMonthData.Months[Math.Abs(invertedIndex)].Month;
		}
	}
}
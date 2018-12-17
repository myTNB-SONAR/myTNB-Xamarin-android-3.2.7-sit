using System;
using System.Collections.Generic;
using System.Linq;
using myTNB.Model;

namespace myTNB
{
    public static class ChartHelper
    {
        /// <summary>
        /// Gets the smart data chart count.
        /// </summary>
        /// <returns>The smart data chart count.</returns>
        public static int GetSmartDataChartCount(ChartDataModelBase currentChart)
        {
            int count = currentChart?.ByDay?.Count ?? 0;
            if (DataManager.DataManager.SharedInstance.IsMontView)
            {
                var chart = currentChart as SmartChartDataModel;
                count = chart?.ByMonth?.Count ?? 0;
            }
            return count;
        }

        /// <summary>
        /// Sorts the chart data descending.
        /// </summary>
        /// <param name="chartModelBase">Chart model base.</param>
        /// <param name="updatedChart">Updated chart.</param>
        public static void SortChartDataDescending(ChartDataModelBase chartModelBase, 
                                          out ChartDataModelBase updatedChart)
        {
            bool isNormalMeter = (chartModelBase is ChartDataModel) ? true : false;
            List<SegmentDetailsModel> chartData = null;

            if (isNormalMeter)
            {
                ChartDataModel model = chartModelBase as ChartDataModel;
                ChartDataModel temp = model;
                temp.ByDay = model?.ByDay?.OrderByDescending(item => item.Index).ToList();
                updatedChart = temp;
            }
            else
            {
                SmartChartDataModel model = chartModelBase as SmartChartDataModel;
                SmartChartDataModel temp = model;
                temp.ByDay = model?.ByDay?.OrderByDescending(item => item.Index).ToList();
                temp.ByMonth = model?.ByMonth?.OrderByDescending(item => item.Index).ToList();
                updatedChart = temp;
            }

            if (chartData == null)
            {
                chartData = new List<SegmentDetailsModel>();
            }
        }

        /// <summary>
        /// Gets the selected chart info.
        /// </summary>
        /// <param name="chartModelBase">Chart model base.</param>
        /// <param name="isMonthView">If set to <c>true</c> is month view.</param>
        /// <param name="chartIndex">Chart index.</param>
        /// <param name="smartMeterMetric">Smart meter metric.</param>
        /// <param name="chartData">Chart data.</param>
        /// <param name="dateRange">Date range.</param>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public static bool GetSelectedChartInfo(ChartDataModelBase chartModelBase, bool isMonthView, int chartIndex, out UsageMetrics smartMeterMetric,
                                          out List<SegmentDetailsModel> chartData, out string dateRange, out bool isNormalMeter)
        {
            isNormalMeter = (chartModelBase is ChartDataModel) ? true : false;
            chartData = null;
            dateRange = "-";
            smartMeterMetric = null;

            if (isNormalMeter)
            {
                ChartDataModel model = chartModelBase as ChartDataModel;
                if (isMonthView)
                {
                    chartData = model?.ByMonth?.Months;
                    dateRange = model?.ByMonth?.Range;

                }
                else if (model.ByDay?.Count > 0 && chartIndex < model.ByDay?.Count)
                {
                    chartData = model?.ByDay[chartIndex]?.Days;
                    dateRange = model?.ByDay[chartIndex]?.Range;
                }

            }
            else
            {
                SmartChartDataModel model = chartModelBase as SmartChartDataModel;

                if (isMonthView)
                {
                    if (model?.ByMonth?.Count > 0 && chartIndex < model?.ByMonth?.Count)
                    {
                        chartData = model?.ByMonth[chartIndex]?.Months;
                        dateRange = model?.ByMonth[chartIndex]?.Range;
                    }

                }
                else if (model?.ByDay?.Count > 0 && DataManager.DataManager.SharedInstance.CurrentChartIndex < model?.ByDay?.Count)
                {
                    chartData = model?.ByDay[chartIndex]?.Days;
                    dateRange = model?.ByDay[chartIndex]?.Range;
                }

                smartMeterMetric = model?.OtherUsageMetrics;

            }

            if (chartData == null)
            {
                chartData = new List<SegmentDetailsModel>();
                CreateDefaultChartData(isMonthView, out chartData, out dateRange);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the default chart data.
        /// </summary>
        /// <param name="isMonthView">If set to <c>true</c> is month view.</param>
        /// <param name="chartData">Chart data.</param>
        /// <param name="dateRange">Date range.</param>
        public static void CreateDefaultChartData(bool isMonthView, 
                                          out List<SegmentDetailsModel> chartData, out string dateRange)
        {
            chartData = new List<SegmentDetailsModel>();
            var currDate = DateTime.Now;
            if (isMonthView)
            {
                int itemCount = 5;
                dateRange = string.Format("{0} - {1}", currDate.AddMonths(itemCount * -1).ToString("MMM yyyy"), currDate.ToString("MMM yyyy"));

                for (int i = itemCount; i >= 0; i--)
                {
                    var segment = new SegmentDetailsModel();
                    segment.Month = currDate.AddMonths(i * -1).ToString("MMM");
                    chartData.Add(segment);
                }
            }
            else
            {
                int itemCount = 6;
                dateRange = string.Format("{0} - {1}", currDate.AddDays(itemCount * -1).ToString("dd MMM"), currDate.ToString("dd MMM"));

                for (int i = itemCount; i >= 0; i--)
                {
                    var segment = new SegmentDetailsModel();
                    var item = currDate.AddDays(i * -1);
                    segment.Month = item.ToString("MMM");
                    segment.Day = item.ToString("dd");
                    chartData.Add(segment);
                }
            }
        }

        /// <summary>
        /// Removes the excess smart month data.
        /// </summary>
        /// <param name="model">Model.</param>
        public static void RemoveExcessSmartMonthData(ref SmartChartDataModel model)
        {
            if (model?.ByMonth?.Count > 1)
            {
                model.ByMonth.RemoveRange(1, model.ByMonth.Count - 1);
            }
        }

        /// <summary>
        /// Updates the chart value for renewable account.
        /// </summary>
        /// <returns>The chart value for re.</returns>
        /// <param name="chartValue">Chart value.</param>
        public static double UpdateValueForRE(double chartValue)
        {
            if (chartValue < 0)
            {
                return chartValue * -1.0;
            }

            return 0;
        }


    }
}

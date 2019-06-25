using System;
using System.Collections.Generic;
using Carousels;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class ChartDataSource : iCarouselDataSource
    {
        readonly UIView _parentView;
        ChartDataModelBase chartBaseData;
        bool isRenewableAcct = false;

        public ChartDataSource(UIView view, ChartDataModelBase chart, bool isREAcct)
        {
            _parentView = view;
            isRenewableAcct = isREAcct;
            ChartHelper.SortChartDataDescending(chart, out chartBaseData);
        }

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        /// <returns>The number of items.</returns>
        /// <param name="carousel">Carousel.</param>
        public override nint GetNumberOfItems(iCarousel carousel)
        {
            // return the number of items in the data
            if (chartBaseData != null)
            {
                bool isNormalMeter = (chartBaseData is ChartDataModel) ? true : false;
                if (isNormalMeter)
                {
                    if (DataManager.DataManager.SharedInstance.IsMontView)
                    {
                        var model = chartBaseData as ChartDataModel;
                        if (model.ByMonth != null)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return chartBaseData?.ByDay?.Count ?? 1;
                    }
                }
                else
                {
                    return ChartHelper.GetSmartDataChartCount(chartBaseData);
                }

            }
            return 1;
        }

        /// <summary>
        /// Gets the view for item.
        /// </summary>
        /// <returns>The view for item.</returns>
        /// <param name="carousel">Carousel.</param>
        /// <param name="index">Index.</param>
        /// <param name="view">View.</param>
        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
        {
            List<SegmentDetailsModel> chartData = null;
            string dateRange = string.Empty;
            bool isNormalMeter = true;
            UsageMetrics smartMeterMetrics = null;
            int chartIndex = (int)index;

            ChartHelper.GetSelectedChartInfo(chartBaseData, DataManager.DataManager.SharedInstance.IsMontView
                , chartIndex, out smartMeterMetrics, out chartData, out dateRange, out isNormalMeter);

            var chartComponent = new ChartComponent(_parentView);
            var viewChart = chartComponent.GetUI(isNormalMeter);

            //if (chartData?.Count > 0)
            {
                chartComponent?.SetFrameByMeterType(isNormalMeter);
                chartComponent.ConstructSegmentViews(chartData, isNormalMeter
                    , DataManager.DataManager.SharedInstance.CurrentChartMode, isRenewableAcct);

                var rangeLabel = new UILabel()
                {
                    Text = dateRange,
                    Hidden = true,
                    Tag = TNBGlobal.Tags.RangeLabel
                };
                viewChart.AddSubview(rangeLabel);
            }
            return viewChart;
        }
    }
}
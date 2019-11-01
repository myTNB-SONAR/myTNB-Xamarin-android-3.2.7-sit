using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CoreGraphics;
using myTNB.Enums;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class ChartComponent
    {
        UIView _parentView;
        UIView _viewChart;
        private bool _hasLineChart = false;

        public ChartComponent(UIView view)
        {
            _parentView = view;
        }
        internal void CreateComponent(bool isNormalMeter)
        {
            var isAccountActive = DataManager.DataManager.SharedInstance.AccountIsActive;
            var isAccountSSMR = DataManager.DataManager.SharedInstance.AccountIsSSMR;
            nfloat adjustHeightSSMR = isAccountSSMR ? 0.8f : 1.0f;
            if (!DataManager.DataManager.SharedInstance.IsSmartMeterAvailable && !isNormalMeter)
            {
                isNormalMeter = true;
            }
            int yLocation = GetChartYLocation(isNormalMeter);

            nfloat viewPercentage = isAccountActive ? 0.33f : 0.30f;

            if (DeviceHelper.IsIphoneXUpResolution())
            {
                if (DeviceHelper.IsIphoneXOrXs())
                {
                    viewPercentage = 0.42f;
                }
                else
                {
                    viewPercentage = 0.45f;
                }
            }
            else if (DeviceHelper.IsIphone6UpResolution())
            {
                viewPercentage = isAccountActive ? 0.40f : 0.35f;
            }
            if (isAccountSSMR)
            {
                if (DeviceHelper.IsIphoneXUpResolution())
                {
                    if (DeviceHelper.IsIphoneXOrXs())
                    {
                        viewPercentage = viewPercentage * adjustHeightSSMR * 0.9f;
                    }
                    else
                    {
                        viewPercentage = viewPercentage * adjustHeightSSMR;
                    }
                }
                else if (DeviceHelper.IsIphone6UpResolution())
                {
                    if (DeviceHelper.IsIphone678())
                    {
                        viewPercentage = viewPercentage * adjustHeightSSMR * 0.8f;
                    }
                    else
                    {
                        viewPercentage = viewPercentage * adjustHeightSSMR * 0.9f;
                    }
                }
                else
                {
                    viewPercentage = viewPercentage * adjustHeightSSMR;
                }
            }
            nfloat viewHeight = _parentView.Frame.Height * viewPercentage;
            _viewChart = new UIView(new CGRect(42, yLocation, _parentView.Frame.Width - 84, viewHeight));
            _viewChart.BackgroundColor = UIColor.Clear;
        }

        internal void RemoveChartViewSubViews()
        {
            foreach (UIView item in _viewChart)
            {
                item.RemoveFromSuperview();
            }
        }

        /// <summary>
        /// Constructs the segment views.
        /// </summary>
        /// <param name="chartData">Chart data.</param>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void ConstructSegmentViews(List<SegmentDetailsModel> chartData, bool isNormalMeter = true
            , ChartModeEnum chartMode = ChartModeEnum.Cost, bool isREAccount = false)
        {
            var isAccountSSMR = DataManager.DataManager.SharedInstance.AccountIsSSMR;
            nfloat adjustHeightSSMR = 1f;
            if (isAccountSSMR)
            {
                if (DeviceHelper.IsIphoneXOrXs())
                {
                    adjustHeightSSMR = 0.8f;
                }
                else
                {
                    adjustHeightSSMR = 0.9f;
                }
            }

            RemoveChartViewSubViews();
            double chartContainerWidth = (double)_viewChart.Frame.Width;
            double chartContainerHeight = (double)_viewChart.Frame.Height;

            int barTopMargin = DeviceHelper.IsIphoneXUpResolution() ? 50 : 30;
            int barBottomMargin = 18;
            if (_parentView.Frame.Width == 320)
            {
                barTopMargin = 22;
                barBottomMargin = 10;
            }

            double maxValue = chartData.Any() ? GetMaxValue(chartData, chartMode)
                                              : 0;
            double barHeight = chartContainerHeight * 0.74 * adjustHeightSSMR;

            double divisor = maxValue <= 0 ? barHeight : maxValue;
            double barHeightByValues = barHeight / divisor;

            if (_hasLineChart)
            {
                DrawLineChart(chartData, barBottomMargin, barHeight, barHeightByValues, chartContainerWidth);
            }

            DrawBarChart(chartData, barTopMargin, barBottomMargin, barHeight, barHeightByValues, chartContainerWidth
                , chartContainerHeight, isNormalMeter, chartMode, isREAccount);
        }

        /// <summary>
        /// Draws the bar chart.
        /// </summary>
        /// <param name="chartData">Chart data.</param>
        /// <param name="barTopMargin">Bar top margin.</param>
        /// <param name="barBottomMargin">Bar bottom margin.</param>
        /// <param name="barHeight">Bar height.</param>
        /// <param name="barHeightByValues">Bar height by values.</param>
        /// <param name="chartContainerWidth">Chart container width.</param>
        /// <param name="chartContainerHeight">Chart container height.</param>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        /// <param name="chartMode">Chart mode.</param>
        private void DrawBarChart(List<SegmentDetailsModel> chartData, int barTopMargin
            , int barBottomMargin, double barHeight, double barHeightByValues
            , double chartContainerWidth, double chartContainerHeight
            , bool isNormalMeter = true, ChartModeEnum chartMode = ChartModeEnum.Cost
            , bool isREAccount = false)
        {
            double barMargin = 10;
            if (_parentView.Frame.Width == 320)
            {
                barMargin = 10;
            }
            else if (_parentView.Frame.Width == 375)
            {
                barMargin = 12;
            }
            else if (_parentView.Frame.Width == 414)
            {
                barMargin = 15;
            }

            float lblDateY = (float)chartContainerHeight - (DeviceHelper.IsIphoneXUpResolution() ? 38 : 14);
            float viewLineY = (float)chartContainerHeight - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 20);

            if (DeviceHelper.IsIphone4())
            {
                lblDateY = (float)chartContainerHeight;
                viewLineY = (float)chartContainerHeight;
            }

            for (int i = 0; i < chartData.Count; i++)
            {
                SegmentDetailsModel segmentData = chartData[i];
                var index = i;
                int x = (int)chartContainerWidth / chartData.Count;

                // set chart value based on consumption mode
                var chartValue = GetChartValue(segmentData, chartMode);
                double chartValueDbl = TextHelper.ParseStringToDouble(chartValue);
                var chartValue2 = GetChartValue(segmentData, ChartModeEnum.REUsage);
                double chartValue2Dbl = TextHelper.ParseStringToDouble(chartValue2);

                if (isREAccount)
                {
                    chartValueDbl = ChartHelper.UpdateValueForRE(chartValueDbl);
                }

                UIView viewSegment = new UIView(new CGRect(x * index, 0, x, chartContainerHeight))
                {
                    Tag = 0
                };

                var lblText = FormatChartValue(chartValueDbl, chartMode);
                var maxLines = 1;
                var lblOffset = 8;

                if (isREAccount)
                {
                    maxLines = 2;
                    lblText += string.Format("{0}{1}", Environment.NewLine, FormatChartValue(chartValue2Dbl, ChartModeEnum.REUsage));

                    if (DeviceHelper.IsIphone5() || DeviceHelper.IsIphone4())
                        lblOffset += 3;
                }

                var lblHeight = 14;
                var lblY = (barHeight - barHeightByValues * Math.Abs(chartValueDbl) - lblOffset * maxLines);
                UILabel lblCost = new UILabel(new CGRect(-15, lblY
                    , viewSegment.Frame.Width + 36, lblHeight * maxLines))
                {
                    Font = MyTNBFont.MuseoSans10_500,
                    TextColor = UIColor.White,
                    TextAlignment = UITextAlignment.Center,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Tag = 0,
                    Hidden = true,
                    Lines = maxLines,
                    Text = lblText
                };
                viewSegment.AddSubview(lblCost);

                UIView viewBar = new UIView(new CGRect(barMargin, viewSegment.Frame.Height - barTopMargin
                    , viewSegment.Frame.Width - (barMargin * 2), 0))
                {
                    BackgroundColor = UIColor.FromWhiteAlpha(1.0f, 0.2f),
                    Tag = 1
                };
                viewBar.Layer.CornerRadius = 10.0f;
                viewSegment.AddSubview(viewBar);

                UILabel lblDate = new UILabel(new CGRect(0, lblDateY
                    , viewSegment.Frame.Width, 14))
                {
                    Font = MyTNBFont.MuseoSans9_300,
                    TextColor = UIColor.FromWhiteAlpha(1.0f, 0.2f),
                    TextAlignment = UITextAlignment.Center
                };

                string dateString = string.Empty;
                if (DataManager.DataManager.SharedInstance.IsMontView)
                {
                    dateString = segmentData.Month;
                }
                else if (index == chartData.Count - 1 || chartData[index + 1].Day == "1")
                {
                    dateString = string.Format("{0} {1}", segmentData.Day, segmentData.Month);
                }
                else
                {
                    dateString = segmentData.Day;
                }

                lblDate.Text = dateString;
                viewSegment.AddSubview(lblDate);

                UITapGestureRecognizer onSegmentTap = new UITapGestureRecognizer(() =>
                {
                    int tapIndex = !_hasLineChart ? index : index + 1;
                    OnSegmentClick(tapIndex);
                });
                double ht = barHeightByValues * Math.Abs(chartValueDbl);
                double y = barBottomMargin + (barHeight - barHeightByValues * Math.Abs(chartValueDbl));

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {

                        viewBar.Frame = new CGRect(barMargin
                                                   , barBottomMargin
                                                        + (barHeight - barHeightByValues * Math.Abs(chartValueDbl))
                                                   , viewSegment.Frame.Width - (barMargin * 2)
                                                   , barHeightByValues * Math.Abs(chartValueDbl));
                    }
                    , () =>
                    {
                        viewBar.Frame = new CGRect(barMargin
                                                   , barBottomMargin
                                                        + (barHeight - barHeightByValues * Math.Abs(chartValueDbl))
                                                   , viewSegment.Frame.Width - (barMargin * 2)
                                                   , barHeightByValues * Math.Abs(chartValueDbl));
                        SetDefaultDate();
                    }
                );

                viewSegment.AddGestureRecognizer(onSegmentTap);
                _viewChart.AddSubview(viewSegment);
            }

            UIView viewLine = new UIView(new CGRect(0, viewLineY, chartContainerWidth, 1))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1.0f, 0.2f)
            };
            _viewChart.AddSubview(viewLine);
        }

        /// <summary>
        /// Draws the line chart.
        /// </summary>
        /// <param name="chartData">Chart data.</param>
        /// <param name="barBottomMargin">Bar bottom margin.</param>
        /// <param name="barHeight">Bar height.</param>
        /// <param name="barHeightByValues">Bar height by values.</param>
        /// <param name="chartContainerWidth">Chart container width.</param>
        private void DrawLineChart(List<SegmentDetailsModel> chartData, int barBottomMargin
            , double barHeight, double barHeightByValues, double chartContainerWidth)
        {
            var points = new List<CGPoint>();
            int x = (int)chartContainerWidth / chartData.Count;

            for (int i = 0; i < chartData.Count; i++)
            {
                SegmentDetailsModel segmentData = chartData[i];
                double y = barBottomMargin + (barHeight - barHeightByValues * Math.Abs(TextHelper.ParseStringToDouble(segmentData.Amount)));
                points.Add(new CGPoint((x * i) + (x / 2), y));
            }

            LineChartComponent lineChart = new LineChartComponent(points) { Frame = new CGRect(0, 0, _viewChart.Frame.Width, _viewChart.Frame.Height) }; //{ Frame = _viewChart.Frame };
            _viewChart.AddSubview(lineChart);
        }

        /// <summary>
        /// Gets the max value.
        /// </summary>
        /// <returns>The max value.</returns>
        /// <param name="chartData">Chart data.</param>
        /// <param name="chartMode">Chart mode.</param>
        private double GetMaxValue(List<SegmentDetailsModel> chartData, ChartModeEnum chartMode)
        {
            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    return chartData.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x.Amount)));
                case ChartModeEnum.Usage:
                    return chartData.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x.Consumption)));
                case ChartModeEnum.Emission:
                    return chartData.Max(x =>
                    {
                        if (!string.IsNullOrEmpty(x.CO2))
                        {
                            return Math.Abs(TextHelper.ParseStringToDouble(x.CO2));
                        }
                        else
                        {
                            return 0;
                        }
                    });
            }
        }

        /// <summary>
        /// Gets the chart value.
        /// </summary>
        /// <returns>The chart value.</returns>
        /// <param name="segment">Segment.</param>
        /// <param name="chartMode">Chart mode.</param>
        private string GetChartValue(SegmentDetailsModel segment, ChartModeEnum chartMode)
        {
            var str = "0";
            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    str = segment.Amount;
                    break;
                case ChartModeEnum.Usage:
                    str = segment.Consumption;
                    break;
                case ChartModeEnum.Emission:
                    str = segment.CO2;
                    break;
                case ChartModeEnum.REUsage:
                    str = segment.Usage;
                    break;
            }

            if (string.IsNullOrEmpty(str))
            {
                str = TNBGlobal.ZERO;
            }

            return str;
        }

        /// <summary>
        /// Formats the chart value.
        /// </summary>
        /// <returns>The chart value.</returns>
        /// <param name="value">Value.</param>
        /// <param name="chartMode">Chart mode.</param>
        private string FormatChartValue(double value, ChartModeEnum chartMode)
        {
            var str = value.ToString("N2", CultureInfo.InvariantCulture);
            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    return string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, str);
                case ChartModeEnum.Usage:
                case ChartModeEnum.REUsage:
                    return string.Format("{0} {1}", str, Constants.UnitEnergy);
                case ChartModeEnum.Emission:
                    return string.Format("{0} {1}", str, Constants.UnitEmission);
            }
        }

        /// <summary>
        /// Sets the frame based on meter type.
        /// </summary>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void SetFrameByMeterType(bool isNormalMeter)
        {
            int yLocation = GetChartYLocation(isNormalMeter);

            var newFrame = _viewChart.Frame;
            newFrame.Y = yLocation;
            _viewChart.Frame = newFrame;
        }

        /// <summary>
        /// Gets the chart Y Location.
        /// </summary>
        /// <returns>The chart YL ocation.</returns>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        private int GetChartYLocation(bool isNormalMeter = true)
        {
            var isAccountActive = DataManager.DataManager.SharedInstance.AccountIsActive;
            var isAccountSSMR = DataManager.DataManager.SharedInstance.AccountIsSSMR;
            var yValue = isAccountActive ? 51f : 30f;
            int yLocation = (int)DeviceHelper.GetScaledHeight(yValue);
            if (!DataManager.DataManager.SharedInstance.IsSmartMeterAvailable && !isNormalMeter)
            {
                isNormalMeter = true;
            }
            if (!isAccountSSMR)
            {
                if (!isNormalMeter)
                {
                    if (!DeviceHelper.IsIphoneXUpResolution())
                    {
                        if (DeviceHelper.IsIphone5())
                        {
                            yLocation += 25;
                        }
                        else
                        {
                            yLocation += 20;
                        }
                    }
                    else
                    {
                        yLocation = 70;
                    }
                }
            }
            else
            {
                if (DeviceHelper.IsIphoneXUpResolution())
                {
                    if (DeviceHelper.IsIphoneXOrXs())
                    {
                        yLocation -= 20;
                    }
                }
                else if (DeviceHelper.IsIphone6UpResolution())
                {
                    yLocation -= 25;
                }
                else
                {
                    yLocation -= 25;
                }
            }

            return yLocation;
        }

        internal void OnSegmentClick(int index)
        {
            for (int i = 0; i <= _viewChart.Subviews.Count() - 1; i++)
            {
                UIView segment = _viewChart.Subviews[i];
                if (segment.Tag != 0)
                {
                    continue;
                }
                ToggleSelected(i == index, segment);
            }
        }

        internal void ToggleSelected(bool isSelected, UIView segment)
        {
            for (int i = 0; i < segment.Subviews.Count(); i++)
            {
                UIView view = segment.Subviews[i];

                if (i == 0)
                {
                    view.Hidden = !isSelected;
                }
                else if (i == 1)
                {
                    view.BackgroundColor = isSelected ? UIColor.FromWhiteAlpha(1.0f, 0.7f)
                        : UIColor.FromWhiteAlpha(1.0f, 0.2f);
                }
                else if (i == 2)
                {
                    (view as UILabel).TextColor = isSelected ? UIColor.FromWhiteAlpha(1.0f, 0.7f)
                        : UIColor.FromWhiteAlpha(1.0f, 0.2f);
                }
            }
        }

        public UIView GetUI()
        {
            return GetUI(true);
        }

        public UIView GetUI(bool isNormalMeter)
        {
            CreateComponent(isNormalMeter);
            return _viewChart;
        }

        public void UnmountUI()
        {
            _viewChart.RemoveFromSuperview();
        }

        internal void SetDefaultDate()
        {
            var index = DataManager.DataManager.SharedInstance.IsMontView ? 5 : 6;
            if (_hasLineChart)
            {
                index++;
            }
            OnSegmentClick(index);
        }
    }
}
using System;
using System.Globalization;
using System.Linq;
using CoreGraphics;
using Foundation;
using myTNB.Enums;
using myTNB.Model;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class ChartCompanionComponent
    {
        public UITapGestureRecognizer ToolTipGestureRecognizer;

        UIView _parentView, _baseView, _chartModeView, _metricView1, _metricView2, _viewTooltip;
        InfoComponent _metricCmp1, _metricCmp2;
        UIButton _amountBtn, _consumptionBtn, _emissionBtn;
        UsageMetrics _usageMetrics;
        double _yLocation;

        private string For = "Component_For".Translate();
        private string MyCostSoFar = "Component_MyCostSoFar".Translate();
        private string MyCostLikelyToBe = "Component_MyCostLikelyToBe".Translate();
        private string MyCurrentUsage = "Component_MyCurrentUsage".Translate();
        private string MyCurrentUsageTrend = "Component_MyCurrentUsageTrend".Translate();
        private string More = "Component_CompareMore".Translate();
        private string Less = "Component_CompareLess".Translate();
        private string Same = "Component_CompareSame".Translate();
        private string TxtCurrentEmission = "Component_CurrentEmission".Translate();
        private const string TxtCurrency = "RM";
        private string TxtUsage = "Component_Usage".Translate();
        private string TxtEmission = "Component_CO2".Translate();
        private double center, btnWidth = 64, btnOffset = 12;
        private int margin = 18;
        private ToolTipComponent _toolTipComponent;

        public ChartCompanionComponent(UIView view)
        {
            _parentView = view;
            _yLocation = _parentView.Frame.Height - 40;
        }

        public ChartCompanionComponent(UIView view, double yLocation) : this(view)
        {
            _yLocation = yLocation;
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent()
        {
            if (!DeviceHelper.IsIphoneXUpResolution())
            {
                _yLocation += 12; // 24 margin
            }
            _baseView = new UIView(new CGRect(margin, _yLocation + 12, _parentView.Frame.Width - (margin * 2), 171));
            center = _baseView.Frame.Width / 2;
            // chart mode
            UITextAttributes attr = new UITextAttributes();
            attr.Font = MyTNBFont.MuseoSans12;
            attr.TextColor = UIColor.White;

            CreateChartViewButtons();
            CreateMetricComponent();
            CreateTooltip();
            _baseView.Frame = new CGRect(_baseView.Frame.X, _baseView.Frame.Y, _baseView.Frame.Width, _viewTooltip.Frame.GetMaxY());
        }

        private void CreateChartViewButtons()
        {
            _chartModeView = new UIView(new CGRect(margin, 0, _baseView.Frame.Width, 26));
            _baseView.AddSubview(_chartModeView);

            _amountBtn = new UIButton
            {
                Frame = new CGRect(center - margin - (btnWidth * 2 + btnOffset) / 2, 0, btnWidth, 26),
                BackgroundColor = UIColor.Clear
            };
            _amountBtn.SetAttributedTitle(CreateAttributedTitle(TxtCurrency, MyTNBColor.PowerBlue), UIControlState.Selected);
            _amountBtn.SetAttributedTitle(CreateAttributedTitle(TxtCurrency, UIColor.White), UIControlState.Normal);
            _amountBtn.Layer.BorderWidth = 1.0f;
            _amountBtn.Layer.BorderColor = UIColor.White.CGColor;
            _amountBtn.Layer.CornerRadius = 13.0f;

            _consumptionBtn = new UIButton
            {
                Frame = new CGRect(_amountBtn.Frame.GetMaxX() + btnOffset, 0, btnWidth, 26),
                BackgroundColor = UIColor.Clear
            };
            _consumptionBtn.SetAttributedTitle(CreateAttributedTitle(TxtUsage, MyTNBColor.PowerBlue), UIControlState.Selected);
            _consumptionBtn.SetAttributedTitle(CreateAttributedTitle(TxtUsage, UIColor.White), UIControlState.Normal);
            _consumptionBtn.Layer.BorderWidth = 1.0f;
            _consumptionBtn.Layer.BorderColor = UIColor.White.CGColor;
            _consumptionBtn.Layer.CornerRadius = 13.0f;

            _emissionBtn = new UIButton
            {
                Frame = new CGRect(center + 80, 0, btnWidth, 26),
                BackgroundColor = UIColor.Clear
            };
            _emissionBtn.SetAttributedTitle(CreateAttributedTitle(TxtEmission, MyTNBColor.PowerBlue), UIControlState.Selected);
            _emissionBtn.SetAttributedTitle(CreateAttributedTitle(TxtEmission, UIColor.White), UIControlState.Normal);
            _emissionBtn.Layer.BorderWidth = 1.0f;
            _emissionBtn.Layer.BorderColor = UIColor.White.CGColor;
            _emissionBtn.Layer.CornerRadius = 13.0f;

            if (!TNBGlobal.IsChartEmissionEnabled)
            {
                _emissionBtn.Hidden = true;
            }
            else
            {
                var posX = center - margin - ((btnWidth * 3 + btnOffset * 2) / 2);
                var newFrame = _amountBtn.Frame;
                newFrame.X = (float)posX;
                _amountBtn.Frame = newFrame;

                newFrame = _consumptionBtn.Frame;
                newFrame.X = (float)(_amountBtn.Frame.GetMaxX() + btnOffset);
                _consumptionBtn.Frame = newFrame;

                newFrame = _emissionBtn.Frame;
                newFrame.X = (float)(_consumptionBtn.Frame.GetMaxX() + btnOffset);
                _emissionBtn.Frame = newFrame;
                _emissionBtn.Hidden = false;
            }

            _chartModeView.AddSubviews(new UIView[] { _amountBtn, _consumptionBtn, _emissionBtn });
        }

        private void CreateMetricComponent()
        {
            _metricCmp1 = new InfoComponent(_baseView, new CGRect(0, _chartModeView.Frame.GetMaxY() + 24, _baseView.Frame.Width, 58));
            _metricCmp1.Icon.Image = UIImage.FromBundle("IC-Charges");
            _metricCmp1.TitleLabel.Text = MyCostSoFar;
            _metricCmp1.SubTitleLabel.Text = For;
            _metricCmp1.ValueLabel.Text = "RM 0";
            _metricView1 = _metricCmp1.GetUI();

            _metricCmp2 = new InfoComponent(_baseView, new CGRect(0, _metricView1.Frame.Y + _metricView1.Frame.Height + 1, _baseView.Frame.Width, 58));
            _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Cost");
            _metricCmp2.TitleLabel.Text = MyCostLikelyToBe;
            _metricCmp2.SubTitleLabel.Text = For;
            _metricCmp2.ValueLabel.Text = "RM 0";
            _metricView2 = _metricCmp2.GetUI();

            _baseView.AddSubviews(new UIView[] { _metricView1, _metricView2 });
        }

        private void CreateTooltip()
        {
            _toolTipComponent = new ToolTipComponent(_baseView);
            _viewTooltip = _toolTipComponent.GetUI();
            _toolTipComponent.SetContent("Dashboard_WhatAreTheseCost");
            _toolTipComponent.SetEvent(ToolTipGestureRecognizer);
            _toolTipComponent.SetTopMargin(_metricView2.Frame.GetMaxY() + 8F);
            _baseView.AddSubview(_viewTooltip);
        }

        public void SetTooltipLink(string title)
        {
            _toolTipComponent.SetContent(title ?? "Dashboard_WhatAreTheseCost");
        }

        /// <summary>
        /// Gets the user interface.
        /// </summary>
        /// <returns>The user interface.</returns>
        public UIView GetUI()
        {
            CreateComponent();
            return _baseView;
        }

        /// <summary>
        /// Adds the chart mode handler.
        /// </summary>
        /// <param name="chartMode">Chart mode.</param>
        /// <param name="handler">Handler.</param>
        public void AddChartModeHandler(ChartModeEnum chartMode, EventHandler handler)
        {
            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    _amountBtn.TouchUpInside += handler;
                    break;
                case ChartModeEnum.Usage:
                    _consumptionBtn.TouchUpInside += handler;
                    break;
                case ChartModeEnum.Emission:
                    _emissionBtn.TouchUpInside += handler;
                    break;
            }
        }

        /// <summary>
        /// Sets the hidden property of the whole view.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void SetHidden(bool isHidden)
        {
            _baseView.Hidden = isHidden;
        }

        /// <summary>
        /// Sets the chart mode.
        /// </summary>
        /// <param name="chartMode">Chart mode.</param>
        public void SetChartMode(ChartModeEnum chartMode)
        {
            _consumptionBtn.Selected = chartMode == ChartModeEnum.Usage;
            _amountBtn.Selected = chartMode == ChartModeEnum.Cost;
            _emissionBtn.Selected = chartMode == ChartModeEnum.Emission;

            _amountBtn.BackgroundColor = chartMode == ChartModeEnum.Cost ? UIColor.White : UIColor.Clear;
            _consumptionBtn.BackgroundColor = chartMode == ChartModeEnum.Usage ? UIColor.White : UIColor.Clear;
            _emissionBtn.BackgroundColor = chartMode == ChartModeEnum.Emission ? UIColor.White : UIColor.Clear;

            UpdateMetricsDisplay(chartMode);
        }

        /// <summary>
        /// Sets the usage metric.
        /// </summary>
        /// <param name="metrics">Metrics.</param>
        public void SetUsageMetric(UsageMetrics metrics)
        {
            _usageMetrics = metrics;
        }

        /// <summary>
        /// Updates the metrics display.
        /// </summary>
        /// <param name="chartMode">Chart mode.</param>
        private void UpdateMetricsDisplay(ChartModeEnum chartMode)
        {
            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    {
                        _metricCmp1.Icon.Image = UIImage.FromBundle("IC-Charges");
                        _metricCmp1.TitleLabel.Text = MyCostSoFar;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(For, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCost?.AsOf);
                        var currCharges = _usageMetrics?.StatsByCost?.CurrentCharges ?? "0";
                        if (!string.IsNullOrEmpty(currCharges))
                        {
                            _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(currCharges, TNBGlobal.UNIT_CURRENCY + " "
                                , true, MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Cost");
                        _metricCmp2.TitleLabel.Text = MyCostLikelyToBe;
                        _metricCmp2.SubTitleLabel.Text = GetDateRange(For, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCost?.AsOf);
                        var prjctdCost = _usageMetrics?.StatsByCost?.ProjectedCost ?? "0";
                        if (!string.IsNullOrEmpty(prjctdCost))
                        {
                            _metricCmp2.ValueLabel.AttributedText = TextHelper.CreateValuePairString(prjctdCost, TNBGlobal.UNIT_CURRENCY + " "
                                , true, MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.ValuePairIcon.Image = null;
                    }
                    break;
                case ChartModeEnum.Usage:
                    {
                        _metricCmp1.Icon.Image = UIImage.FromBundle("IC-Energy-Usage");
                        _metricCmp1.TitleLabel.Text = MyCurrentUsage;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(For, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByUsage?.AsOf);
                        var currUsageKWH = _usageMetrics?.StatsByUsage?.CurrentUsageKWH ?? "0";
                        if (!string.IsNullOrEmpty(currUsageKWH))
                        {
                            _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(TextHelper.ParseStringToDouble(currUsageKWH)
                                .ToString("N2", CultureInfo.InvariantCulture), " " + TNBGlobal.UNITENERGY, false, MyTNBFont.MuseoSans16_300
                                , UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Avg-Elec-Usage");
                        _metricCmp2.TitleLabel.Text = MyCurrentUsageTrend;

                        string value;
                        bool hasChange;
                        bool isUp;
                        GetUsageComparisonAttributes(_usageMetrics?.StatsByUsage?.UsageComparedToPrevious, out value, out hasChange, out isUp);
                        _metricCmp2.ValueLabel.AttributedText = TextHelper.CreateValuePairString(value, "%", false, MyTNBFont.MuseoSans16_300
                            , UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        _metricCmp2.ValuePairIcon.Image = isUp ? UIImage.FromBundle("Arrow-Up") : UIImage.FromBundle("Arrow-Down");
                        _metricCmp2.ValuePairIcon.Hidden = !hasChange;
                        string compareText = Same;
                        if (hasChange)
                        {
                            compareText = isUp ? More : Less;
                        }
                        _metricCmp2.SubTitleLabel.Text = compareText;
                        AdjustArrowFrames();
                    }
                    break;
                case ChartModeEnum.Emission:
                    {
                        _metricCmp1.Icon.Image = UIImage.FromBundle("IC-CO2");
                        _metricCmp1.TitleLabel.Text = TxtCurrentEmission;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(For, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCo2?.First()?.AsOf);
                        string value = _usageMetrics?.StatsByCo2?.Count > 0 ?
                                                     _usageMetrics?.StatsByCo2?.Sum(item => TextHelper.ParseStringToDouble(item.Quantity)).ToString() : "0";
                        _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(value, " " + TNBGlobal.UNITEMISSION, false
                            , MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                    }
                    break;
            }

            _viewTooltip.Hidden = chartMode != ChartModeEnum.Cost;
            _metricCmp2.SetHidden(chartMode == ChartModeEnum.Emission);
        }

        string GetDateRange(string preffix, string fromDate, string toDate)
        {
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrWhiteSpace(fromDate)
                || string.IsNullOrEmpty(toDate) || string.IsNullOrWhiteSpace(toDate))
            {
                return "--";
            }
            return preffix + fromDate + " - " + toDate;
        }

        /// <summary>
        /// Adjusts the frames.
        /// </summary>
        private void AdjustArrowFrames()
        {
            double valueWidth = _baseView.Frame.Width * 0.30;
            CGSize newSize = LabelHelper.GetLabelSize(_metricCmp2.ValueLabel, (float)valueWidth, _metricCmp2.ValueLabel.Frame.Height);
            float newWidth = (float)Math.Ceiling(newSize.Width);
            var temp = _metricCmp2.ValuePairIcon.Frame;
            temp.X = (float)_baseView.Frame.Width - newWidth - 8;
            _metricCmp2.ValuePairIcon.Frame = temp;
        }

        /// <summary>
        /// Gets the usage comparison attributes.
        /// </summary>
        /// <param name="inputText">Input text.</param>
        /// <param name="value">Value.</param>
        /// <param name="hasChange">If set to <c>true</c> has change.</param>
        /// <param name="isUp">If set to <c>true</c> is up.</param>
        private void GetUsageComparisonAttributes(string inputText, out string value, out bool hasChange, out bool isUp)
        {
            value = "0";
            hasChange = false;
            isUp = false;

            if (!string.IsNullOrEmpty(inputText))
            {
                var num = TextHelper.ParseStringToDouble(inputText);
                isUp = num > 0;
                hasChange = num != 0;// inputText != "0";
                value = (num < 0) ? inputText.Substring(1) : inputText;
            }
        }

        /// <summary>
        /// Creates the attributed title.
        /// </summary>
        /// <returns>The attributed title.</returns>
        /// <param name="text">Text.</param>
        private NSAttributedString CreateAttributedTitle(string text, UIColor textColor)
        {
            return new NSAttributedString(
                text ?? string.Empty,
                font: MyTNBFont.MuseoSans12,
                foregroundColor: textColor);
        }
    }
}
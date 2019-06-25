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

        UIView _parentView, _baseView, _chartModeView, _metricView1
            , _metricView2, _msgLabelView, _msgViewLine;
        InfoComponent _metricCmp1, _metricCmp2;
        UIButton _amountBtn, _consumptionBtn, _emissionBtn;
        UsageMetrics _usageMetrics;
        UILabel _messageLabel;
        double _yLocation;

        private const string TxtCurrentCharges = "My current charges";
        private const string TxtProjectedCost = "My estimated charges";
        private const string TxtAsOf = "from ";
        private const string TxtForCurrentMonth = "for ";
        private const string TxtCurrentUsage = "My current usage";
        private const string TxtAverageUsage = "My average electric usage";
        private const string TxtComparedFrom = "compared to previous bill period";
        private const string TxtVsLastMonth = "compoared to ";
        private const string TxtCurrentEmission = "Current emission";
        private const string TxtCurrency = "RM";
        private const string TxtUsage = "Usage";
        private const string TxtEmission = "CO2";

        public ChartCompanionComponent(UIView view)
        {
            _parentView = view;
            _yLocation = _parentView.Frame.Height - 40;
        }

        public ChartCompanionComponent(UIView view, double yLocation)
            : this(view)
        {
            _yLocation = yLocation;
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent()
        {
            int margin = 18;

            if (!DeviceHelper.IsIphoneXUpResolution())
            {
                _yLocation += 12; // 24 margin
            }
            _baseView = new UIView(new CGRect(margin, _yLocation, _parentView.Frame.Width - (margin * 2), 171));

            double center = _baseView.Frame.Width / 2;
            double btnWidth = 64;
            double btnOffset = 12;

            // chart mode
            UITextAttributes attr = new UITextAttributes();
            attr.Font = MyTNBFont.MuseoSans12;
            attr.TextColor = UIColor.White;
            _chartModeView = new UIView(new CGRect(margin, _baseView.Frame.Height - 26, _baseView.Frame.Width, 26));

            _msgLabelView = new UIView(new CGRect(0, _baseView.Frame.Height - 112, _baseView.Frame.Width, 59));
            _messageLabel = new UILabel
            {
                Frame = new CGRect(7, 13, _msgLabelView.Frame.Width - 14, 32),
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = UIColor.White,
                Lines = 2,
                LineBreakMode = UILineBreakMode.TailTruncation,
                TextAlignment = UITextAlignment.Left,
                Text = "Component_SmartMeterMessage".Translate()
            };
            _msgLabelView.AddSubview(_messageLabel);
            _msgViewLine = new UIView(new CGRect(0, _msgLabelView.Frame.Height - 1,
                                                    _msgLabelView.Frame.Width, 1))
            {
                BackgroundColor = MyTNBColor.SelectionSemiTransparent
            };
            _msgLabelView.AddSubview(_msgViewLine);
            _msgLabelView.Hidden = true;

            _baseView.AddSubview(_msgLabelView);

            _amountBtn = new UIButton();
            _amountBtn.Frame = new CGRect(center - margin - (btnWidth * 2 + btnOffset) / 2, 0, btnWidth, 26);
            _amountBtn.SetAttributedTitle(CreateAttributedTitle(TxtCurrency, MyTNBColor.PowerBlue), UIControlState.Selected);
            _amountBtn.SetAttributedTitle(CreateAttributedTitle(TxtCurrency, UIColor.White), UIControlState.Normal);
            _amountBtn.BackgroundColor = UIColor.Clear;
            _amountBtn.Layer.BorderWidth = 1.0f;
            _amountBtn.Layer.BorderColor = UIColor.White.CGColor;
            _amountBtn.Layer.CornerRadius = 13.0f;
            _chartModeView.AddSubview(_amountBtn);

            _consumptionBtn = new UIButton();
            _consumptionBtn.Frame = new CGRect(_amountBtn.Frame.GetMaxX() + btnOffset, 0, btnWidth, 26);
            _consumptionBtn.SetAttributedTitle(CreateAttributedTitle(TxtUsage, MyTNBColor.PowerBlue), UIControlState.Selected);
            _consumptionBtn.SetAttributedTitle(CreateAttributedTitle(TxtUsage, UIColor.White), UIControlState.Normal);
            _consumptionBtn.BackgroundColor = UIColor.Clear;
            _consumptionBtn.Layer.BorderWidth = 1.0f;
            _consumptionBtn.Layer.BorderColor = UIColor.White.CGColor;
            _consumptionBtn.Layer.CornerRadius = 13.0f;
            _chartModeView.AddSubview(_consumptionBtn);

            _emissionBtn = new UIButton();
            _emissionBtn.Frame = new CGRect(center + 80, 0, btnWidth, 26);
            _emissionBtn.SetAttributedTitle(CreateAttributedTitle(TxtEmission, MyTNBColor.PowerBlue), UIControlState.Selected);
            _emissionBtn.SetAttributedTitle(CreateAttributedTitle(TxtEmission, UIColor.White), UIControlState.Normal);
            _emissionBtn.BackgroundColor = UIColor.Clear;
            _emissionBtn.Layer.BorderWidth = 1.0f;
            _emissionBtn.Layer.BorderColor = UIColor.White.CGColor;
            _emissionBtn.Layer.CornerRadius = 13.0f;
            _chartModeView.AddSubview(_emissionBtn);

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

            _baseView.AddSubview(_chartModeView);

            // metrics
            _metricCmp1 = new InfoComponent(_baseView);
            _metricCmp1.Icon.Image = UIImage.FromBundle("IC-Charges");
            _metricCmp1.TitleLabel.Text = TxtCurrentCharges;
            _metricCmp1.SubTitleLabel.Text = TxtAsOf;
            _metricCmp1.ValueLabel.Text = "RM 0";
            _metricView1 = _metricCmp1.GetUI();
            _baseView.AddSubview(_metricView1);

            _metricCmp2 = new InfoComponent(_baseView, new CGRect(0, _metricView1.Frame.Y + _metricView1.Frame.Height + 1, _baseView.Frame.Width, 58));
            _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Cost");
            _metricCmp2.TitleLabel.Text = TxtProjectedCost;
            _metricCmp2.SubTitleLabel.Text = TxtForCurrentMonth;
            _metricCmp2.ValueLabel.Text = "RM 0";
            /*_metricCmp2.InfoMsgLabel.Text = "What is this?";
            if (ToolTipGestureRecognizer != null)
            {
                _metricCmp2.InfoMsgUIView.AddGestureRecognizer(ToolTipGestureRecognizer);
            }*/
            _metricView2 = _metricCmp2.GetUI();
            _baseView.AddSubview(_metricView2);

        }

        /// <summary>
        /// Shows the message label for the Smart Meter graph in Monthly view.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public void ShowMessage(bool flag)
        {
            _msgLabelView.Hidden = flag;
            CGRect baseFrame = _baseView.Frame;
            CGRect chartFrame = _chartModeView.Frame;
            CGRect labelMsgFrame = _msgLabelView.Frame;
            baseFrame.Height = flag ? 171 : 230;
            _baseView.Frame = baseFrame;
            chartFrame = new CGRect(18, _baseView.Frame.Height - 26, _baseView.Frame.Width, 26);
            labelMsgFrame = new CGRect(0, _baseView.Frame.Height - 112, _baseView.Frame.Width, 59);
            _chartModeView.Frame = chartFrame;
            _msgLabelView.Frame = labelMsgFrame;
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

            switch (chartMode)
            {
                default:
                case ChartModeEnum.Cost:
                    {
                        _consumptionBtn.BackgroundColor = UIColor.Clear;
                        _amountBtn.BackgroundColor = UIColor.White;
                        _emissionBtn.BackgroundColor = UIColor.Clear;
                    }
                    break;
                case ChartModeEnum.Usage:
                    {
                        _consumptionBtn.BackgroundColor = UIColor.White;
                        _amountBtn.BackgroundColor = UIColor.Clear;
                        _emissionBtn.BackgroundColor = UIColor.Clear;
                    }
                    break;
                case ChartModeEnum.Emission:
                    {
                        _consumptionBtn.BackgroundColor = UIColor.Clear;
                        _amountBtn.BackgroundColor = UIColor.Clear;
                        _emissionBtn.BackgroundColor = UIColor.White;
                    }
                    break;

            }

            //_metricCmp2.ToggleInfoPopup(chartMode == ChartModeEnum.Cost);

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
                        _metricCmp1.TitleLabel.Text = TxtCurrentCharges;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(TxtAsOf, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCost?.AsOf);
                        var currCharges = _usageMetrics?.StatsByCost?.CurrentCharges ?? "0";
                        if (!string.IsNullOrEmpty(currCharges))
                        {
                            _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(currCharges, TNBGlobal.UNIT_CURRENCY + " "
                                , true, MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Cost");
                        _metricCmp2.TitleLabel.Text = TxtProjectedCost;
                        _metricCmp2.SubTitleLabel.Text = GetDateRange(TxtForCurrentMonth, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCost?.AsOf);
                        var prjctdCost = _usageMetrics?.StatsByCost?.ProjectedCost ?? "0";
                        if (!string.IsNullOrEmpty(prjctdCost))
                        {
                            _metricCmp2.ValueLabel.AttributedText = TextHelper.CreateValuePairString(prjctdCost, TNBGlobal.UNIT_CURRENCY + " "
                                , true, MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.ValuePairIcon.Image = null;
                        _metricCmp2.SetHidden(false);
                    }
                    break;
                case ChartModeEnum.Usage:
                    {
                        _metricCmp1.Icon.Image = UIImage.FromBundle("IC-Energy-Usage");
                        _metricCmp1.TitleLabel.Text = TxtCurrentUsage;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(TxtAsOf, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByUsage?.AsOf);
                        var currUsageKWH = _usageMetrics?.StatsByUsage?.CurrentUsageKWH ?? "0";
                        if (!string.IsNullOrEmpty(currUsageKWH))
                        {
                            _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(TextHelper.ParseStringToDouble(currUsageKWH)
                                .ToString("N2", CultureInfo.InvariantCulture), " " + TNBGlobal.UNITENERGY, false, MyTNBFont.MuseoSans16_300
                                , UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        }
                        _metricCmp2.Icon.Image = UIImage.FromBundle("IC-Avg-Elec-Usage");
                        _metricCmp2.TitleLabel.Text = TxtAverageUsage;
                        _metricCmp2.SubTitleLabel.Text = TxtComparedFrom;

                        string value;
                        bool hasChange;
                        bool isUp;
                        GetUsageComparisonAttributes(_usageMetrics?.StatsByUsage?.UsageComparedToPrevious, out value, out hasChange, out isUp);
                        _metricCmp2.ValueLabel.AttributedText = TextHelper.CreateValuePairString(value, "%", false, MyTNBFont.MuseoSans16_300
                            , UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        _metricCmp2.ValuePairIcon.Image = isUp ? UIImage.FromBundle("Arrow-Up") : UIImage.FromBundle("Arrow-Down");
                        _metricCmp2.ValuePairIcon.Hidden = !hasChange;
                        _metricCmp2.SetHidden(false);
                        AdjustArrowFrames();
                    }
                    break;
                case ChartModeEnum.Emission:
                    {
                        _metricCmp1.Icon.Image = UIImage.FromBundle("IC-CO2");
                        _metricCmp1.TitleLabel.Text = TxtCurrentEmission;
                        _metricCmp1.SubTitleLabel.Text = GetDateRange(TxtAsOf, _usageMetrics?.FromCycleDate, _usageMetrics?.StatsByCo2?.First()?.AsOf);
                        string value = _usageMetrics?.StatsByCo2?.Count > 0 ?
                                                     _usageMetrics?.StatsByCo2?.Sum(item => TextHelper.ParseStringToDouble(item.Quantity)).ToString() : "0";
                        _metricCmp1.ValueLabel.AttributedText = TextHelper.CreateValuePairString(value, " " + TNBGlobal.UNITEMISSION, false
                            , MyTNBFont.MuseoSans16_300, UIColor.White, MyTNBFont.MuseoSans12_300, UIColor.White);
                        _metricCmp2.SetHidden(true);
                    }
                    break;
            }
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
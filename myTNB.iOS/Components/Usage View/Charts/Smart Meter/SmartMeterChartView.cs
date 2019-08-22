using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class SmartMeterChartView : BaseChartView
    {
        public SmartMeterChartView()
        {
            ShimmerHeight = GetHeightByScreenSize(203);
        }

        protected override void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetWidthByScreenSize(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetHeightByScreenSize(189)));

            _lblDateRange = new UILabel(new CGRect(_baseMargin, 0, _baseMarginedWidth, GetHeightByScreenSize(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = AccountUsageCache.ByMonthDateRange
            };

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrameScreenSize(_lblDateRange.Frame, 150)
                , _baseMarginedWidth, GetHeightByScreenSize(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            CreateSegment();
        }

        protected override void CreateSegment()
        {
            _segmentContainer = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(_lblDateRange.Frame, 16)
               , _width, GetHeightByScreenSize(157)));

            nfloat height = _segmentContainer.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(18);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetHeightByScreenSize(14);
            nfloat maxBarHeight = GetHeightByScreenSize(108);
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            nfloat segmentWidth = GetWidthByScreenSize(30);
            nfloat barMargin = GetWidthByScreenSize(9);

            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            List<string> valueList = usageData.Select(x => x.UsageTotal).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                bool isLatestBar = index == usageData.Count - 1;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapSmartMeterMonthBar"
                };
                _segmentContainer.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.UsageTotal, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = lblHeight + amountBarMargin + (maxBarHeight - barHeight);

                nfloat barWidth = isLatestBar ? GetWidthByScreenSize(16) : width;
                nfloat barXLoc = isLatestBar ? barMargin - GetWidthByScreenSize(2) : barMargin;
                CustomUIView viewBar = new CustomUIView(new CGRect(barXLoc
                    , segment.Frame.Height - lblHeight - GetHeightByScreenSize(17), barWidth, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = barWidth / 2;
                if (isLatestBar)
                {
                    viewBar.Layer.BorderWidth = GetWidthByScreenSize(1);
                    viewBar.Layer.BorderColor = (index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White).CGColor;
                }

                nfloat coverWidth = isLatestBar ? viewBar.Frame.Width - GetWidthByScreenSize(6) : viewBar.Frame.Width;
                nfloat coverXLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
                nfloat coverHeight = isLatestBar ? barHeight - GetHeightByScreenSize(6) : barHeight;
                nfloat coverYLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;

                UIView viewCover = new UIView(new CGRect(new CGPoint(coverXLoc, coverYLoc), new CGSize(coverWidth, coverHeight)))
                {
                    BackgroundColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 2001,
                    Hidden = false
                };
                if (isLatestBar) { viewCover.Layer.CornerRadius = coverWidth / 2; }
                viewBar.AddSubview(viewCover);

                AddTariffBlocks(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size, isLatestBar);

                nfloat amtYLoc = yLoc - amountBarMargin - lblHeight;
                UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetWidthByScreenSize(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = item.AmountTotal.FormatAmountString(item.Currency),
                    Hidden = index < usageData.Count - 1,
                    Tag = 1002
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetWidthByScreenSize(100));
                lblAmount.Frame = new CGRect((segmentWidth - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                UILabel lblDate = new UILabel(new CGRect(0, segment.Frame.Height - lblHeight
                    , GetWidthByScreenSize(30), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.IsNullOrEmpty(item.Year) ? item.Month : string.Format(Format_Value, item.Month, item.Year),
                    Tag = 1003
                };
                nfloat lblDateWidth = lblDate.GetLabelWidth(GetWidthByScreenSize(30));
                lblDate.Frame = new CGRect((segmentWidth - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);

                segment.AddSubviews(new UIView[] { lblAmount, viewBar, lblDate });

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSegmentTap(index);
                }));

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                        lblAmount.Frame = new CGRect(lblAmount.Frame.X, amtYLoc, lblAmount.Frame.Width, lblAmount.Frame.Height);
                    }
                    , () => { }
                );
            }
            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size, bool isLatestBar)
        {
            if (viewBar == null || tariffList == null || tariffList.Count == 0) { return; }
            nfloat baseHeigt = size.Height;
            nfloat barMaxY = size.Height;
            nfloat xLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
            nfloat yLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;
            UIView viewTariffContainer = new UIView(new CGRect(xLoc, yLoc, size.Width, size.Height))
            {
                Tag = 2002,
                Hidden = true,
                ClipsToBounds = true
            };
            if (isLatestBar) { viewTariffContainer.Layer.CornerRadius = size.Width / 2; }
            for (int i = 0; i < tariffList.Count; i++)
            {
                TariffItemModel item = tariffList[i];
                double.TryParse(item.Usage, out double val);
                nfloat percentage = (nfloat)(val / baseValue);
                nfloat blockHeight = baseHeigt * percentage;
                barMaxY -= blockHeight;
                UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, size.Width, blockHeight))
                {
                    BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected)
                };
                viewTariffContainer.AddSubview(viewTariffBlock);
                barMaxY -= GetHeightByScreenSize(1);
            }
            viewBar.AddSubview(viewTariffContainer);
        }

        protected override void OnSegmentTap(int index)
        {
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                bool isLatestBar = i == _segmentContainer.Subviews.Count() - 1;
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (isLatestBar)
                {
                    bar.Layer.BorderColor = (isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F)).CGColor;
                }
                if (bar != null)
                {
                    UIView viewCover = bar.ViewWithTag(2001);
                    if (viewCover != null) { viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }
                    UIView viewTariff = bar.ViewWithTag(2002);
                    if (viewTariff != null)
                    {
                        for (int j = 0; j < viewTariff.Subviews.Count(); j++)
                        {
                            UIView tBlock = viewTariff.Subviews[j];
                            UIColor tColor = tBlock.BackgroundColor;
                            nint componentCount = tColor.CGColor.NumberOfComponents;
                            if (componentCount == 4)
                            {
                                nfloat[] components = tColor.CGColor.Components;
                                nfloat alpha = isSelected ? 1F : 0.5F;
                                tBlock.BackgroundColor = new UIColor(components[0], components[1], components[2], alpha);
                            }
                        }
                    }
                }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value != null)
                {
                    value.TextColor = isLatestBar || isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                    value.Hidden = isLatestBar ? false : !isSelected;
                }
                UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                if (date != null) { date.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }
                if (isLatestBar)
                {
                    Debug.WriteLine("Todo: Go to day view.");
                }
            }
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar == null) { continue; }
                UIView viewCover = bar.ViewWithTag(2001);
                if (viewCover != null) { viewCover.Hidden = isTariffView; }
                UIView viewTariff = bar.ViewWithTag(2002);
                if (viewTariff != null) { viewTariff.Hidden = !isTariffView; }
            }
        }

        public override void ToggleRMKWHValues(RMkWhEnum state)
        {
            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value == null) { continue; }
                value.Text = state == RMkWhEnum.RM ? usageData[i].AmountTotal.FormatAmountString(usageData[i].Currency)
                    : string.Format(Format_Value, usageData[i].UsageTotal, usageData[i].UsageUnit);
                nfloat lblAmountWidth = value.GetLabelWidth(GetWidthByScreenSize(200));
                value.Frame = new CGRect((GetWidthByScreenSize(30) - lblAmountWidth) / 2, value.Frame.Y, lblAmountWidth, value.Frame.Height);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class NormalChartView : BaseChartView
    {
        public NormalChartView()
        {
            ShimmerHeight = GetHeightByScreenSize(189);
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
            double divisor = maxValue > 0 ? maxBarHeight / maxValue : 0;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                bool isSelected = index == usageData.Count - 1;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapNormalBar"
                };
                _segmentContainer.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.UsageTotal, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = lblHeight + amountBarMargin + (maxBarHeight - barHeight);

                CustomUIView viewBar = new CustomUIView(new CGRect(barMargin
                    , segment.Frame.Height - lblHeight - GetHeightByScreenSize(17), width, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = width / 2;
                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F),
                    Tag = 2001,
                    Hidden = false
                };
                viewBar.AddSubview(viewCover);
                if (!item.DPCIndicator)
                {
                    AddTariffBlocks(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size);
                }

                nfloat amtYLoc = yLoc - amountBarMargin - lblHeight;
                UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetWidthByScreenSize(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_500,
                    TextColor = UIColor.White,
                    Text = item.AmountTotal.FormatAmountString(item.Currency),
                    Hidden = !isSelected,
                    Tag = 1002
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetWidthByScreenSize(100));
                ViewHelper.AdjustFrameSetWidth(lblAmount, lblAmountWidth);
                ViewHelper.AdjustFrameSetX(lblAmount, GetXLocationToCenterObject(lblAmountWidth, segment));

                UILabel lblDate = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, segment.Frame.Height - lblHeight
                    , GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.White,
                    Text = item.Month,
                    Tag = 1003
                };
                nfloat lblDateWidth = lblDate.GetLabelWidth(GetWidthByScreenSize(100));
                lblDate.Frame = new CGRect((segmentWidth - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);

                segment.AddSubviews(new UIView[] { lblAmount, viewBar, lblDate });

                UIImageView dpcIcon = new UIImageView();
                if (item.DPCIndicator)
                {
                    dpcIcon = new UIImageView(new CGRect(0, segment.Frame.Height - lblHeight - GetScaledHeight(12) - GetHeightByScreenSize(17), GetScaledWidth(12), GetScaledHeight(12)))
                    {
                        Image = UIImage.FromBundle(UsageConstants.IMG_DPCIndicator),
                        Hidden = true,
                        Tag = 1005
                    };
                    ViewHelper.AdjustFrameSetX(dpcIcon, GetXLocationToCenterObject(GetScaledWidth(12), segment));
                    segment.AddSubview(dpcIcon);
                }

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSegmentTap(index);
                    if (LoadTariffLegendWithIndex != null)
                    {
                        LoadTariffLegendWithIndex.Invoke(index);
                    }
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
            if (LoadTariffLegendWithIndex != null)
            {
                if (usageData != null && usageData.Count > 0)
                {
                    LoadTariffLegendWithIndex.Invoke(usageData.Count - 1);
                }
            }
            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size)
        {
            if (viewBar == null || tariffList == null || tariffList.Count == 0) { return; }
            nfloat baseHeigt = size.Height;
            nfloat barMaxY = size.Height;
            UIView viewTariffContainer = new UIView(new CGRect(0, 0, size.Width, size.Height))
            {
                Tag = 2002,
                Hidden = true
            };
            for (int i = 0; i < tariffList.Count; i++)
            {
                TariffItemModel item = tariffList[i];
                double val = item.Usage;
                double percentage = (baseValue > 0) ? (nfloat)(val / baseValue) : 0;
                nfloat blockHeight = (nfloat)(baseHeigt * percentage);
                barMaxY -= blockHeight;
                UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, size.Width, blockHeight))
                {
                    BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected, false)
                };
                viewTariffContainer.AddSubview(viewTariffBlock);
                barMaxY -= GetHeightByScreenSize(1);
            }
            viewBar.AddSubview(viewTariffContainer);
        }

        protected override void OnSegmentTap(int index)
        {
            if (_segmentContainer == null)
                return;

            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar != null)
                {
                    UIView viewCover = bar.ViewWithTag(2001);
                    UIView viewTariff = bar.ViewWithTag(2002);

                    if (IsTariffView)
                    {
                        List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
                        MonthItemModel item = usageData[i];
                        if (viewCover != null) { viewCover.Hidden = !item.DPCIndicator; }
                        if (viewTariff != null) { viewTariff.Hidden = item.DPCIndicator; }
                    }

                    if (viewCover != null) { viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }

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
                    List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
                    MonthItemModel item = usageData[i];

                    if (ConsumptionState == RMkWhEnum.RM || (ConsumptionState == RMkWhEnum.kWh && !item.DPCIndicator))
                    {
                        value.Hidden = !isSelected;
                    }
                    else
                    {
                        value.Hidden = true;
                    }
                }
                UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                if (date != null)
                {
                    date.Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300;
                    nfloat lblDateWidth = date.GetLabelWidth(GetWidthByScreenSize(100));
                    date.Frame = new CGRect((segmentView.Frame.Width - lblDateWidth) / 2, date.Frame.Y, lblDateWidth, date.Frame.Height);
                }
                UIImageView dpcIcon = segmentView.ViewWithTag(1005) as UIImageView;
                if (dpcIcon != null)
                {
                    dpcIcon.Alpha = isSelected ? 1 : 0.5F;
                }
            }
            SelectedIndex = index;
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            IsTariffView = isTariffView;
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar == null) { continue; }
                CGRect barOriginalFrame = bar.Frame;
                bar.Frame = new CGRect(bar.Frame.X, bar.Frame.GetMaxY(), bar.Frame.Width, 0);

                UIView viewCover = bar.ViewWithTag(2001);
                UIView viewTariff = bar.ViewWithTag(2002);

                if (isTariffView)
                {
                    List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
                    if (i < usageData.Count)
                    {
                        MonthItemModel item = usageData[i];
                        if (viewCover != null) { viewCover.Hidden = !usageData[i].DPCIndicator; }
                        if (viewTariff != null) { viewTariff.Hidden = usageData[i].DPCIndicator; }
                    }
                }
                else
                {
                    if (viewCover != null) { viewCover.Hidden = isTariffView; }
                    if (viewTariff != null) { viewTariff.Hidden = !isTariffView; }
                }

                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                CGRect valueOriginalFrame = new CGRect();
                if (value != null)
                {
                    valueOriginalFrame = value.Frame;
                    value.Frame = new CGRect(value.Frame.X, bar.Frame.GetMinY() - amountBarMargin - value.Frame.Height
                        , value.Frame.Width, value.Frame.Height);
                }
                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                   , () =>
                   {
                       bar.Frame = barOriginalFrame;
                       if (value != null)
                       {
                           value.Frame = valueOriginalFrame;
                       }
                   }
                   , () => { }
               );
            }
        }

        public override void ToggleRMKWHValues(RMkWhEnum state)
        {
            if (_segmentContainer == null)
                return;

            ConsumptionState = state;
            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value == null) { continue; }
                double usageTotal;
                double.TryParse(usageData[i].UsageTotal, out usageTotal);
                value.Text = state == RMkWhEnum.RM ? usageData[i].AmountTotal.FormatAmountString(usageData[i].Currency)
                    : string.Format(Format_Value, usageTotal, usageData[i].UsageUnit);
                nfloat lblAmountWidth = value.GetLabelWidth(GetWidthByScreenSize(200));
                value.Frame = new CGRect((GetWidthByScreenSize(30) - lblAmountWidth) / 2, value.Frame.Y, lblAmountWidth, value.Frame.Height);
                if (usageData[i].DPCIndicator)
                {
                    UpdateDPCIndicator(segmentView);
                }
            }
        }

        private void UpdateDPCIndicator(CustomUIView segmentView)
        {
            bool isSelected = segmentView.Tag == SelectedIndex;
            CustomUIView viewBar = segmentView.ViewWithTag(1001) as CustomUIView;
            UIImageView dpcIcon = segmentView.ViewWithTag(1005) as UIImageView;
            UILabel lblAmount = segmentView.ViewWithTag(1002) as UILabel;
            if (viewBar == null || dpcIcon == null || lblAmount == null) { return; }
            viewBar.Hidden = ConsumptionState == RMkWhEnum.kWh;
            dpcIcon.Hidden = ConsumptionState == RMkWhEnum.RM;
            lblAmount.Hidden = ConsumptionState == RMkWhEnum.kWh || !isSelected;
        }
    }
}

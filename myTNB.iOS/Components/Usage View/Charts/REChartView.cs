﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class REChartView : BaseChartView
    {
        public REChartView()
        {
        }

        protected override void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetScaledWidth(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetScaledHeight(203)));

            _lblDateRange = new UILabel(new CGRect(_baseMargin, 0, _baseMarginedWidth, GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = AccountUsageCache.ByMonthDateRange
            };

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrame(_lblDateRange.Frame, 164)
                , _baseMarginedWidth, GetScaledHeight(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            CreateSegment();
        }

        protected override void CreateSegment()
        {
            _segmentContainer = new CustomUIView(new CGRect(0, GetYLocationFromFrame(_lblDateRange.Frame, 16)
               , _width, GetScaledHeight(171)));

            nfloat height = _segmentContainer.Frame.Height;
            nfloat width = GetScaledWidth(12);
            nfloat barMargin = GetScaledWidth(36);
            nfloat baseMargin = GetScaledWidth(34);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetScaledHeight(14);
            nfloat maxBarHeight = GetScaledHeight(108);
            nfloat amountBarMargin = GetScaledHeight(4);

            List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            List<string> valueList = usageData.Select(x => x.UsageTotal).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, width, height)) { Tag = index };
                _segmentContainer.AddSubview(segment);
                xLoc += width + barMargin;

                double.TryParse(item.UsageTotal, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = (lblHeight * 2) + amountBarMargin + (maxBarHeight - barHeight);

                CustomUIView viewBar = new CustomUIView(new CGRect(0, segment.Frame.Height - lblHeight - GetScaledHeight(17), width, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = width / 2;

                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 2001,
                    Hidden = false
                };
                viewBar.AddSubview(viewCover);
                nfloat usageYLoc = yLoc - amountBarMargin - lblHeight;
                UILabel lblUsage = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetScaledWidth(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.Format(Format_Value, item.UsageTotal, item.UsageUnit),
                    Hidden = index < usageData.Count - 1,
                    Tag = 1002
                };
                nfloat lblUsageWidth = lblUsage.GetLabelWidth(GetScaledWidth(100));
                lblUsage.Frame = new CGRect((width - lblUsageWidth) / 2, lblUsage.Frame.Y, lblUsageWidth, lblUsage.Frame.Height);

                nfloat amtYLoc = usageYLoc - lblHeight;
                UILabel lblAmount = new UILabel(new CGRect(0, lblUsage.Frame.GetMinY() - lblHeight
                   , GetScaledWidth(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_500,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.Format(Format_Value, item.Currency, item.AmountTotal),
                    Hidden = index < usageData.Count - 1,
                    Tag = 1003
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetScaledWidth(100));
                lblAmount.Frame = new CGRect((width - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                UILabel lblDate = new UILabel(new CGRect(0, segment.Frame.Height - lblHeight
                    , GetScaledWidth(30), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = index < usageData.Count - 1 ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.IsNullOrEmpty(item.Year) ? item.Month : string.Format(Format_Value, item.Month, item.Year),
                    Tag = 1004
                };
                nfloat lblDateWidth = lblDate.GetLabelWidth(GetScaledWidth(30));
                lblDate.Frame = new CGRect((width - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);

                segment.AddSubviews(new UIView[] { lblUsage, lblAmount, viewBar, lblDate });

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSegmentTap(index);
                }));

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                   , () =>
                   {
                       viewBar.Frame = new CGRect(0, yLoc, width, barHeight);
                       lblAmount.Frame = new CGRect(lblAmount.Frame.X, amtYLoc, lblAmount.Frame.Width, lblAmount.Frame.Height);
                       lblUsage.Frame = new CGRect(lblUsage.Frame.X, usageYLoc, lblUsage.Frame.Width, lblUsage.Frame.Height);
                   }
                   , () => { }
               );
            }
            _mainView.AddSubview(_segmentContainer);
        }

        protected override void OnSegmentTap(int index)
        {
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                UIView viewCover = bar.ViewWithTag(2001);
                viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                UIView viewTariff = bar.ViewWithTag(2002);
                if (viewTariff != null && !viewTariff.Hidden)
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
                UILabel usage = segmentView.ViewWithTag(1002) as UILabel;
                if (usage != null)
                {
                    usage.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                    usage.Hidden = !isSelected;
                }
                UILabel amt = segmentView.ViewWithTag(1003) as UILabel;
                if (amt != null)
                {
                    amt.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                    amt.Hidden = !isSelected;
                }
                UILabel date = segmentView.ViewWithTag(1004) as UILabel;
                if (date != null)
                {
                    date.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                }
            }
        }
    }
}

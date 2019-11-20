using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class SmartMeterMonthView : BaseSmartMeterView
    {
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public Action<int> OnSegmentTap { set; private get; }
        public Action<int> LoadTariffLegendWithIndex { set; private get; }

        public override void CreateSegment(ref CustomUIView view)
        {
            base.CreateSegment(ref view);
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 6)
               , _width, GetHeightByScreenSize(157)));

            nfloat height = view.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(18);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetHeightByScreenSize(14);
            nfloat maxBarHeight = GetHeightByScreenSize(108);
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            nfloat segmentWidth = GetWidthByScreenSize(30);
            nfloat barMargin = GetWidthByScreenSize(7);

            List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
            List<string> valueList = IsAmountState ? usageData.Select(x => x.AmountTotal).ToList() : usageData.Select(x => x.UsageTotal).ToList();
            List<bool> dpcIndicatorList = usageData.Select(x => x.DPCIndicator).ToList();
            double maxValue = GetMaxValue(ConsumptionState, valueList, dpcIndicatorList);
            double divisor = maxValue > 0 ? maxBarHeight / maxValue : 0;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                bool isLatestBar = false;// index == usageData.Count - 1;
                bool isSelected = index == usageData.Count - 1;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapSmartMeterMonthBar"
                };
                view.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                if (index == usageData.Count - 1 && AccountUsageSmartCache.IsMDMSDown)
                {
                    UILabel lblDate = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, segment.Frame.Height - lblHeight
                        , GetWidthByScreenSize(40), lblHeight))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300,
                        TextColor = UIColor.White,
                        Text = item.Month,
                        Tag = 1003
                    };

                    UIImageView unavailableIcon = new UIImageView(new CGRect(0
                        , segment.Frame.Height - lblHeight - width - GetHeightByScreenSize(17)
                        , width, width))
                    {
                        Image = UIImage.FromBundle(UsageConstants.IMG_MDMSDownIcon),
                        Tag = 1009
                    };
                    ViewHelper.AdjustFrameSetX(unavailableIcon, GetXLocationToCenterObject(width, segment));

                    /*nfloat lblIndicatorHeight = GetScaledHeight(28);
                    UILabel lblIndicator = new UILabel(new CGRect(0, unavailableIcon.Frame.GetMinY() - GetScaledHeight(8) - lblIndicatorHeight, GetScaledWidth(54), lblIndicatorHeight))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300,
                        TextColor = UIColor.White,
                        Lines = 0,
                        Text = GetI18NValue(UsageConstants.I18N_MDMSUnavailable),
                        Tag = 1004
                    };
                    nfloat lblWidth = lblIndicator.GetLabelWidth(GetScaledWidth(54));
                    ViewHelper.AdjustFrameSetWidth(lblIndicator, lblWidth);
                    ViewHelper.AdjustFrameSetX(lblIndicator, GetXLocationToCenterObject(lblWidth, segment));
                    */
                    segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        OnSegmentTap(index);
                    }));

                    segment.AddSubviews(new UIView[] { unavailableIcon, lblDate });
                }
                else
                {
                    string valReference = IsAmountState ? item.AmountTotal : item.UsageTotal;
                    double.TryParse(valReference, out double value);
                    if (value < 0) { value = 0; }
                    nfloat barHeight = (nfloat)(divisor * value);
                    nfloat yLoc = lblHeight + amountBarMargin + (maxBarHeight - barHeight);

                    nfloat barWidth = isLatestBar ? GetWidthByScreenSize(18) : width;
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
                        viewBar.Layer.BorderColor = (isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F)).CGColor;
                    }

                    nfloat coverWidth = isLatestBar ? viewBar.Frame.Width - GetWidthByScreenSize(6) : viewBar.Frame.Width;
                    nfloat coverXLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
                    nfloat coverHeight = isLatestBar ? barHeight - GetHeightByScreenSize(6) : barHeight;
                    nfloat coverYLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;

                    UIView viewCover = new UIView(new CGRect(new CGPoint(coverXLoc, coverYLoc), new CGSize(coverWidth, coverHeight)))
                    {
                        BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F),
                        Tag = 2001,
                        Hidden = IsTariffView && !item.DPCIndicator
                    };
                    if (isLatestBar) { viewCover.Layer.CornerRadius = coverWidth / 2; }
                    viewBar.AddSubview(viewCover);
                    if (AddTariffBlocks != null && !item.DPCIndicator)
                    {
                        AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size, isLatestBar);
                    }
                    nfloat amtYLoc = yLoc - amountBarMargin - lblHeight;
                    double.TryParse(item.UsageTotal, out double usageTotal);
                    string displayText = ConsumptionState == RMkWhEnum.RM ? item.AmountTotal.FormatAmountString(item.Currency) :
                        string.Format(Format_Value, usageTotal, item.UsageUnit);

                    UILabel lblConsumption = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                        , GetWidthByScreenSize(100), lblHeight))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Font = TNBFont.MuseoSans_10_500,
                        TextColor = UIColor.White,
                        Text = displayText,
                        Hidden = !isSelected,
                        Tag = 1002
                    };

                    if (lblConsumption.Text != null)
                    {
                        nfloat lblAmountWidth = lblConsumption.GetLabelWidth(GetWidthByScreenSize(100));
                        lblConsumption.Frame = new CGRect((segmentWidth - lblAmountWidth) / 2, lblConsumption.Frame.Y
                            , lblAmountWidth, lblConsumption.Frame.Height);
                    }

                    UILabel lblDate = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, segment.Frame.Height - lblHeight
                        , GetWidthByScreenSize(40), lblHeight))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Font = index == usageData.Count - 1 ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300,
                        TextColor = UIColor.White,
                        Text = item.Month,
                        Tag = 1003
                    };

                    if (lblDate.Text != null)
                    {
                        nfloat lblDateWidth = lblDate.GetLabelWidth(GetWidthByScreenSize(100));
                        lblDate.Frame = new CGRect((segmentWidth - lblDateWidth) / 2, lblDate.Frame.Y, lblDateWidth, lblDate.Frame.Height);
                    }

                    segment.AddSubviews(new UIView[] { lblConsumption, viewBar, lblDate });

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
                        if (OnSegmentTap != null)
                        {
                            OnSegmentTap.Invoke(index);
                        }
                    }));
                    if (ConsumptionState == RMkWhEnum.RM || (ConsumptionState == RMkWhEnum.kWh && !item.DPCIndicator))
                    {
                        UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                        , () =>
                        {
                            viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                            lblConsumption.Frame = new CGRect(lblConsumption.Frame.X, amtYLoc, lblConsumption.Frame.Width, lblConsumption.Frame.Height);
                        }
                        , () => { }
                        );
                    }
                    else
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                        lblConsumption.Frame = new CGRect(lblConsumption.Frame.X, amtYLoc, lblConsumption.Frame.Width, lblConsumption.Frame.Height);
                        viewBar.Hidden = true;
                        lblConsumption.Hidden = true;
                        dpcIcon.Hidden = false;
                        dpcIcon.Alpha = isSelected ? 1 : 0.5F;
                    }
                }
            }
            if (LoadTariffLegendWithIndex != null)
            {
                LoadTariffLegendWithIndex.Invoke(usageData.Count - 1);
            }
        }
    }
}
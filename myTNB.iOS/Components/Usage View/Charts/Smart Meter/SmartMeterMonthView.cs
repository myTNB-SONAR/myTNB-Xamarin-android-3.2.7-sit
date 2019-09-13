using System;
using System.Collections.Generic;
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
        public Action<UIPinchGestureRecognizer> PinchAction { set; private get; }

        public override void CreateSegment(ref CustomUIView view)
        {
            base.CreateSegment(ref view);
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 16)
               , _width, GetHeightByScreenSize(157)));

            view.AddGestureRecognizer(new UIPinchGestureRecognizer((obj) =>
            {
                if (PinchAction != null)
                {
                    PinchAction.Invoke(obj);
                }
            }));

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
            List<string> valueList = usageData.Select(x => x.UsageTotal).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;

            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                bool isLatestBar = false;// index == usageData.Count - 1;
                bool isSelected = index < usageData.Count - 1;
                MonthItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapSmartMeterMonthBar"
                };
                view.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.UsageTotal, out double value);
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
                    viewBar.Layer.BorderColor = (isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White).CGColor;
                }

                nfloat coverWidth = isLatestBar ? viewBar.Frame.Width - GetWidthByScreenSize(6) : viewBar.Frame.Width;
                nfloat coverXLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
                nfloat coverHeight = isLatestBar ? barHeight - GetHeightByScreenSize(6) : barHeight;
                nfloat coverYLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;

                UIView viewCover = new UIView(new CGRect(new CGPoint(coverXLoc, coverYLoc), new CGSize(coverWidth, coverHeight)))
                {
                    BackgroundColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 2001,
                    Hidden = IsTariffView
                };
                if (isLatestBar) { viewCover.Layer.CornerRadius = coverWidth / 2; }
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size, isLatestBar);
                }
                nfloat amtYLoc = yLoc - amountBarMargin - lblHeight;
                UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetWidthByScreenSize(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = item.AmountTotal.FormatAmountString(item.Currency),
                    Hidden = isSelected,
                    Tag = 1002
                };
                nfloat lblAmountWidth = lblAmount.GetLabelWidth(GetWidthByScreenSize(100));
                lblAmount.Frame = new CGRect((segmentWidth - lblAmountWidth) / 2, lblAmount.Frame.Y, lblAmountWidth, lblAmount.Frame.Height);

                UILabel lblDate = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, segment.Frame.Height - lblHeight
                    , GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300,
                    TextColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = string.IsNullOrEmpty(item.Year) ? item.Month : string.Format(Format_Value, item.Month, item.Year),
                    Tag = 1003
                };
                segment.AddSubviews(new UIView[] { lblAmount, viewBar, lblDate });

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (OnSegmentTap != null)
                    {
                        OnSegmentTap.Invoke(index);
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
        }
    }
}

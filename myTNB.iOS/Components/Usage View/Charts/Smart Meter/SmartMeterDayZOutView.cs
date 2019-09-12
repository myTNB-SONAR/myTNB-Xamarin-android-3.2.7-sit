using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class SmartMeterDayView : BaseSmartMeterView
    {
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public Action<CustomUIView, List<TariffItemModel>, double, bool, CGSize, bool> AddTariffBlocks { set; private get; }

        public override void CreateSegment(ref CustomUIView view)
        {
            view = new CustomUIView(new CGRect(0, 0, _width, GetHeightByScreenSize(157)));

            nfloat height = view.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(18);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat lblHeight = GetHeightByScreenSize(14);
            nfloat maxBarHeight = GetHeightByScreenSize(108);
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            nfloat segmentWidth = GetWidthByScreenSize(30);
            nfloat barMargin = GetWidthByScreenSize(9);

            // List<MonthItemModel> usageData = AccountUsageCache.ByMonthUsage;
            List<DayItemModel> usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;
            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
                bool isSelected = false;// index < usageData.Count - 1;
                DayItemModel item = usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapNormalBar"
                };
                view.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.Amount, out double value);
                nfloat barHeight = (nfloat)(divisor * value); //GetHeightByScreenSize(96);//(nfloat)(divisor * value);
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
                    BackgroundColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Tag = 2001,
                    Hidden = false
                };
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks(viewBar, item.tariffBlocks, value, index == usageData.Count - 1, viewCover.Frame.Size, false);
                }

                nfloat amtYLoc = 0;// yLoc - amountBarMargin - lblHeight;
                UILabel lblAmount = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetWidthByScreenSize(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                   // Text = item.Amount.FormatAmountString(),
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
                    Text = "6",//string.IsNullOrEmpty(item.Year) ? item.Month : string.Format(Format_Value, item.Month, item.Year),
                    Tag = 1003
                };

                segment.AddSubviews(new UIView[] {  viewBar });

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

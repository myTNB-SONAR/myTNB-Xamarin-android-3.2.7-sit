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

        public CGRect ReferenceWidget { set; private get; }
        public Action<CustomUIView, List<TariffItemModel>, double, bool, CGSize, bool> AddTariffBlocks { set; private get; }

        public override void CreateSegment(ref CustomUIView view)
        {
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 24)
               , _width, GetHeightByScreenSize(149)));

            view.Layer.BorderColor = UIColor.Red.CGColor;
            view.Layer.BorderWidth = 1;

            nfloat height = view.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(4);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(117);
            nfloat segmentWidth = GetWidthByScreenSize(5);
            nfloat barMargin = GetWidthByScreenSize(5);

            List<DayItemModel> usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;
            for (int i = 0; i < usageData.Count; i++)
            {
                int index = i;
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
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = maxBarHeight - barHeight;

                CustomUIView viewBar = new CustomUIView(new CGRect(0, maxBarHeight, segmentWidth, 0))
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = 1001,
                    ClipsToBounds = true
                };
                viewBar.Layer.CornerRadius = segmentWidth / 2;
                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = UIColor.White,
                    Tag = 2001,
                    Hidden = false
                };
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, true, viewCover.Frame.Size, false);
                }

                segment.AddSubview(viewBar);

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                    }
                    , () => { }
                );
            }
        }
    }
}

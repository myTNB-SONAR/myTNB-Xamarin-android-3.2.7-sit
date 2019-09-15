using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class SmartMeterDayZInView : BaseSmartMeterView
    {
        private UIScrollView _segmentScrollView;
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        public override void CreateSegment(ref CustomUIView view)
        {
            _segmentScrollView = new UIScrollView(new CGRect(0, 0, _width, GetHeightByScreenSize(169)));
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 6)
               , _width, GetHeightByScreenSize(169)));
            view.AddSubview(_segmentScrollView);

            nfloat height = view.Frame.Height;
            nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(4);
            nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(117);
            nfloat segmentWidth = GetWidthByScreenSize(5);
            nfloat barMargin = GetWidthByScreenSize(5);
            nfloat missingReadingBarMargin = GetHeightByScreenSize(10);

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
                _segmentScrollView.AddSubview(segment);
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

                UIImageView imgMissingReading = null;
                if (item.IsEstimatedReading || index == 20)//For Testing
                {
                    imgMissingReading = new UIImageView(new CGRect(0, maxBarHeight - missingReadingBarMargin, segmentWidth, segmentWidth))
                    {
                        Image = UIImage.FromBundle(SmartMeterConstants.IMG_MissingReading),
                        Tag = 3001
                    };
                    segment.AddSubview(imgMissingReading);
                }

                UIView viewCover = new UIView(new CGRect(new CGPoint(0, 0), new CGSize(viewBar.Frame.Width, barHeight)))
                {
                    BackgroundColor = UIColor.White,
                    Tag = 2001,
                    Hidden = IsTariffView
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
                        if (imgMissingReading != null)
                        {
                            imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X, yLoc - missingReadingBarMargin)
                                , imgMissingReading.Frame.Size);
                        }
                    }
                    , () => { }
                );
            }
        }
    }
}
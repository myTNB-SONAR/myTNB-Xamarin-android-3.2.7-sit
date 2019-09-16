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
        private UIView _viewSegmentContainer;
        private nfloat _width = UIScreen.MainScreen.Bounds.Width;

        private void AddIndicator(ref CustomUIView view)
        {
            nfloat width = GetWidthByScreenSize(12);
            nfloat height = width / 2;
            UIImageView imgIndicator = new UIImageView(new CGRect((_width - width) / 2, view.Frame.Height - height, width, height))
            {
                Image = UIImage.FromBundle("Usage-Chart-Indicator")
            };
            view.AddSubview(imgIndicator);
        }

        private void AddScrollView(ref CustomUIView view)
        {
            _segmentScrollView = new UIScrollView(new CGRect(0, 0, _width, view.Frame.Height - GetWidthByScreenSize(6))) { Tag = 4000};
            view.AddSubview(_segmentScrollView);

            _viewSegmentContainer = new UIView(new CGRect(0, 0, _width, view.Frame.Height - GetWidthByScreenSize(6)));
            //_segmentScrollView.AddSubview(_viewSegmentContainer);

            _segmentScrollView.Layer.BorderColor = UIColor.Red.CGColor;
            _segmentScrollView.Layer.BorderWidth = 1;
        }

        public override void CreateSegment(ref CustomUIView view)
        {
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 6)
               , _width, GetHeightByScreenSize(169)));
            AddScrollView(ref view);
            AddIndicator(ref view);

            nfloat height = view.Frame.Height;
            //nfloat width = GetWidthByScreenSize(12);
            nfloat segmentMargin = GetWidthByScreenSize(24);//4
            //nfloat baseMargin = GetWidthByScreenSize(25);
            nfloat xLoc = 0;// baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(96);
            nfloat segmentWidth = GetWidthByScreenSize(12);
            //nfloat barMargin = GetWidthByScreenSize(5);
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

                segment.Layer.BorderColor = UIColor.Yellow.CGColor;
                segment.Layer.BorderWidth = 1;

                _segmentScrollView.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.Amount, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = GetHeightByScreenSize(44) + maxBarHeight - barHeight;

                CustomUIView viewBar = new CustomUIView(new CGRect(0, GetHeightByScreenSize(44) + maxBarHeight, segmentWidth, 0))
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

            nfloat contentWidth = usageData.Count * (GetWidthByScreenSize(12) + GetWidthByScreenSize(24));
            _segmentScrollView.ContentSize = new CGSize(contentWidth, _segmentScrollView.Frame.Height);
        }
    }
}
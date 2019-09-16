using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private nfloat _contentWidth;
        private CGPoint _currentPoint;

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
            _segmentScrollView = new UIScrollView(new CGRect(0, 0, _width, view.Frame.Height - GetWidthByScreenSize(10))) { Tag = 4000 };
            _segmentScrollView.Scrolled += OnBarScroll;

            view.AddSubview(_segmentScrollView);

            _segmentScrollView.Layer.BorderColor = UIColor.Red.CGColor;
            _segmentScrollView.Layer.BorderWidth = 1;
        }

        private void OnBarScroll(object sender, EventArgs e)
        {
            nfloat baseMargin = (_width / 2) - GetWidthByScreenSize(6);
            nfloat xOffset = _segmentScrollView.ContentOffset.X;
            if (xOffset > _contentWidth - (baseMargin * 2) - GetWidthByScreenSize(12))
            {
                //Snap to last?
                Debug.WriteLine("xOffset >");
                return;
            }
            if (xOffset < 0)
            {
                //Snap to first?
                Debug.WriteLine("xOffset <");
                return;
            }

            Debug.WriteLine("xOffset: " + xOffset);
            Debug.WriteLine("_currentPoint.X: " + _currentPoint.X);

            nfloat barDelta = GetWidthByScreenSize(30);
            nfloat delta = xOffset - _currentPoint.X;
            Debug.WriteLine("delta: " + delta);
            return;
            if (delta > GetWidthByScreenSize(6))
            {
                Debug.WriteLine("Right");
                _currentPoint = new CGPoint(_currentPoint.X + barDelta, _currentPoint.Y);
                _segmentScrollView.SetContentOffset(_currentPoint, true);
            }
            else if (delta < -GetWidthByScreenSize(6))
            {
                Debug.WriteLine("Left");
                _currentPoint = new CGPoint(_currentPoint.X - barDelta, _currentPoint.Y);
                _segmentScrollView.SetContentOffset(_currentPoint, true);
            }


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
            nfloat baseMargin = (_width / 2) - GetWidthByScreenSize(6);
            nfloat xLoc = baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(96);
            nfloat segmentWidth = GetWidthByScreenSize(12);
            //nfloat barMargin = GetWidthByScreenSize(5);
            nfloat missingReadingBarMargin = GetHeightByScreenSize(10);
            nfloat lblHeight = GetHeightByScreenSize(10);
            nfloat amountBarMargin = GetHeightByScreenSize(4);

            List<DayItemModel> usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;
            CGPoint lastSegment = new CGPoint();
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

                if (index == usageData.Count - 1)
                {
                    lastSegment = segment.Frame.Location;
                }

                segment.Layer.BorderColor = UIColor.Yellow.CGColor;
                segment.Layer.BorderWidth = 1;

                _segmentScrollView.AddSubview(segment);
                xLoc += segmentWidth + segmentMargin;

                double.TryParse(item.Amount, out double value);
                nfloat barHeight = (nfloat)(divisor * value);
                nfloat yLoc = GetHeightByScreenSize(34) + maxBarHeight - barHeight;

                CustomUIView viewBar = new CustomUIView(new CGRect(0, GetHeightByScreenSize(34) + maxBarHeight, segmentWidth, 0))
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
                    Hidden = IsTariffView
                };
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, true, viewCover.Frame.Size, false);
                }
                segment.AddSubview(viewBar);

                string displayText = ConsumptionState == RMkWhEnum.RM ? item.Amount.FormatAmountString(TNBGlobal.UNIT_CURRENCY) :
                    string.Format(Format_Value, item.Consumption, TNBGlobal.UNITENERGY);
                nfloat consumptionYLoc = yLoc - amountBarMargin - lblHeight;
                UILabel lblConsumption = new UILabel(new CGRect(0, viewBar.Frame.GetMinY() - amountBarMargin - lblHeight
                    , GetWidthByScreenSize(100), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.White,
                    Text = displayText,
                    Hidden = false,
                    Tag = 1002
                };
                nfloat lblAmountWidth = lblConsumption.GetLabelWidth(GetWidthByScreenSize(100));
                lblConsumption.Frame = new CGRect((segmentWidth - lblAmountWidth) / 2
                    , lblConsumption.Frame.Y, lblAmountWidth, lblConsumption.Frame.Height);
                segment.AddSubview(lblConsumption);

                UIImageView imgMissingReading = null;
                if (item.IsEstimatedReading || index == 3)//For Testing
                {
                    imgMissingReading = new UIImageView(new CGRect(0
                        , lblConsumption.Frame.GetMinY() - segmentWidth - amountBarMargin, segmentWidth, segmentWidth))
                    {
                        Image = UIImage.FromBundle(SmartMeterConstants.IMG_MissingReading),
                        Tag = 3001
                    };
                    segment.AddSubview(imgMissingReading);
                }

                UILabel lblDay = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, height - (lblHeight * 3)
                  , GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.White,
                    Text = item.Day,
                    Tag = 1003
                };
                segment.AddSubview(lblDay);

                UILabel lblMonth = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, height - (lblHeight * 2)
                 , GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = UIColor.White,
                    Text = item.Month
                };
                segment.AddSubview(lblMonth);

                UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
                    , () =>
                    {
                        viewBar.Frame = new CGRect(viewBar.Frame.X, yLoc, viewBar.Frame.Width, barHeight);
                        lblConsumption.Frame = new CGRect(lblConsumption.Frame.X, consumptionYLoc, lblConsumption.Frame.Width, lblConsumption.Frame.Height);
                        if (imgMissingReading != null)
                        {
                            imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X, lblConsumption.Frame.GetMinY() - segmentWidth - amountBarMargin)
                                , imgMissingReading.Frame.Size);
                        }
                    }
                    , () => { }
                );
            }

            _contentWidth = (usageData.Count * GetWidthByScreenSize(12)) + ((usageData.Count - 1) * GetWidthByScreenSize(24)) + (baseMargin * 2);
            _segmentScrollView.ContentSize = new CGSize(_contentWidth, _segmentScrollView.Frame.Height);
            _currentPoint = new CGPoint(_contentWidth - (baseMargin * 2) - GetWidthByScreenSize(12), lastSegment.Y);
            _segmentScrollView.SetContentOffset(_currentPoint, true);
        }
    }
}
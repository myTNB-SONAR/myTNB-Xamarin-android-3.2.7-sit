using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Force.DeepCloner;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class SmartMeterDayZInView : BaseSmartMeterView
    {
        internal UIScrollView _segmentScrollView;
        internal nfloat _width = UIScreen.MainScreen.Bounds.Width;
        internal nfloat _contentWidth;
        internal CGPoint _currentPoint;
        internal CGPoint _refPoint;
        internal CGPoint _lastSegment;
        internal nfloat _lastXContentOffset;
        internal Dictionary<nint, CGPoint> _locationDictionary = new Dictionary<nint, CGPoint>();
        internal nint _currentBar;

        private UILabel _lblMonth;
        private List<DayItemModel> _usageData = new List<DayItemModel>();

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
            _segmentScrollView = new UIScrollView(new CGRect(0, 0, _width, view.Frame.Height - GetWidthByScreenSize(18))) { Tag = 4000 };
            _segmentScrollView.Delegate = new BarScrollDelegate(this);
            _segmentScrollView.ShowsHorizontalScrollIndicator = false;
            view.AddSubview(_segmentScrollView);

            _lblMonth = new UILabel(new CGRect(0, _segmentScrollView.Frame.GetMaxY() - GetHeightByScreenSize(3), _width, GetHeightByScreenSize(12)))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White
            };
            view.AddSubview(_lblMonth);

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

            _usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = _usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxBarHeight / maxValue;
            _lastSegment = new CGPoint();
            _locationDictionary.Clear();
            for (int i = 0; i < _usageData.Count; i++)
            {
                int index = i;
                bool isSelected = index < _usageData.Count - 1;
                DayItemModel item = _usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapNormalBar"
                };
                _currentBar = segment.Tag;
                if (!_locationDictionary.ContainsKey(segment.Tag))
                {
                    _locationDictionary.Add(segment.Tag, segment.Frame.Location);
                }

                if (index == _usageData.Count - 1)
                {
                    _lastSegment = segment.Frame.Location;
                }

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
                    BackgroundColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
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
                    Hidden = isSelected,
                    Tag = 1002
                };
                nfloat lblConsumptionWidth = lblConsumption.GetLabelWidth(GetWidthByScreenSize(100));
                lblConsumption.Frame = new CGRect((segmentWidth - lblConsumptionWidth) / 2
                    , lblConsumption.Frame.Y, lblConsumptionWidth, lblConsumption.Frame.Height);
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

                UILabel lblDay = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2, height - ((lblHeight * 2) + GetHeightByScreenSize(12))
                  , GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = isSelected ? UIColor.FromWhiteAlpha(1, 0.50F) : UIColor.White,
                    Text = item.Day,
                    Tag = 1003
                };
                segment.AddSubview(lblDay);

                if (isSelected)
                {
                    _lblMonth.Text = item.Month ?? string.Empty;
                }

                segment.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnBarTapped(index);
                }));

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

            _contentWidth = (_usageData.Count * GetWidthByScreenSize(12)) + ((_usageData.Count - 1) * GetWidthByScreenSize(24)) + (baseMargin * 2);
            _segmentScrollView.ContentSize = new CGSize(_contentWidth, _segmentScrollView.Frame.Height);
            _currentPoint = new CGPoint(_contentWidth - (baseMargin * 2) - GetWidthByScreenSize(12), _lastSegment.Y);
            _refPoint = _currentPoint;
            _lastXContentOffset = _currentPoint.X;
            _segmentScrollView.SetContentOffset(_currentPoint, true);
        }

        private void OnBarTapped(int tag)
        {
            if (_locationDictionary.ContainsKey(tag))
            {
                CGPoint point = _locationDictionary[tag];
                nfloat baseMargin = (_width / 2) - GetWidthByScreenSize(6);

                point.X = point.X - baseMargin;
                _segmentScrollView.SetContentOffset(point, true);

                UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
                selectionFeedback.Prepare();
                selectionFeedback.ImpactOccurred();

                if (tag != _currentBar)
                {
                    UpdateBarsOnScroll(tag);
                }
            }
        }

        internal void UpdateBarsOnScroll(nint key)
        {
            SetBar(key, true);
            SetBar(_currentBar, false);
            _currentBar = key;
            string month = _usageData[(int)key].Month ?? string.Empty;
            _lblMonth.Text = month;
        }

        private void SetBar(nint key, bool isActive)
        {
            CustomUIView view = _segmentScrollView.ViewWithTag(key) as CustomUIView;
            if (view == null) { return; }

            UIView viewCover = view.ViewWithTag(2001) as UIView;
            if (viewCover != null)
            {
                viewCover.BackgroundColor = isActive ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
            }

            UILabel lblConsumption = view.ViewWithTag(1002) as UILabel;
            if (lblConsumption != null)
            {
                lblConsumption.Hidden = !isActive;
            }

            UILabel lblDay = view.ViewWithTag(1003) as UILabel;
            if (lblDay != null)
            {
                lblDay.TextColor = isActive ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
            }
        }
    }

    public class BarScrollDelegate : UIScrollViewDelegate
    {
        private SmartMeterDayZInView _controller;
        public BarScrollDelegate(SmartMeterDayZInView controller)
        {
            _controller = controller;
        }
        public override void Scrolled(UIScrollView scrollView)
        {
            nfloat baseMargin = (_controller._width / 2) - _controller.GetWidthByScreenSize(6);
            nfloat xOffset = _controller._segmentScrollView.ContentOffset.X;
            if (xOffset < 0 || xOffset > _controller._contentWidth - (baseMargin * 2) - _controller.GetWidthByScreenSize(12))
            {
                return;
            }
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (!willDecelerate)
            {
                Debug.WriteLine("DraggingEnded Decelerate");
                SetContentOffset();
            }
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            Debug.WriteLine("DecelerationEnded");
            SetContentOffset();
        }

        private void SetContentOffset()
        {
            nfloat baseMargin = (_controller._width / 2) - _controller.GetWidthByScreenSize(6);
            nfloat xOffset = _controller._segmentScrollView.ContentOffset.X + baseMargin + _controller.GetWidthByScreenSize(6);
            List<CGPoint> values = _controller._locationDictionary.Values.ToList();

            CGPoint closest = values.OrderBy(v => Math.Abs((nfloat)v.X - xOffset)).First();

            nint key = _controller._locationDictionary.FirstOrDefault(x => x.Value == closest).Key;

            nfloat barDelta = _controller.GetWidthByScreenSize(30);
            closest.X = closest.X - baseMargin;
            _controller._segmentScrollView.SetContentOffset(closest, true);

            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();

            if (key != _controller._currentBar)
            {
                _controller.UpdateBarsOnScroll(key);
            }
        }
    }
}

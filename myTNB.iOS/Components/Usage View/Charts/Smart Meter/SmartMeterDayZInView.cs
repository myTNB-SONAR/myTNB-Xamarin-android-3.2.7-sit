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
        internal UIScrollView _segmentScrollView;
        internal nfloat _width = UIScreen.MainScreen.Bounds.Width;
        internal nfloat _contentWidth;
        internal CGPoint _currentPoint;
        internal Dictionary<nint, CGPoint> _locationDictionary = new Dictionary<nint, CGPoint>();
        internal nint _currentBar;
        internal nfloat _baseMargin;

        private UILabel _lblMonth;
        private List<DayItemModel> _usageData = new List<DayItemModel>();

        private void AddIndicator(ref CustomUIView view)
        {
            nfloat width = GetWidthByScreenSize(12);
            nfloat height = width / 2;
            UIImageView imgIndicator = new UIImageView(new CGRect((_width - width) / 2, view.Frame.Height - height, width, height))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_ChartIndicator)
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
            _baseMargin = (_width / 2) - GetWidthByScreenSize(6);
            view = new CustomUIView(new CGRect(0, GetYLocationFromFrameScreenSize(ReferenceWidget, 6)
               , _width, GetHeightByScreenSize(169)));
            AddScrollView(ref view);
            AddIndicator(ref view);

            nfloat height = view.Frame.Height;
            nfloat segmentMargin = GetWidthByScreenSize(24);
            nfloat xLoc = _baseMargin;
            nfloat maxBarHeight = GetHeightByScreenSize(96);
            nfloat segmentWidth = GetWidthByScreenSize(12);
            nfloat missingReadingBarMargin = GetHeightByScreenSize(10);
            nfloat lblHeight = GetHeightByScreenSize(10);
            nfloat amountBarMargin = GetHeightByScreenSize(4);

            _usageData = AccountUsageSmartCache.FlatDays;
            List<string> valueList = _usageData.Select(x => x.Amount).ToList();
            double maxValue = GetMaxValue(RMkWhEnum.RM, valueList);
            double divisor = maxValue == 0 ? 0 : maxBarHeight / maxValue;
            CGPoint lastSegment = new CGPoint();
            _locationDictionary.Clear();
            for (int i = 0; i < _usageData.Count; i++)
            {
                int index = i;
                bool isSelected = index == _usageData.Count - 1;
                DayItemModel item = _usageData[index];
                CustomUIView segment = new CustomUIView(new CGRect(xLoc, 0, segmentWidth, height))
                {
                    Tag = index,
                    PageName = "InnerDashboard",
                    EventName = "OnTapSmartMeterDayBar"
                };
                _currentBar = segment.Tag;
                if (!_locationDictionary.ContainsKey(segment.Tag))
                {
                    _locationDictionary.Add(segment.Tag, segment.Frame.Location);
                }

                if (index == _usageData.Count - 1)
                {
                    lastSegment = segment.Frame.Location;
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
                    BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F),
                    Tag = 2001,
                    Hidden = IsTariffView
                };
                viewBar.AddSubview(viewCover);
                if (AddTariffBlocks != null)
                {
                    AddTariffBlocks.Invoke(viewBar, item.tariffBlocks, value, isSelected, viewCover.Frame.Size, false);
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
                    Hidden = !isSelected,
                    Tag = 1002
                };
                nfloat lblConsumptionWidth = lblConsumption.GetLabelWidth(GetWidthByScreenSize(100));
                lblConsumption.Frame = new CGRect((segmentWidth - lblConsumptionWidth) / 2
                    , lblConsumption.Frame.Y, lblConsumptionWidth, lblConsumption.Frame.Height);
                segment.AddSubview(lblConsumption);

                UIImageView imgMissingReading = null;
                if (item.IsMissingReading)
                {
                    nfloat imgYLoc = (isSelected ? lblConsumption.Frame.GetMinY() : viewBar.Frame.GetMinY()) - segmentWidth - amountBarMargin;
                    imgMissingReading = new UIImageView(new CGRect(0
                        , imgYLoc, segmentWidth, segmentWidth))
                    {
                        Image = UIImage.FromBundle(SmartMeterConstants.IMG_MissingReading),
                        Tag = 3001,
                        Alpha = isSelected ? 1 : 0.5F
                    };
                    segment.AddSubview(imgMissingReading);
                }

                UILabel lblDay = new UILabel(new CGRect((segmentWidth - GetWidthByScreenSize(40)) / 2
                    , height - ((lblHeight * 2) + GetHeightByScreenSize(12)), GetWidthByScreenSize(40), lblHeight))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_10_300,
                    TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F),
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
                        lblConsumption.Frame = new CGRect(lblConsumption.Frame.X, consumptionYLoc
                            , lblConsumption.Frame.Width, lblConsumption.Frame.Height);
                        if (imgMissingReading != null)
                        {
                            nfloat imgYLoc = (isSelected ? lblConsumption.Frame.GetMinY() : viewBar.Frame.GetMinY()) - segmentWidth - amountBarMargin;
                            imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X, imgYLoc)
                                , imgMissingReading.Frame.Size);
                        }
                    }
                    , () => { }
                );
            }

            _contentWidth = (_usageData.Count * GetWidthByScreenSize(12)) + ((_usageData.Count - 1) * GetWidthByScreenSize(24)) + (_baseMargin * 2);
            _segmentScrollView.ContentSize = new CGSize(_contentWidth, _segmentScrollView.Frame.Height);
            _currentPoint = new CGPoint(_contentWidth - (_baseMargin * 2) - GetWidthByScreenSize(12), lastSegment.Y);
            _segmentScrollView.SetContentOffset(_currentPoint, true);
            nfloat zoomScale = _segmentScrollView.ZoomScale;
            _segmentScrollView.ZoomScale = 0.1F;
            nfloat zoomScale2 = _segmentScrollView.ZoomScale;
        }

        private void OnBarTapped(int tag)
        {
            if (_locationDictionary.ContainsKey(tag))
            {
                CGPoint point = _locationDictionary[tag];
                nfloat baseMargin = (_width / 2) - GetWidthByScreenSize(6);

                point.X -= baseMargin;
                _segmentScrollView.SetContentOffset(point, true);
            }
        }

        internal void UpdateBarsOnScroll(nint key)
        {
            SetBar(key, true);
            SetBar(_currentBar, false);
            _currentBar = key;
            string month = _usageData[(int)key].Month ?? string.Empty;
            _lblMonth.Text = month;
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
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

            CustomUIView viewBar = view.ViewWithTag(1001) as CustomUIView;

            UIImageView imgMissing = view.ViewWithTag(3001) as UIImageView;
            if (imgMissing != null)
            {
                nfloat segmentWidth = GetWidthByScreenSize(12);
                nfloat amountBarMargin = GetHeightByScreenSize(4);
                nfloat minY = 0;
                if (viewBar != null)
                {
                    minY = viewBar.Frame.GetMinY();
                }
                nfloat imgYLoc = (isActive ? lblConsumption.Frame.GetMinY() : minY) - segmentWidth - amountBarMargin;
                CGRect frame = imgMissing.Frame;
                frame.Y = imgYLoc;
                imgMissing.Frame = frame;
                imgMissing.Alpha = isActive ? 1 : 0.5F;
            }
            if (viewBar != null)
            {
                UIView viewTariff = viewBar.ViewWithTag(2002);
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
                            nfloat alpha = isActive ? 1F : 0.5F;
                            tBlock.BackgroundColor = new UIColor(components[0], components[1], components[2], alpha);
                        }
                    }
                }
            }
        }
    }

    public class BarScrollDelegate : UIScrollViewDelegate
    {
        private readonly SmartMeterDayZInView _controller;
        public BarScrollDelegate(SmartMeterDayZInView controller)
        {
            _controller = controller;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            nfloat xOffset = _controller._segmentScrollView.ContentOffset.X;
            if (xOffset < 0 || xOffset > _controller._contentWidth - (_controller._baseMargin * 2) - _controller.GetWidthByScreenSize(12))
            {
                return;
            }

            SetContentOffset(true);
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (!willDecelerate)
            {
                SetContentOffset();
            }
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            SetContentOffset();
        }

        private void SetContentOffset(bool isScrolling = false)
        {
            nfloat xOffset = _controller._segmentScrollView.ContentOffset.X + _controller._baseMargin + _controller.GetWidthByScreenSize(6);
            List<CGPoint> values = _controller._locationDictionary.Values.ToList();

            CGPoint closest = values.OrderBy(v => Math.Abs((nfloat)v.X - xOffset)).First();

            nint key = _controller._locationDictionary.FirstOrDefault(x => x.Value == closest).Key;

            closest.X -= _controller._baseMargin;
            if (!isScrolling)
            {
                _controller._segmentScrollView.SetContentOffset(closest, true);
            }

            if (key != _controller._currentBar)
            {
                _controller.UpdateBarsOnScroll(key);
            }
        }
    }
}
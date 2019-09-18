using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using myTNB.SmartMeterView;
using UIKit;

namespace myTNB
{
    public class SmartMeterChartView : BaseChartView
    {
        public SmartMeterChartView()
        {
            ShimmerHeight = GetHeightByScreenSize(229);
        }

        private BaseSmartMeterView _baseSmartMeterView;
        private bool _isTariffView;
        private RMkWhEnum _consumptionState;
        private SmartMeterConstants.SmartMeterViewType _viewType;
        private CustomUIView _viewLine;
        private UIImageView _pinchIcon;

        protected override void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetWidthByScreenSize(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetHeightByScreenSize(229)));

            UIView toggleView = GetToggleView(_mainView);
            _mainView.AddSubview(toggleView);

            _lblDateRange = new UILabel(new CGRect(_baseMargin, toggleView.Frame.GetMaxY() + GetScaledHeight(24), _baseMarginedWidth, GetHeightByScreenSize(16)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.ButterScotch,
                Font = TNBFont.MuseoSans_12_500,
                Text = AccountUsageSmartCache.ByMonthDateRange
            };

            _viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrameScreenSize(_lblDateRange.Frame, 141)
               , _baseMarginedWidth, GetHeightByScreenSize(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, _viewLine });
            CreateSegment(SmartMeterConstants.SmartMeterViewType.Month);
        }

        protected void PinchAction(UIPinchGestureRecognizer obj)
        {
            if (_viewType != SmartMeterConstants.SmartMeterViewType.Month)
            {
                Debug.WriteLine("PinchAction");
                Debug.WriteLine("obj.Scale=== " + obj.Scale);
                nfloat pinchScale = obj.Scale;
                if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZOut && pinchScale > 1)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZIn);
                    Debug.WriteLine("ZOUT");
                }
                else if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn && pinchScale < 1)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZOut);
                    Debug.WriteLine("ZIN");
                }
            }
            else
            {
                Debug.WriteLine("Month View: " + obj.Scale);
            }
        }

        private UIView GetToggleView(CustomUIView parentView)
        {
            UIView toggleView = new UIView(new CGRect(0, 0, parentView.Frame.Width, GetScaledHeight(26)))
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat toggleWidth = GetScaledWidth(122);
            nfloat toggleHeight = GetScaledHeight(26);

            UITextAttributes attr = new UITextAttributes();
            attr.Font = TNBFont.MuseoSans_12_300;
            attr.TextColor = UIColor.White;
            UITextAttributes attrSelected = new UITextAttributes();
            attrSelected.Font = TNBFont.MuseoSans_12_300;
            attrSelected.TextColor = MyTNBColor.DarkPeriwinkle;

            UISegmentedControl toggleBar = new UISegmentedControl(new CGRect(GetXLocationToCenterObject(toggleWidth, parentView), 1, toggleWidth, toggleHeight));
            toggleBar.InsertSegment(LanguageUtility.GetCommonI18NValue(Constants.I18N_Day), 0, false);
            toggleBar.InsertSegment(LanguageUtility.GetCommonI18NValue(Constants.I18N_Month), 1, false);
            toggleBar.TintColor = UIColor.White;
            toggleBar.SetTitleTextAttributes(attr, UIControlState.Normal);
            toggleBar.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            toggleBar.Layer.CornerRadius = toggleHeight / 2;
            toggleBar.Layer.BorderColor = UIColor.White.CGColor;
            toggleBar.Layer.BorderWidth = GetScaledHeight(1);
            toggleBar.Layer.MasksToBounds = true;
            toggleBar.SelectedSegment = 1;
            toggleBar.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("toggleBar.SelectedSegment: " + toggleBar.SelectedSegment);
                SmartMeterConstants.SmartMeterViewType smartMeterViewType = default;
                if (toggleBar.SelectedSegment == 0)
                {
                    smartMeterViewType = SmartMeterConstants.SmartMeterViewType.DayZOut;
                }
                else
                {
                    smartMeterViewType = SmartMeterConstants.SmartMeterViewType.Month;
                }
                CreateSegment(smartMeterViewType);
            };
            toggleView.AddSubview(toggleBar);
            nfloat iconWidth = GetScaledWidth(24);
            nfloat iconHeight = GetScaledHeight(24);
            _pinchIcon = new UIImageView(new CGRect(toggleBar.Frame.GetMaxX() + GetScaledWidth(59), 0, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_PinchOut),
                UserInteractionEnabled = true
            };
            _pinchIcon.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("pinchIcon..");
                if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZOut)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZIn);
                    _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchIn);
                    Debug.WriteLine("ZOUT");

                }
                else if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZOut);
                    _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchOut);
                    Debug.WriteLine("ZIN");
                }
            }));
            toggleView.AddSubview(_pinchIcon);
            return toggleView;
        }

        public override CustomUIView GetShimmerUI()
        {
            nfloat baseWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            CustomShimmerView shimmeringView = new CustomShimmerView();
            CustomUIView parentView = new CustomUIView(new CGRect(BaseMarginWidth16, 0
                , baseWidth - (BaseMarginWidth16 * 2), ShimmerHeight))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            parentView.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView toggleView = GetToggleView(parentView);
            parentView.AddSubview(toggleView);

            UIView viewShDate = new UIView(new CGRect(GetWidthByScreenSize(82), toggleView.Frame.GetMaxY() + GetScaledHeight(24)
                , parentView.Frame.Width - GetWidthByScreenSize(164), GetHeightByScreenSize(14)))
            {
                BackgroundColor = new UIColor(red: 0.75f, green: 0.85f, blue: 0.95f, alpha: 0.25f)
            };
            viewShDate.Layer.CornerRadius = GetScaledHeight(2f);
            nfloat viewShChartYPos = GetYLocationFromFrameScreenSize(viewShDate.Frame, 24);
            UIView viewShChart = new UIView(new CGRect(0, viewShChartYPos
                , parentView.Frame.Width, ShimmerHeight - viewShChartYPos))
            { BackgroundColor = new UIColor(red: 0.75f, green: 0.85f, blue: 0.95f, alpha: 0.25f) };
            viewShChart.Layer.CornerRadius = GetScaledHeight(5f);

            viewShimmerContent.AddSubviews(new UIView[] { viewShDate, viewShChart });
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();
            return parentView;
        }

        protected override void CreateSegment(SmartMeterConstants.SmartMeterViewType viewType)
        {
            _viewType = viewType;
            if (_pinchIcon != null)
            {
                _pinchIcon.Hidden = _viewType == SmartMeterConstants.SmartMeterViewType.Month;
            }
            if (_viewLine != null)
            {
                CGRect lineFrame = _viewLine.Frame;
                lineFrame.Width = _viewType == SmartMeterConstants.SmartMeterViewType.DayZIn ? _width : _baseMarginedWidth;
                lineFrame.X = _viewType == SmartMeterConstants.SmartMeterViewType.DayZIn ? 0 : _baseMargin;
                _viewLine.Frame = lineFrame;
            }
            if (_segmentContainer != null)
            {
                _segmentContainer.RemoveFromSuperview();
            }
            if (viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                _baseSmartMeterView = new SmartMeterMonthView()
                {
                    OnSegmentTap = OnSegmentTap,
                };
            }
            else if (viewType == SmartMeterConstants.SmartMeterViewType.DayZOut)
            {
                _baseSmartMeterView = new SmartMeterDayZOutView();
            }
            else
            {
                _baseSmartMeterView = new SmartMeterDayZInView();
            }
            _baseSmartMeterView.PinchAction = PinchAction;
            _baseSmartMeterView.IsTariffView = _isTariffView;
            _baseSmartMeterView.ReferenceWidget = _lblDateRange.Frame;
            _baseSmartMeterView.AddTariffBlocks = AddTariffBlocks;
            _baseSmartMeterView.ConsumptionState = _consumptionState;
            _baseSmartMeterView.CreateSegment(ref _segmentContainer);

            _segmentContainer.AddGestureRecognizer(new UIPinchGestureRecognizer((obj) =>
            {
                PinchAction(obj);
            }));

            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size, bool isLatestBar)
        {
            if (viewBar == null || tariffList == null || tariffList.Count == 0 || baseValue == 0) { return; }
            nfloat baseHeigt = size.Height;
            nfloat barMaxY = size.Height;
            nfloat xLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
            nfloat yLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;
            UIView viewTariffContainer = new UIView(new CGRect(xLoc, yLoc, size.Width, size.Height))
            {
                Tag = 2002,
                Hidden = !_isTariffView,
                ClipsToBounds = true
            };
            if (isLatestBar) { viewTariffContainer.Layer.CornerRadius = size.Width / 2; }
            for (int i = 0; i < tariffList.Count; i++)
            {
                TariffItemModel item = tariffList[i];
                double val = item.Usage;
                if (val == 0) { continue; }
                nfloat percentage = (nfloat)(val / baseValue); // if 0/0
                nfloat blockHeight = baseHeigt * percentage;
                barMaxY -= blockHeight;
                UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, size.Width, blockHeight))
                {
                    BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected, true)
                };
                viewTariffContainer.AddSubview(viewTariffBlock);
                barMaxY -= GetHeightByScreenSize(1);
            }
            viewBar.AddSubview(viewTariffContainer);
        }

        protected override void OnSegmentTap(int index)
        {
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API

            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                bool isLatestBar = i == _segmentContainer.Subviews.Count() - 1;
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (isLatestBar)
                {
                    bar.Layer.BorderColor = (isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F)).CGColor;
                }
                if (bar != null)
                {
                    UIView viewCover = bar.ViewWithTag(2001);
                    if (viewCover != null) { viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }
                    UIView viewTariff = bar.ViewWithTag(2002);
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
                                nfloat alpha = isSelected ? 1F : 0.5F;
                                tBlock.BackgroundColor = new UIColor(components[0], components[1], components[2], alpha);
                            }
                        }
                    }
                }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value != null)
                {
                    value.TextColor = isLatestBar || isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                    value.Hidden = isLatestBar ? false : !isSelected;
                }
                UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                if (date != null)
                {
                    date.TextColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F);
                    date.Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300;
                }
            }
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            _isTariffView = isTariffView;
            if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn)
            {
                UIScrollView scrollview = _segmentContainer.ViewWithTag(4000) as UIScrollView;
                if (scrollview == null) { return; }
                for (int i = 0; i < scrollview.Subviews.Count(); i++)
                {
                    CustomUIView segmentView = scrollview.Subviews[i] as CustomUIView;
                    if (segmentView == null) { continue; }
                    UpdateTariffView(segmentView);
                }
            }
            else
            {
                for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
                {
                    CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                    if (segmentView == null) { continue; }
                    UpdateTariffView(segmentView);
                }
            }
        }

        private void UpdateTariffView(CustomUIView segmentView)
        {
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
            if (bar == null) { return; }

            CGRect barOriginalFrame = bar.Frame;
            bar.Frame = new CGRect(bar.Frame.X, bar.Frame.GetMaxY(), bar.Frame.Width, 0);

            UIView viewCover = bar.ViewWithTag(2001);
            if (viewCover != null) { viewCover.Hidden = _isTariffView; }

            UIView viewTariff = bar.ViewWithTag(2002);
            if (viewTariff != null) { viewTariff.Hidden = !_isTariffView; }

            UILabel value = segmentView.ViewWithTag(1002) as UILabel;
            CGRect valueOriginalFrame = new CGRect();
            if (value != null)
            {
                valueOriginalFrame = value.Frame;
                value.Frame = new CGRect(value.Frame.X, bar.Frame.GetMinY() - amountBarMargin - value.Frame.Height
                    , value.Frame.Width, value.Frame.Height);
            }

            UIImageView imgMissingReading = segmentView.ViewWithTag(3001) as UIImageView;
            CGRect imgMissingReadingOriginalFrame = new CGRect();
            if (imgMissingReading != null)
            {
                imgMissingReadingOriginalFrame = imgMissingReading.Frame;
                nfloat yRef = bar.Frame.GetMinY() - GetHeightByScreenSize(10);
                if (value != null && !value.Hidden)
                {
                    yRef = value.Frame.GetMinY() - GetHeightByScreenSize(10);
                }
                imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X, yRef), imgMissingReading.Frame.Size);
            }

            UIView.Animate(1, 0.3, UIViewAnimationOptions.CurveEaseOut
               , () =>
               {
                   bar.Frame = barOriginalFrame;
                   if (value != null)
                   {
                       value.Frame = valueOriginalFrame;
                   }
                   if (imgMissingReading != null)
                   {
                       imgMissingReading.Frame = imgMissingReadingOriginalFrame;
                   }
               }
               , () => { }
           );
        }

        public override void ToggleRMKWHValues(RMkWhEnum state)
        {
            _consumptionState = state;
            if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn)
            {
                UIScrollView scrollview = _segmentContainer.ViewWithTag(4000) as UIScrollView;
                if (scrollview == null) { return; }
                List<DayItemModel> usageData = AccountUsageSmartCache.FlatDays;
                for (int i = 0; i < scrollview.Subviews.Count(); i++)
                {
                    int index = i;
                    CustomUIView segmentView = scrollview.Subviews[index] as CustomUIView;
                    if (segmentView == null || index >= usageData.Count) { continue; }
                    string usageText = _consumptionState == RMkWhEnum.RM ? usageData[index].Amount.FormatAmountString(TNBGlobal.UNIT_CURRENCY)
                        : string.Format(Format_Value, usageData[index].Consumption, TNBGlobal.UNITENERGY);
                    UpdateRMKWHValues(segmentView, usageText);
                }
            }
            else if (_viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
                for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
                {
                    int index = i;
                    CustomUIView segmentView = _segmentContainer.Subviews[index] as CustomUIView;
                    if (segmentView == null || index >= usageData.Count) { continue; }
                    string usageText = _consumptionState == RMkWhEnum.RM ? usageData[index].AmountTotal.FormatAmountString(usageData[index].Currency)
                        : string.Format(Format_Value, usageData[index].UsageTotal, usageData[index].UsageUnit);
                    UpdateRMKWHValues(segmentView, usageText);
                }
            }
        }

        private void UpdateRMKWHValues(CustomUIView segmentView, string usageText)
        {
            UILabel value = segmentView.ViewWithTag(1002) as UILabel;
            if (value == null) { return; }
            value.Text = usageText;
            nfloat lblAmountWidth = value.GetLabelWidth(GetWidthByScreenSize(200));
            nfloat baseX = GetWidthByScreenSize(_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn ? 12 : 30);
            value.Frame = new CGRect((baseX - lblAmountWidth) / 2, value.Frame.Y, lblAmountWidth, value.Frame.Height);
        }
    }
}
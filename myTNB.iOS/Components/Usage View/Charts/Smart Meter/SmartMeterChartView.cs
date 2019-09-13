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

            CustomUIView viewLine = new CustomUIView(new CGRect(_baseMargin, GetYLocationFromFrameScreenSize(_lblDateRange.Frame, 150)
                , _baseMarginedWidth, GetHeightByScreenSize(1)))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.30F) };

            _mainView.AddSubviews(new UIView[] { _lblDateRange, viewLine });
            CreateSegment(SmartMeterConstants.SmartMeterViewType.Month);
        }

        protected void PinchAction(UIPinchGestureRecognizer obj)
        {
            Debug.WriteLine("PinchAction");
            Debug.WriteLine("obj.Scale=== " + obj.Scale);
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
            UIImageView pinchIcon = new UIImageView(new CGRect(toggleBar.Frame.GetMaxX() + GetScaledWidth(59), 0, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle("Pinch-Icon"),
                UserInteractionEnabled = true
            };
            pinchIcon.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("pinchIcon..");
            }));
            toggleView.AddSubview(pinchIcon);
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
            if (_segmentContainer != null)
            {
                _segmentContainer.RemoveFromSuperview();
            }
            if (viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                _baseSmartMeterView = new SmartMeterMonthView()
                {
                    OnSegmentTap = OnSegmentTap,
                    PinchAction = PinchAction,
                };
            }
            else
            {
                _baseSmartMeterView = new SmartMeterDayView();
            }
            _baseSmartMeterView.IsTariffView = _isTariffView;
            _baseSmartMeterView.ReferenceWidget = _lblDateRange.Frame;
            _baseSmartMeterView.AddTariffBlocks = AddTariffBlocks;
            _baseSmartMeterView.CreateSegment(ref _segmentContainer);
            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size, bool isLatestBar)
        {
            if (viewBar == null || tariffList == null || tariffList.Count == 0) { return; }
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
                double.TryParse(item.Usage, out double val);
                nfloat percentage = (nfloat)(val / baseValue);
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
            UIImpactFeedbackGenerator selectionFeedback = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            selectionFeedback.Prepare();
            selectionFeedback.ImpactOccurred();
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
                if (isLatestBar)
                {
                    Debug.WriteLine("Todo: Go to day view.");
                }
            }
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            _isTariffView = isTariffView;
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }

                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (bar == null) { continue; }

                CGRect barOriginalFrame = bar.Frame;
                bar.Frame = new CGRect(bar.Frame.X, bar.Frame.GetMaxY(), bar.Frame.Width, 0);

                UIImageView imgMissingReading = segmentView.ViewWithTag(3001) as UIImageView;
                CGRect imgMissingReadingOriginalFrame = new CGRect();
                if (imgMissingReading != null)
                {
                    imgMissingReadingOriginalFrame = imgMissingReading.Frame;
                    imgMissingReading.Frame = new CGRect(new CGPoint(imgMissingReading.Frame.X
                        , bar.Frame.GetMidY() - GetHeightByScreenSize(10)), imgMissingReading.Frame.Size);
                }

                UIView viewCover = bar.ViewWithTag(2001);
                if (viewCover != null) { viewCover.Hidden = isTariffView; }

                UIView viewTariff = bar.ViewWithTag(2002);
                if (viewTariff != null) { viewTariff.Hidden = !isTariffView; }

                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                CGRect valueOriginalFrame = new CGRect();
                if (value != null)
                {
                    valueOriginalFrame = value.Frame;
                    value.Frame = new CGRect(value.Frame.X, bar.Frame.GetMinY() - amountBarMargin - value.Frame.Height
                        , value.Frame.Width, value.Frame.Height);
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
        }

        public override void ToggleRMKWHValues(RMkWhEnum state)
        {
            List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
            for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
            {
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                UILabel value = segmentView.ViewWithTag(1002) as UILabel;
                if (value == null) { continue; }
                value.Text = state == RMkWhEnum.RM ? usageData[i].AmountTotal.FormatAmountString(usageData[i].Currency)
                    : string.Format(Format_Value, usageData[i].UsageTotal, usageData[i].UsageUnit);
                nfloat lblAmountWidth = value.GetLabelWidth(GetWidthByScreenSize(200));
                value.Frame = new CGRect((GetWidthByScreenSize(30) - lblAmountWidth) / 2, value.Frame.Y, lblAmountWidth, value.Frame.Height);
            }
        }
    }
}

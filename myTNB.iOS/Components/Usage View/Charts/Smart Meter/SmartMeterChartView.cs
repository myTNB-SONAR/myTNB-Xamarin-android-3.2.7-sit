using System;
using System.Collections.Generic;
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

        public Action PinchOverlayAction { set; private get; }
        public Action<List<string>> LoadTariffLegendWithBlockIds { set; private get; }
        public Action OnMDMSIconTap { set; private get; }
        public Action<bool> SetDPCNoteForMDMSDown { set; private get; }
        public Action OnMDMSRefresh { set; private get; }
        public Action DisableTariffButton { set; private get; }
        public Action SetTariffButtonState { set; private get; }
        public Action<bool> SetRMKwHButtonState { set; private get; }

        private BaseSmartMeterView _baseSmartMeterView;
        private bool _isTariffView;
        private RMkWhEnum _consumptionState;
        private SmartMeterConstants.SmartMeterViewType _viewType;
        private CustomUIView _viewLine;
        private UIImageView _pinchIcon;
        private bool _isOverlayDisplayed;
        private bool _isDataReceived;
        private List<string> _availableTariffBlockIDList = new List<string>();
        private int _selectedIndex = -1;

        protected override void CreatUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            _baseMargin = GetWidthByScreenSize(16);
            _baseMarginedWidth = _width - (_baseMargin * 2);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, GetHeightByScreenSize(229)));

            UIView toggleView = GetToggleView(_mainView);
            _mainView.AddSubview(toggleView);

            _lblDateRange = new UILabel(new CGRect(_baseMargin, toggleView.Frame.GetMaxY() + GetScaledHeight(12)
                , _baseMarginedWidth, GetHeightByScreenSize(16)))
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
#pragma warning disable XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
            if (_viewType != SmartMeterConstants.SmartMeterViewType.Month)
            {
                nfloat pinchScale = obj.Scale;
                if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZOut && pinchScale > 1)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZIn);
                    _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchIn);
                }
                else if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn && pinchScale < 1)
                {
                    CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZOut);
                    _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchOut);
                }
            }
#pragma warning restore XI0003 // Notifies you when using a deprecated, obsolete or unavailable Apple API
        }

        private UIView GetToggleView(CustomUIView parentView)
        {
            UIView toggleView = new UIView(new CGRect(0, 0, parentView.Frame.Width, GetScaledHeight(26)))
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat toggleWidth = GetScaledWidth(DeviceHelper.IsIOS13AndUp ? 114 : 122);
            nfloat toggleHeight = GetScaledHeight(26);

            UITextAttributes attr = new UITextAttributes
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = UIColor.White
            };
            UITextAttributes attrSelected = new UITextAttributes
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.DarkPeriwinkle
            };

            _toggleBar = new UISegmentedControl(new CGRect(GetXLocationToCenterObject(toggleWidth, parentView)
                , 1, toggleWidth, toggleHeight)) { ClipsToBounds = false };
            CustomUIView segment1View = new CustomUIView();
            CustomUIView segment2View = new CustomUIView();
            if (DeviceHelper.IsIOS13AndUp)
            {
                CustomUIView toggleBarView = new CustomUIView(new CGRect(-GetScaledWidth(4), 0
                    , toggleWidth + GetScaledWidth(8), toggleHeight)) { ClipsToBounds = true };
                toggleBarView.Layer.CornerRadius = toggleHeight / 2;
                toggleBarView.Layer.BorderColor = UIColor.White.CGColor;
                toggleBarView.Layer.BorderWidth = GetScaledHeight(1);
                _toggleBar.AddSubview(toggleBarView);
                _toggleBar.SendSubviewToBack(toggleBarView);
                segment1View = new CustomUIView(new CGRect(0, 0, toggleBarView.Frame.Width / 2, toggleHeight));
                segment2View = new CustomUIView(new CGRect(segment1View.Frame.GetMaxX(), 0, toggleBarView.Frame.Width / 2, toggleHeight));
                toggleBarView.AddSubviews(new CustomUIView[] { segment1View, segment2View });
                toggleBarView.SendSubviewToBack(segment1View);
                toggleBarView.SendSubviewToBack(segment2View);
            }

            _toggleBar.InsertSegment(GetCommonI18NValue(UsageConstants.I18N_Day), 0, false);
            _toggleBar.InsertSegment(GetCommonI18NValue(UsageConstants.I18N_Month), 1, false);
            _toggleBar.TintColor = DeviceHelper.IsIOS13AndUp ? UIColor.Clear : UIColor.White;

            _toggleBar.SetTitleTextAttributes(attr, UIControlState.Normal);
            _toggleBar.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            if (!DeviceHelper.IsIOS13AndUp)
            {
                _toggleBar.Layer.CornerRadius = toggleHeight / 2;
                _toggleBar.Layer.BorderColor = UIColor.White.CGColor;
                _toggleBar.Layer.BorderWidth = GetScaledHeight(1);
                _toggleBar.Layer.MasksToBounds = true;
            }

            if (DeviceHelper.IsIOS13AndUp)
            {
                _toggleBar.SelectedSegment = 0;
            }
            _toggleBar.SelectedSegment = 1;
            UpdateSegmentBackground(segment1View, segment2View);

            _toggleBar.ValueChanged += (sender, e) =>
            {
                if (_isDataReceived)
                {
                    SmartMeterConstants.SmartMeterViewType smartMeterViewType;
                    UpdateSegmentBackground(segment1View, segment2View);
                    if (_toggleBar.SelectedSegment == 0)
                    {
                        if (PinchOverlayAction != null && !_isOverlayDisplayed && !AccountUsageSmartCache.IsMDMSDown)
                        {
                            PinchOverlayAction?.Invoke();
                            _isOverlayDisplayed = true;
                        }

                        smartMeterViewType = SmartMeterConstants.SmartMeterViewType.DayZOut;
                        if (SetDPCNoteForMDMSDown != null)
                        {
                            SetDPCNoteForMDMSDown.Invoke(true);
                        }

                        if (AccountUsageSmartCache.IsMDMSDown)
                        {
                            if (DisableTariffButton != null)
                            {
                                DisableTariffButton.Invoke();
                            }
                        }

                        if (SetRMKwHButtonState != null)
                        {
                            SetRMKwHButtonState.Invoke(AccountUsageSmartCache.IsMDMSDown);
                        }

                        _lblDateRange.Hidden = AccountUsageSmartCache.IsMDMSDown;
                    }
                    else
                    {
                        smartMeterViewType = SmartMeterConstants.SmartMeterViewType.Month;
                        if (SetDPCNoteForMDMSDown != null)
                        {
                            SetDPCNoteForMDMSDown.Invoke(false);
                        }
                        if (SetTariffButtonState != null)
                        {
                            SetTariffButtonState.Invoke();
                        }
                        if (SetRMKwHButtonState != null)
                        {
                            SetRMKwHButtonState.Invoke(false);
                        }
                        _lblDateRange.Hidden = false;
                    }
                    CreateSegment(smartMeterViewType);
                    if (_toggleBar.SelectedSegment == 0)
                    {
                        if (LoadTariffLegendWithBlockIds != null)
                        {
                            LoadTariffLegendWithBlockIds.Invoke(AvailableTariffBlockIDList);
                        }
                    }
                }
            };
            toggleView.AddSubview(_toggleBar);
            nfloat iconWidth = GetScaledWidth(24);
            nfloat iconHeight = GetScaledHeight(24);
            _pinchIcon = new UIImageView(new CGRect(_toggleBar.Frame.GetMaxX() + GetScaledWidth(59), 0, iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(UsageConstants.IMG_PinchOut),
                UserInteractionEnabled = true,
                Hidden = true
            };
            _pinchIcon.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (!AccountUsageSmartCache.IsMDMSDown)
                {
                    if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZOut)
                    {
                        CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZIn);
                        _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchIn);
                    }
                    else if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn)
                    {
                        CreateSegment(SmartMeterConstants.SmartMeterViewType.DayZOut);
                        _pinchIcon.Image = UIImage.FromBundle(UsageConstants.IMG_PinchOut);
                    }
                }
            }));
            toggleView.AddSubview(_pinchIcon);
            return toggleView;
        }

        private void UpdateSegmentBackground(CustomUIView segment1View, CustomUIView segment2View)
        {
            if (DeviceHelper.IsIOS13AndUp && segment1View != null && segment2View != null)
            {
                segment1View.BackgroundColor = _toggleBar.SelectedSegment == 0 ? UIColor.White : MyTNBColor.DarkPeriwinkle;
                segment2View.BackgroundColor = _toggleBar.SelectedSegment == 1 ? UIColor.White : MyTNBColor.DarkPeriwinkle;
            }
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

            UIView viewShDate = new UIView(new CGRect(GetWidthByScreenSize(82), toggleView.Frame.GetMaxY() + GetScaledHeight(12)
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

        private void SetDateRange(SmartMeterConstants.SmartMeterViewType viewType)
        {
            if (viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                _lblDateRange.Text = AccountUsageSmartCache.ByMonthDateRange ?? string.Empty;
            }
            else
            {
                _lblDateRange.Text = AccountUsageSmartCache.DateRange ?? string.Empty;
            }
        }

        protected override void CreateSegment(SmartMeterConstants.SmartMeterViewType viewType)
        {
            _viewType = viewType;
            _isDataReceived = true;
            SetDateRange(_viewType);
            if (_pinchIcon != null)
            {
                _pinchIcon.Hidden = AccountUsageSmartCache.IsMDMSDown || _viewType == SmartMeterConstants.SmartMeterViewType.Month;
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
                    LoadTariffLegendWithIndex = OnBarSelected,
                    GetI18NValue = GetI18NValue
                };
            }
            else if (AccountUsageSmartCache.IsMDMSDown)
            {
                _baseSmartMeterView = new SmartMeterMDMSDownView()
                {
                    GetI18NValue = GetI18NValue,
                    OnMDMSRefresh = OnMDMSRefresh
                };
            }
            else if (viewType == SmartMeterConstants.SmartMeterViewType.DayZOut)
            {
                _baseSmartMeterView = new SmartMeterDayZOutView();
            }
            else
            {
                _baseSmartMeterView = new SmartMeterDayZInView()
                {
                    OnHighlightedBarTap = OnZoomInHighligtedBarTap
                };
            }
            _viewLine.Hidden = viewType != SmartMeterConstants.SmartMeterViewType.Month && AccountUsageSmartCache.IsMDMSDown;
            _baseSmartMeterView.PinchAction = PinchAction;
            _baseSmartMeterView.IsTariffView = _isTariffView;
            _baseSmartMeterView.ReferenceWidget = _lblDateRange.Frame;
            _baseSmartMeterView.AddTariffBlocks = AddTariffBlocks;
            _baseSmartMeterView.ConsumptionState = _consumptionState;
            _baseSmartMeterView.CreateSegment(ref _segmentContainer);

            if (!AccountUsageSmartCache.IsMDMSDown)
            {
                _segmentContainer.AddGestureRecognizer(new UIPinchGestureRecognizer((obj) =>
                {
                    PinchAction(obj);
                }));
            }

            _mainView.AddSubview(_segmentContainer);
        }

        protected override void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size, bool isLatestBar, bool isDPC)
        {
            //if (viewBar == null || tariffList == null || tariffList.Count == 0 || baseValue == 0) { return; }
            if (viewBar == null || baseValue == 0) { return; }
            nfloat baseHeigt = size.Height;
            nfloat barMaxY = size.Height;
            nfloat xLoc = isLatestBar ? GetWidthByScreenSize(3) : 0;
            nfloat yLoc = isLatestBar ? GetHeightByScreenSize(3) : 0;

            nfloat totalTariffValue = GetTotalTariff(tariffList);
            int tariffCount = GetTariffWithValueCount(tariffList);
            nfloat sharedMissingPercentage = 0;
            if (baseValue > 0 && baseValue > totalTariffValue)
            {
                double percentMissing = 1 - (totalTariffValue / baseValue);
                sharedMissingPercentage = (nfloat)(percentMissing / tariffCount);
            }

            UIView viewTariffContainer = new UIView(new CGRect(xLoc, yLoc, size.Width, size.Height))
            {
                Tag = 2002,
                Hidden = !_isTariffView || (_isTariffView && isDPC),
                ClipsToBounds = true,
                BackgroundColor = tariffCount > 0 ? UIColor.Clear : UIColor.White
            };
            if (_viewType != SmartMeterConstants.SmartMeterViewType.DayZOut)
            {
                viewTariffContainer.Alpha = isSelected ? 1F : 0.5F;
            }
            if (isLatestBar) { viewTariffContainer.Layer.CornerRadius = size.Width / 2; }

            if (tariffCount > 0 && !isDPC)
            {
                baseHeigt -= ((GetTariffWithValueCount(tariffList) - 1) * GetHeightByScreenSize(1));
                for (int i = 0; i < tariffList.Count; i++)
                {
                    TariffItemModel item = tariffList[i];
                    UpdateAvailableTariffList(item);
                    double val = IsAmountState ? item.Amount : item.Usage;
                    double percentage = (baseValue > 0 && val > 0) ? (nfloat)(val / baseValue) + sharedMissingPercentage : 0;
                    nfloat blockHeight = (nfloat)(baseHeigt * percentage);

                    barMaxY -= blockHeight;
                    UIView viewTariffBlock = new UIView(new CGRect(0, barMaxY, size.Width, blockHeight))
                    {
                        BackgroundColor = GetTariffBlockColor(item.BlockId, isSelected, true)
                    };
                    viewTariffContainer.AddSubview(viewTariffBlock);
                    barMaxY -= GetHeightByScreenSize(1);
                }
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
                bool isLatestBar = false;// i == _segmentContainer.Subviews.Count() - 1;
                CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                if (segmentView == null) { continue; }
                bool isSelected = segmentView.Tag == index;
                CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
                if (isLatestBar)
                {
                    if (bar != null)
                    {
                        bar.Layer.BorderColor = (isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F)).CGColor;
                    }
                    UILabel indicatorLabel = segmentView.ViewWithTag(1004) as UILabel;
                    if (indicatorLabel != null)
                    {
                        indicatorLabel.Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300;
                    }
                }
                if (bar != null)
                {
                    UIView viewCover = bar.ViewWithTag(2001);
                    UIView viewTariff = bar.ViewWithTag(2002);

                    if (_isTariffView && _viewType == SmartMeterConstants.SmartMeterViewType.Month)
                    {
                        List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
                        MonthItemModel item = usageData[i];
                        if (viewCover != null) { viewCover.Hidden = !item.DPCIndicator; }
                        if (viewTariff != null) { viewTariff.Hidden = item.DPCIndicator; }
                    }

                    if (viewCover != null) { viewCover.BackgroundColor = isSelected ? UIColor.White : UIColor.FromWhiteAlpha(1, 0.50F); }

                    if (viewTariff != null)
                    {
                        viewTariff.Alpha = isSelected ? 1F : 0.5F;
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
                    List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
                    MonthItemModel item = usageData[i];
                    if (_consumptionState == RMkWhEnum.RM || (_consumptionState == RMkWhEnum.kWh && !item.DPCIndicator))
                    {
                        value.Hidden = isLatestBar ? false : !isSelected;
                    }
                    else
                    {
                        value.Hidden = true;
                    }
                }
                UILabel date = segmentView.ViewWithTag(1003) as UILabel;
                if (date != null)
                {
                    date.Font = isSelected ? TNBFont.MuseoSans_10_500 : TNBFont.MuseoSans_10_300;
                    if (date.Text != null)
                    {
                        nfloat lblDateWidth = date.GetLabelWidth(GetWidthByScreenSize(100));
                        date.Frame = new CGRect((segmentView.Frame.Width - lblDateWidth) / 2, date.Frame.Y, lblDateWidth, date.Frame.Height);
                    }
                }

                if (_viewType == SmartMeterConstants.SmartMeterViewType.Month)
                {
                    UIImageView mdmsIcon = segmentView.ViewWithTag(1009) as UIImageView;
                    if (mdmsIcon != null)
                    {
                        mdmsIcon.Alpha = isSelected ? 1 : 0.5F;
                    }

                    UIImageView dpcIcon = segmentView.ViewWithTag(1005) as UIImageView;
                    if (dpcIcon != null)
                    {
                        dpcIcon.Alpha = isSelected ? 1 : 0.5F;
                    }
                }
            }

            bool isHighlighted = false;

            if (_viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
                int usageDataCount = usageData != null ? usageData.Count : 0;
                isHighlighted = usageDataCount > 0 && index == (usageDataCount - 1);
                if (usageDataCount > 0 && index == (usageDataCount - 1) && AccountUsageSmartCache.IsMDMSDown
                    && OnMDMSIconTap != null)
                {
                    OnMDMSIconTap.Invoke();
                }
            }

            OnBarSelected(index, isHighlighted);
            _selectedIndex = index;
        }

        private void OnBarSelected(int index, bool isHighlighted)
        {
            if (LoadTariffLegendWithIndex != null)
            {
                LoadTariffLegendWithIndex.Invoke(index, isHighlighted);
            }
        }

        private void OnZoomInHighligtedBarTap()
        {
            if (ShowMissedReadToolTip != null)
            {
                ShowMissedReadToolTip.Invoke();
            }
        }

        public override void ToggleTariffView(bool isTariffView)
        {
            _isTariffView = isTariffView;
            if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZIn)
            {
                if (_segmentContainer != null)
                {
                    UIScrollView scrollview = _segmentContainer.ViewWithTag(4000) as UIScrollView;
                    if (scrollview == null) { return; }
                    for (int i = 0; i < scrollview.Subviews.Count(); i++)
                    {
                        CustomUIView segmentView = scrollview.Subviews[i] as CustomUIView;
                        if (segmentView == null) { continue; }
                        UpdateTariffView(segmentView, i);
                    }
                }
            }
            else
            {
                if (_segmentContainer != null)
                {
                    for (int i = 0; i < _segmentContainer.Subviews.Count(); i++)
                    {
                        CustomUIView segmentView = _segmentContainer.Subviews[i] as CustomUIView;
                        if (segmentView == null) { continue; }
                        UpdateTariffView(segmentView, i);

                    }
                }
            }
        }

        private void UpdateTariffView(CustomUIView segmentView, int index)
        {
            nfloat amountBarMargin = GetHeightByScreenSize(4);
            CustomUIView bar = segmentView.ViewWithTag(1001) as CustomUIView;
            if (bar == null) { return; }

            CGRect barOriginalFrame = bar.Frame;
            bar.Frame = new CGRect(bar.Frame.X, bar.Frame.GetMaxY(), bar.Frame.Width, 0);

            UIView viewCover = bar.ViewWithTag(2001);
            UIView viewTariff = bar.ViewWithTag(2002);

            if (_isTariffView && _viewType == SmartMeterConstants.SmartMeterViewType.Month)
            {
                List<MonthItemModel> usageData = AccountUsageSmartCache.ByMonthUsage;
                if (index < usageData.Count)
                {
                    MonthItemModel item = usageData[index];
                    if (viewCover != null) { viewCover.Hidden = !usageData[index].DPCIndicator; }
                    if (viewTariff != null) { viewTariff.Hidden = usageData[index].DPCIndicator; }
                }
            }
            else
            {
                if (viewCover != null) { viewCover.Hidden = _isTariffView; }
                if (viewTariff != null) { viewTariff.Hidden = !_isTariffView; }
            }

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
            if (_segmentContainer == null) { return; }
            _consumptionState = state;
            CreateSegment(_viewType);

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
                    double.TryParse(usageData[index].Consumption, out double consumption);
                    string usageText = _consumptionState == RMkWhEnum.RM ? usageData[index].Amount.FormatAmountString(TNBGlobal.UNIT_CURRENCY)
                        : string.Format(Format_Value, consumption, Constants.UnitEnergy);
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
                    double usageTotal;
                    double.TryParse(usageData[index].UsageTotal, out usageTotal);
                    string usageText = _consumptionState == RMkWhEnum.RM ? usageData[index].AmountTotal.FormatAmountString(usageData[index].Currency)
                        : string.Format(Format_Value, usageTotal, usageData[index].UsageUnit);
                    UpdateRMKWHValues(segmentView, usageText);
                    if (usageData[index].DPCIndicator)
                    {
                        UpdateDPCIndicator(segmentView);
                    }
                }
            }
        }

        private void UpdateDPCIndicator(CustomUIView segmentView)
        {
            bool isSelected = false;// segmentView.Tag == _selectedIndex;
            CustomUIView viewBar = segmentView.ViewWithTag(1001) as CustomUIView;
            UIImageView dpcIcon = segmentView.ViewWithTag(1005) as UIImageView;
            UILabel lblConsumption = segmentView.ViewWithTag(1002) as UILabel;
            if (viewBar == null || dpcIcon == null || lblConsumption == null) { return; }
            viewBar.Hidden = _consumptionState == RMkWhEnum.kWh;
            dpcIcon.Hidden = _consumptionState == RMkWhEnum.RM;
            lblConsumption.Hidden = _consumptionState == RMkWhEnum.kWh || !isSelected;
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

        private void UpdateAvailableTariffList(TariffItemModel tariff)
        {
            if (_viewType != SmartMeterConstants.SmartMeterViewType.DayZOut) { return; }
            int index = _availableTariffBlockIDList.FindIndex(x => x == tariff.BlockId);
            if (index < 0)
            {
                _availableTariffBlockIDList.Add(tariff.BlockId);
            }
        }

        public override List<string> AvailableTariffBlockIDList
        {
            get
            {
                if (_viewType == SmartMeterConstants.SmartMeterViewType.DayZOut)
                {
                    return _availableTariffBlockIDList;
                }
                return new List<string>();
            }
        }
    }
}
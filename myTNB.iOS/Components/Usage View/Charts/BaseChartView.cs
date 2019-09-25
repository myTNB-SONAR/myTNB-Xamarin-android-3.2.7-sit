using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class BaseChartView : BaseComponent
    {
        public BaseChartView() { }
        protected CustomUIView _mainView, _segmentContainer, _zoomView;
        protected nfloat _width, _baseMargin, _baseMarginedWidth;
        protected UILabel _lblDateRange;
        protected UISegmentedControl _toggleBar;

        protected string Format_Value = "{0} {1}";
        protected nfloat ShimmerHeight;

        public virtual void ToggleTariffView(bool isTariffView) { }

        public virtual void ToggleRMKWHValues(RMkWhEnum state) { }
        public Action<int> PrepareTariffLegend;

        public virtual CustomUIView GetUI()
        {
            CreatUI();
            return _mainView;
        }

        public virtual CustomUIView GetShimmerUI()
        {
            nfloat baseWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            CustomShimmerView shimmeringView = new CustomShimmerView();
            CustomUIView parentView = new CustomUIView(new CGRect(BaseMarginWidth16, 0
                , baseWidth - (BaseMarginWidth16 * 2), ShimmerHeight))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            parentView.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView viewShDate = new UIView(new CGRect(GetWidthByScreenSize(82), 0
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

        protected virtual void CreatUI() { }

        protected virtual void CreateSegment() { }

        protected virtual void CreateSegment(SmartMeterView.SmartMeterConstants.SmartMeterViewType viewType) { }

        protected virtual double GetMaxValue(RMkWhEnum view, List<string> value)
        {
            double maxValue = 0;
            if (value != null &&
               value.Count > 0)
            {
                switch (view)
                {
                    case RMkWhEnum.kWh:
                        {
                            maxValue = value.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x)));
                            break;
                        }
                    case RMkWhEnum.RM:
                        {
                            maxValue = value.Max(x => Math.Abs(TextHelper.ParseStringToDouble(x)));
                            break;
                        }
                    default:
                        {
                            maxValue = 0;
                            break;
                        }
                }
            }
            return maxValue;
        }

        protected virtual void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size)
        { }

        protected virtual void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariffList
            , double baseValue, bool isSelected, CGSize size, bool isLatestBar)
        { }

        protected virtual UIColor GetTariffBlockColor(string blockID, bool isSelected, bool isSmartMeter)
        {
            List<LegendItemModel> legend = isSmartMeter ? AccountUsageSmartCache.GetTariffLegendList() : AccountUsageCache.GetTariffLegendList();
            LegendItemModel item = legend.Find(x => x.BlockId == blockID);
            if (item != null)
            {
                return new UIColor((nfloat)item.RGB.R / 255F, (nfloat)item.RGB.G / 255F, (nfloat)item.RGB.B / 255F, isSelected ? 1F : 0.5F);
            }
            return UIColor.White;
        }

        protected virtual void OnSegmentTap(int index) { }

        protected nfloat GetWidthByScreenSize(nfloat value)
        {
            return ScaleUtility.GetWidthByScreenSize(value);
        }

        protected nfloat GetHeightByScreenSize(nfloat value)
        {
            return ScaleUtility.GetHeightByScreenSize(value);
        }

        protected nfloat GetYLocationFromFrameScreenSize(CGRect frame, nfloat yValue)
        {
            return ScaleUtility.GetYLocationFromFrameScreenSize(frame, yValue);
        }
    }
}
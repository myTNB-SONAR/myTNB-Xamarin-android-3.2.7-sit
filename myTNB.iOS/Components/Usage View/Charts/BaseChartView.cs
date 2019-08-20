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
        public BaseChartView()
        {
        }
        protected CustomUIView _mainView, _segmentContainer;
        protected nfloat _width, _baseMargin, _baseMarginedWidth;
        protected UILabel _lblDateRange;
        protected string Format_Value = "{0} {1}";

        public virtual void ToggleTariffView(bool isTariffView) { }

        public virtual void ToggleRMKWHValues(RMkWhEnum state) { }

        public virtual CustomUIView GetUI()
        {
            CreatUI();
            return _mainView;
        }

        protected virtual void CreatUI() { }

        protected virtual void CreateSegment() { }

        protected virtual double GetMaxValue(RMkWhEnum view, List<string> value)
        {
            double maxValue = 0;
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
            return maxValue;
        }

        protected virtual void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariff
            , double baseValue, bool isSelected, CGSize size)
        { }

        protected virtual UIColor GetTariffBlockColor(string blockID, bool isSelected)
        {
            List<LegendItemModel> legend = AccountUsageCache.GetTariffLegendList();
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
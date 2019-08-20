using System;
using System.Collections.Generic;
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

        protected virtual double GetMaxValue(RMkWhEnum view, List<string> value) { return 0; }

        protected virtual void AddTariffBlocks(CustomUIView viewBar, List<TariffItemModel> tariff, double baseValue, bool isSelected) { }

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
    }
}
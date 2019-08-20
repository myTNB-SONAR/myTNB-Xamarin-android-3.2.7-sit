using System;
namespace myTNB
{
    public class BaseChartView : BaseComponent
    {
        public BaseChartView()
        {
        }

        public virtual void ToggleTariffView(bool isTariffView) { }

        public virtual void ToggleRMKWHValues(RMkWhEnum state) { }
    }
}

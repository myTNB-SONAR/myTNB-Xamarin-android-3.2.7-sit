using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB.SmartMeterView
{
    public class BaseSmartMeterView
    {
        protected string Format_Value = "{0:n0} {1}";

        public virtual void CreateSegment(ref CustomUIView view) { }
        public CGRect ReferenceWidget { set; protected get; }
        public Action<CustomUIView, List<TariffItemModel>, double, bool, CGSize, bool> AddTariffBlocks { set; protected get; }
        public Action<UIPinchGestureRecognizer> PinchAction { set; protected get; }
        public bool IsTariffView { set; protected get; } = false;
        public RMkWhEnum ConsumptionState { set; protected get; } = RMkWhEnum.kWh;
        public Func<string, string> GetI18NValue;

        protected virtual double GetMaxValue(RMkWhEnum view, List<string> value, List<bool> dpcIndicatorList = null)
        {
            if (dpcIndicatorList != null && view == RMkWhEnum.kWh)
            {
                for (int i = 0; i < dpcIndicatorList.Count; i++)
                {
                    if (dpcIndicatorList[i])
                    {
                        value[i] = "0";
                    }
                }
            }
            double maxValue = 0;
            if (value != null && value.Count > 0)
            {
                switch (view)
                {
                    case RMkWhEnum.kWh:
                        {
                            maxValue = value.Max(x => Math.Abs(ParseValue(x)));
                            break;
                        }
                    case RMkWhEnum.RM:
                        {
                            maxValue = value.Max(x => Math.Abs(ParseValue(x)));
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

        private double ParseValue(string val)
        {
            double parsedValue = TextHelper.ParseStringToDouble(val);
            if (parsedValue < 0)
            {
                parsedValue = 0;
            }
            return parsedValue;
        }

        public nfloat GetWidthByScreenSize(nfloat value)
        {
            return ScaleUtility.GetWidthByScreenSize(value);
        }

        public nfloat GetHeightByScreenSize(nfloat value)
        {
            return ScaleUtility.GetHeightByScreenSize(value);
        }

        public nfloat GetYLocationFromFrameScreenSize(CGRect frame, nfloat yValue)
        {
            return ScaleUtility.GetYLocationFromFrameScreenSize(frame, yValue);
        }

        public nfloat GetScaledWidth(nfloat value)
        {
            return ScaleUtility.GetScaledWidth(value);
        }

        public nfloat GetScaledHeight(nfloat value)
        {
            return ScaleUtility.GetScaledHeight(value);
        }

        public nfloat GetXLocationToCenterObject(nfloat width, UIView view = null)
        {
            return ScaleUtility.GetXLocationToCenterObject(width, view);
        }

        protected bool IsAmountState
        {
            get
            {
                return ConsumptionState == RMkWhEnum.RM;
            }
        }
    }
}
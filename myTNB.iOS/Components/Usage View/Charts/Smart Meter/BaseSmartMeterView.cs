﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace myTNB.SmartMeterView
{
    public class BaseSmartMeterView
    {
        protected string Format_Value = "{0} {1}";

        public virtual void CreateSegment(ref CustomUIView view) { }

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
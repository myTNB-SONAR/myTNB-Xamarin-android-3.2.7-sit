using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class BaseComponent
    {
        #region I18N Utilities
        public string GetCommonI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.CommonI18NDictionary != null
                && DataManager.DataManager.SharedInstance.CommonI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.CommonI18NDictionary[key] : string.Empty;
        }
        public string GetHintI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.HintI18NDictionary != null
                && DataManager.DataManager.SharedInstance.HintI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.HintI18NDictionary[key] : string.Empty;
        }
        public string GetErrorI18NValue(string key)
        {
            return DataManager.DataManager.SharedInstance.ErrorI18NDictionary != null
                && DataManager.DataManager.SharedInstance.ErrorI18NDictionary.ContainsKey(key)
                ? DataManager.DataManager.SharedInstance.ErrorI18NDictionary[key] : string.Empty;
        }
        #endregion

        #region Scale Utility
        public nfloat GetScaledWidth(nfloat value)
        {
            return ScaleUtility.GetScaledWidth(value);
        }
        public nfloat GetScaledHeight(nfloat value)
        {
            return ScaleUtility.GetScaledHeight(value);
        }
        public void GetYLocationFromFrame(CGRect frame, ref nfloat yValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
        }
        public void GetYLocationFromFrame(CGRect frame, nfloat yValue, out nfloat scaledValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
            scaledValue = yValue;
        }
        public nfloat GetYLocationFromFrame(CGRect frame, nfloat yValue)
        {
            ScaleUtility.GetYLocationFromFrame(frame, ref yValue);
            return yValue;
        }
        public nfloat GetYLocationToCenterObject(nfloat height, UIView view = null)
        {
            return ScaleUtility.GetYLocationToCenterObject(height, view);
        }
        public nfloat GetXLocationToCenterObject(nfloat width, UIView view = null)
        {
            return ScaleUtility.GetXLocationToCenterObject(width, view);
        }
        public void GetValuesFromAspectRatio(ref nfloat width, ref nfloat height)
        {
            ScaleUtility.GetValuesFromAspectRatio(ref width, ref height);
        }
        public static nfloat GetPercentHeightValue(CGRect frame, nfloat percentage)
        {
            return ScaleUtility.GetPercentHeightValue(frame, percentage);
        }
        public nfloat BaseMarginWidth8
        {
            get { return ScaleUtility.BaseMarginWidth8; }
        }
        public nfloat BaseMarginWidth12
        {
            get { return ScaleUtility.BaseMarginWidth12; }
        }
        public nfloat BaseMarginWidth16
        {
            get { return ScaleUtility.BaseMarginWidth16; }
        }
        public nfloat BaseMarginHeight16
        {
            get { return ScaleUtility.BaseMarginHeight16; }
        }
        public nfloat GetWidthByScreenSize(nfloat width)
        {
           return ScaleUtility.GetWidthByScreenSize(width);
        }
        #endregion
    }
}

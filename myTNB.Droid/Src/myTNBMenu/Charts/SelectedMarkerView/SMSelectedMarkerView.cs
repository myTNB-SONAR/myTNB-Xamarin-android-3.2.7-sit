using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Text;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Util;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView
{
    public class SMSelectedMarkerView : MarkerView
    {

        private DecimalFormat decimalFormat;
        private DecimalFormat kwhFormat;
        private DecimalFormat zoonInkWhFormat;
        private TextView titleMarker;
        private TextView titlekWhMarker;
        private ImageView imgMissingCopy;
        public SMUsageHistoryData UsageHistoryData { get; set; }
        public List<double> smDayViewCurrencyList { get; set; }
        public List<double> smDayViewUsageList { get; set; }
        public List<bool> smMissingList { get; set; }
        public string smDayCurrencyUnit { get; set; }
        public string smDayUsageUnit { get; set; }
        public bool isZoomIn { get; set; }
        public ChartDataType ChartDataType { get; set; }
        public ChartType ChartType { get; set; }
        public bool isMDMSDown { get; set; }
        public string AccountType { get; set; }
        public int CurrentParentIndex = -1;

        public Context currentContext;
        public SMSelectedMarkerView(Context context) : base(context, Resource.Layout.NewMarkerView)
        {
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            titlekWhMarker = FindViewById<TextView>(Resource.Id.txtkWhMarker);
            imgMissingCopy = FindViewById<ImageView>(Resource.Id.imgMissingCopy);
            titleMarker.TextSize = TextViewUtils.GetFontSize(11f);
            titlekWhMarker.TextSize = TextViewUtils.GetFontSize(11f);
            titleMarker.Gravity = GravityFlags.Center;
            titlekWhMarker.Gravity = GravityFlags.Center;
            TextViewUtils.SetMuseoSans500Typeface(titleMarker);
            TextViewUtils.SetMuseoSans300Typeface(titlekWhMarker);
            titlekWhMarker.Visibility = ViewStates.Gone;
            imgMissingCopy.Visibility = ViewStates.Gone;
            decimalFormat = new DecimalFormat("#,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
            kwhFormat = new DecimalFormat("#,###,##0", new DecimalFormatSymbols(Java.Util.Locale.Us));
            zoonInkWhFormat = new DecimalFormat("#,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
            currentContext = context;
        }

        protected SMSelectedMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        public override void RefreshContent(Entry e, Highlight highlight)
        {
            if (ChartType != null)
            {
                imgMissingCopy.Visibility = ViewStates.Gone;
                int index = (int)e.GetX();

                if (ChartType == ChartType.Month)
                {
                    bool isDisableWord = false;
                    if (isMDMSDown)
                    {
                        if (index == (UsageHistoryData.ByMonth.Months.Count - 1))
                        {
                            isDisableWord = true;
                        }
                        else if (UsageHistoryData.ByMonth.Months[index].DPCIndicator && ChartDataType == ChartDataType.kWh)
                        {
                            isDisableWord = true;
                        }
                        else
                        {
                            isDisableWord = false;
                        }
                    }
                    else if (UsageHistoryData.ByMonth.Months[index].DPCIndicator && ChartDataType == ChartDataType.kWh)
                    {
                        isDisableWord = true;
                    }
                    else
                    {
                        isDisableWord = false;
                    }

                    if (!isDisableWord)
                    {
                        titleMarker.Visibility = ViewStates.Visible;
                        if (ChartDataType == ChartDataType.RM)
                        {
                            titlekWhMarker.Visibility = ViewStates.Gone;
                            float val = (float)UsageHistoryData.ByMonth.Months[index].AmountTotal;
                            titleMarker.Text = ((val < 0.00f) ? "- " : "") + UsageHistoryData.ByMonth.Months[index].Currency + " " + decimalFormat.Format(Math.Abs(val));
                        }
                        else if (ChartDataType == ChartDataType.kWh)
                        {
                            titlekWhMarker.Visibility = ViewStates.Gone;
                            float valKwh = (float)UsageHistoryData.ByMonth.Months[index].UsageTotal;
                            titleMarker.Text = kwhFormat.Format(Math.Abs(valKwh)) + " " + UsageHistoryData.ByMonth.Months[index].UsageUnit;
                        }
                    }
                    else
                    {
                        titleMarker.Visibility = ViewStates.Gone;
                        titlekWhMarker.Visibility = ViewStates.Gone;
                    }
                }
                else if (ChartType == ChartType.Day && isZoomIn)
                {
                    titleMarker.Visibility = ViewStates.Visible;

                    if (smMissingList[index])
                    {
                        imgMissingCopy.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        imgMissingCopy.Visibility = ViewStates.Gone;
                    }

                    if (ChartDataType == ChartDataType.RM)
                    {
                        titlekWhMarker.Visibility = ViewStates.Gone;
                        float val = (float)smDayViewCurrencyList[index];
                        titleMarker.Text = ((val < 0.00f) ? "- " : "") + smDayCurrencyUnit + " " + decimalFormat.Format(Math.Abs(val));
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        titlekWhMarker.Visibility = ViewStates.Gone;
                        float valKwh = (float)smDayViewUsageList[index];
                        titleMarker.Text = kwhFormat.Format(Math.Round(Math.Abs(valKwh), MidpointRounding.AwayFromZero)) + " " + smDayUsageUnit;
                    }

                }
                else
                {
                    titleMarker.Visibility = ViewStates.Gone;
                    titlekWhMarker.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                titleMarker.Visibility = ViewStates.Visible;
                titlekWhMarker.Visibility = ViewStates.Gone;
                titleMarker.Text = "RM " + decimalFormat.Format(e.GetY());
            }


            base.RefreshContent(e, highlight);
        }

        public override MPPointF GetOffsetForDrawingAtPoint(float posX, float posY)
        {
            return new MPPointF(-(Width / 2), -(Height));
        }
    }
}
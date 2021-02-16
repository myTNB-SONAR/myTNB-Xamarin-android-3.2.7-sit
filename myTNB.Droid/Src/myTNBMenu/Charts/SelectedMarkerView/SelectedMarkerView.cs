using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Text;
using MikePhil.Charting.Buffer;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Interfaces.Datasets;
using MikePhil.Charting.Util;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom;
using System;
using System.Collections;
using System.Collections.Generic;

namespace myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView
{
    public class SelectedMarkerView : MarkerView
    {

        private DecimalFormat decimalFormat;
        private DecimalFormat kwhFormat;
        private TextView titleMarker;
        private TextView titlekWhMarker;
        public UsageHistoryData UsageHistoryData { get; set; }
        public ChartType ChartType { get; set; }
        public ChartDataType ChartDataType { get; set; }
        public string AccountType { get; set; }
        public int CurrentParentIndex = -1;
        
        public Context currentContext;
        public SelectedMarkerView(Context context) : base(context, Resource.Layout.NewMarkerView)
        {
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            titlekWhMarker = FindViewById<TextView>(Resource.Id.txtkWhMarker);
            titleMarker.Gravity = GravityFlags.Center;
            titlekWhMarker.Gravity = GravityFlags.Center;
            TextViewUtils.SetMuseoSans500Typeface(titleMarker);
            TextViewUtils.SetMuseoSans300Typeface(titlekWhMarker);
            titleMarker.TextSize = TextViewUtils.GetFontSize(11f);
            titlekWhMarker.TextSize = TextViewUtils.GetFontSize(11f);
            titlekWhMarker.Visibility = ViewStates.Gone;
            decimalFormat = new DecimalFormat("#,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
            kwhFormat = new DecimalFormat("#,###,##0", new DecimalFormatSymbols(Java.Util.Locale.Us));
            currentContext = context;
        }

        protected SelectedMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }



        public override void RefreshContent(Entry e, Highlight highlight)
        {
            if (ChartType != null)
            {
                int index = (int)e.GetX();
                if (ChartType == ChartType.Month)
                {
                    bool isDisableWord = false;

                    if (UsageHistoryData.ByMonth.Months[index].DPCIndicator && ChartDataType == ChartDataType.kWh && !AccountType.Equals("2"))
                    {
                        isDisableWord = true;
                    }
                    else
                    {
                        isDisableWord = false;
                    }

                    if (isDisableWord)
                    {
                        titleMarker.Visibility = ViewStates.Gone;
                        titlekWhMarker.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        titleMarker.Visibility = ViewStates.Visible;
                        if (ChartDataType == ChartDataType.RM)
                        {
                            if (AccountType.Equals("2"))
                            {
                                titlekWhMarker.Visibility = ViewStates.Visible;
                                float val = (float)UsageHistoryData.ByMonth.Months[index].AmountTotal;
                                float valKwh = (float)UsageHistoryData.ByMonth.Months[index].UsageTotal;
                                titleMarker.Text = UsageHistoryData.ByMonth.Months[index].Currency + " " + decimalFormat.Format(Math.Abs(val));
                                titlekWhMarker.Text = kwhFormat.Format(Math.Abs(valKwh)) + " " + UsageHistoryData.ByMonth.Months[index].UsageUnit;
                            }
                            else
                            {
                                titlekWhMarker.Visibility = ViewStates.Gone;
                                float val = (float)UsageHistoryData.ByMonth.Months[index].AmountTotal;
                                titleMarker.Text = ((val < 0.00f) ? "- " : "") + UsageHistoryData.ByMonth.Months[index].Currency + " " + decimalFormat.Format(Math.Abs(val));
                            }

                        }
                        else if (ChartDataType == ChartDataType.kWh)
                        {
                            titlekWhMarker.Visibility = ViewStates.Gone;
                            float valKwh = (float)UsageHistoryData.ByMonth.Months[index].UsageTotal;
                            titleMarker.Text = kwhFormat.Format(Math.Abs(valKwh)) + " " + UsageHistoryData.ByMonth.Months[index].UsageUnit;
                        }
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
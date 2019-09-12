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
using System;

namespace myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView
{
    public class SMSelectedMarkerView : MarkerView
    {

        private DecimalFormat decimalFormat;
        private DecimalFormat kwhFormat;
        private TextView titleMarker;
        private TextView titlekWhMarker;
        public SMUsageHistoryData UsageHistoryData { get; set; }
        public ChartDataType ChartDataType { get; set; }
        public ChartType ChartType { get; set; }
        public string AccountType { get; set; }
        public int CurrentParentIndex = -1;

        public Context currentContext;
        public SMSelectedMarkerView(Context context) : base(context, Resource.Layout.NewMarkerView)
        {
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            titlekWhMarker = FindViewById<TextView>(Resource.Id.txtkWhMarker);
            titleMarker.Gravity = GravityFlags.Center;
            titlekWhMarker.Gravity = GravityFlags.Center;
            TextViewUtils.SetMuseoSans500Typeface(titleMarker);
            TextViewUtils.SetMuseoSans300Typeface(titlekWhMarker);
            titlekWhMarker.Visibility = ViewStates.Gone;
            decimalFormat = new DecimalFormat("#,###,##0.00");
            kwhFormat = new DecimalFormat("#,###,##0");
            currentContext = context;
        }

        protected SMSelectedMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        public override void RefreshContent(Entry e, Highlight highlight)
        {
            if (ChartType != null)
            {
                int index = (int)e.GetX();
                if (ChartType == ChartType.Month)
                {
                    if (ChartDataType == ChartDataType.RM)
                    {
                        titlekWhMarker.Visibility = ViewStates.Gone;
                        float val = (float)UsageHistoryData.ByMonth.Months[index].AmountTotal;
                        titleMarker.Text = UsageHistoryData.ByMonth.Months[index].Currency + " " + decimalFormat.Format(val);
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
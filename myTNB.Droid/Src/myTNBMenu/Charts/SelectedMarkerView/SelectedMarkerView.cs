using Android.Content;
using Android.OS;
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
    public class SelectedMarkerView : MarkerView
    {

        private DecimalFormat decimalFormat;
        private DecimalFormat kwhFormat;
        private TextView titleMarker;
        private TextView titlekWhMarker;
        public UsageHistoryData UsageHistoryData { get; set; }
        public ChartType ChartType { get; set; }
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
            titlekWhMarker.Visibility = ViewStates.Gone;
            decimalFormat = new DecimalFormat("#,###,##0.00");
            kwhFormat = new DecimalFormat("#,###,##0");
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
                if (ChartType == ChartType.RM)
                {
                    if (AccountType.Equals("2"))
                    {
                        titlekWhMarker.Visibility = ViewStates.Visible;
                        float val = (float)UsageHistoryData.ByMonth.Months[index].Amount;
                        float valKwh = (float)UsageHistoryData.ByMonth.Months[index].Usage;
                        titleMarker.Text = "RM " + decimalFormat.Format(Math.Abs(val));
                        titlekWhMarker.Text = kwhFormat.Format(Math.Abs(valKwh)) + " kWh";
                    }
                    else
                    {
                        titlekWhMarker.Visibility = ViewStates.Gone;
                        float val = (float)UsageHistoryData.ByMonth.Months[index].Amount;
                        titleMarker.Text = "RM " + decimalFormat.Format(val);
                    }

                }
                else if (ChartType == ChartType.kWh)
                {
                    titlekWhMarker.Visibility = ViewStates.Gone;
                    float valKwh = (float)UsageHistoryData.ByMonth.Months[index].Usage;
                    titleMarker.Text = kwhFormat.Format(Math.Abs(valKwh)) + " kWh";
                }
                if (CurrentParentIndex == -1)
                {
                    CurrentParentIndex = index;
                }
                else
                {
                    if (index != CurrentParentIndex)
                    {
                        CurrentParentIndex = index;
                        Vibrator vibrator = (Vibrator)currentContext.GetSystemService(Context.VibratorService);
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.O)
                        {
                            vibrator.Vibrate(VibrationEffect.CreateOneShot(200, 12));

                        }
                        else
                        {
                            vibrator.Vibrate(200);

                        }
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
            return new MPPointF(-(Width / 2), (int) -(Height * 1.25));
        }
    }
}
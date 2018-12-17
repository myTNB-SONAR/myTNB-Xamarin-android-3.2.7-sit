using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Components;
using myTNB_Android.Src.myTNBMenu.Models;
using MikePhil.Charting.Data;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Util;
using Java.Text;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Charts.SelectedMarkerView
{
    public class SelectedMarkerView : MarkerView
    {

        private DecimalFormat decimalFormat;
        private DecimalFormat kwhFormat;
        private TextView titleMarker;
        public UsageHistoryData UsageHistoryData { get; set; }
        public ChartType ChartType { get; set; }
        public string AccountType { get; set;  }
        public int CurrentParentIndex = 0;
        public SelectedMarkerView(Context context) : base(context, Resource.Layout.MarkerView)
        {
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            titleMarker.Gravity = GravityFlags.Center;
            TextViewUtils.SetMuseoSans500Typeface(titleMarker);
            decimalFormat = new DecimalFormat("#,###,##0.00");
            kwhFormat = new DecimalFormat("#,###,##0.00");
        }

        protected SelectedMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }



        public override void RefreshContent(Entry e, Highlight highlight)
        {
            Console.WriteLine(decimalFormat.Format(e.GetY()));

            if (ChartType != null)
            {
                if (ChartType == ChartType.Month)
                {
                    int index = (int)e.GetX();
                    if (AccountType.Equals("2"))
                    {
                        float val = (float)UsageHistoryData.ByMonth.Months[index].Amount;
                        float valKwh = (float)UsageHistoryData.ByMonth.Months[index].Usage;
                        titleMarker.Text = "RM " + decimalFormat.Format(Math.Abs(val)) +"\n"+ kwhFormat.Format(Math.Abs(valKwh)) +" kWh";
                    }
                    else
                    {
                        float val = (float)UsageHistoryData.ByMonth.Months[index].Amount;
                        titleMarker.Text = "RM " + decimalFormat.Format(val);
                    }                   

                }
                else if (ChartType == ChartType.Day)
                {
                    int index = (int)e.GetX();
                    float val = (float)UsageHistoryData.ByDay[CurrentParentIndex].Days[index].Amount;
                    titleMarker.Text = "RM " + decimalFormat.Format(val);
                }
            }
            else
            {
                titleMarker.Text = "RM " + decimalFormat.Format(e.GetY());
            }

            
            base.RefreshContent(e , highlight);
        }

        public override MPPointF GetOffsetForDrawingAtPoint(float posX, float posY)
        {
            return new MPPointF(-(Width / 2), -Height);
        }
    }
}
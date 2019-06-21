using Android.Content;
using Android.Runtime;
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
        private TextView titleMarker;
        public SMUsageHistoryData UsageHistoryData { get; set; }
        public ChartType ChartType { get; set; }
        public ChartDataType ChartDataType { get; set; }
        public string AccountType { get; set; }
        public int CurrentParentIndex = 0;
        public SMSelectedMarkerView(Context context) : base(context, Resource.Layout.MarkerView)
        {
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            TextViewUtils.SetMuseoSans500Typeface(titleMarker);
            decimalFormat = new DecimalFormat("#,###,##0.00");
        }

        protected SMSelectedMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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
                    if (ChartDataType == ChartDataType.RM)
                    {
                        float val = float.Parse(UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].Amount == null ? "0.00" : UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].Amount);
                        titleMarker.Text = "RM " + decimalFormat.Format(val);
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        float val = float.Parse(UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].Consumption == null ? "0.00" : UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].Consumption);
                        titleMarker.Text = decimalFormat.Format(val) + " kWh";
                    }
                    else if (ChartDataType == ChartDataType.CO2)
                    {
                        float val = float.Parse(UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].CO2 == null ? "0.00" : UsageHistoryData.ByMonth[CurrentParentIndex].Months[index].CO2);
                        titleMarker.Text = decimalFormat.Format(val) + " kg";
                    }

                }
                else if (ChartType == ChartType.Day)
                {
                    int index = (int)e.GetX();
                    if (ChartDataType == ChartDataType.RM)
                    {
                        float val = float.Parse(UsageHistoryData.ByDay[CurrentParentIndex].Days[index].Amount == null ? "0.00" : UsageHistoryData.ByDay[CurrentParentIndex].Days[index].Amount);
                        titleMarker.Text = "RM " + decimalFormat.Format(val);
                    }
                    else if (ChartDataType == ChartDataType.kWh)
                    {
                        float val = float.Parse(UsageHistoryData.ByDay[CurrentParentIndex].Days[index].Consumption == null ? "0.00" : UsageHistoryData.ByDay[CurrentParentIndex].Days[index].Consumption);
                        titleMarker.Text = decimalFormat.Format(val) + " kWh";
                    }
                    else if (ChartDataType == ChartDataType.CO2)
                    {
                        float val = float.Parse(UsageHistoryData.ByDay[CurrentParentIndex].Days[index].CO2 == null ? "0.00" : UsageHistoryData.ByDay[CurrentParentIndex].Days[index].CO2);
                        titleMarker.Text = decimalFormat.Format(val) + " kg";
                    }
                }
            }
            else
            {
                titleMarker.Text = "RM " + decimalFormat.Format(e.GetY());
            }


            base.RefreshContent(e, highlight);
        }

        public override MPPointF GetOffsetForDrawingAtPoint(float posX, float posY)
        {
            return new MPPointF(-(Width / 2), -Height);
        }
    }
}
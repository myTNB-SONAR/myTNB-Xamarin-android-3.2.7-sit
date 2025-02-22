﻿using Android.Content;
using Android.Widget;
using Java.Text;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Util;

namespace myTNB_Android.Src.Utils.Custom.MarkerTextView
{
    public class TitleView : MarkerView
    {
        private IAxisValueFormatter xValueFormatter;
        private TextView titleMarker;
        private DecimalFormat decimalFormat;


        public TitleView(Context context, IAxisValueFormatter valueFormatter) : base(context, Resource.Layout.MarkerView)
        {
            this.xValueFormatter = valueFormatter;
            titleMarker = FindViewById<TextView>(Resource.Id.txtMarker);
            decimalFormat = new DecimalFormat("#,###,###.00");
        }

        public override void RefreshContent(Entry e, Highlight highlight)
        {
            titleMarker.Text = "RM " + decimalFormat.Format(e.GetY());
        }

        public override MPPointF GetOffsetForDrawingAtPoint(float posX, float posY)
        {
            return new MPPointF(-(Width / 2), -Height);
        }
    }
}
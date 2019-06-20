using Android.Content;
using Android.Runtime;
using Android.Widget;
using Java.Text;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;
using System;

namespace myTNB_Android.Src.Utils.Custom.Charts
{
    public class XYMarkerView : MarkerView
    {

        private TextView tvContent;
        private IAxisValueFormatter xAxisValueFormatter;

        private DecimalFormat format;

        public XYMarkerView(Context p0, int p1) : base(p0, p1)
        {


        }

        protected XYMarkerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}
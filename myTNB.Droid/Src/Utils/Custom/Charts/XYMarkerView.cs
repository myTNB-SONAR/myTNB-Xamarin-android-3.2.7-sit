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
using MikePhil.Charting.Formatter;
using Java.Text;

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
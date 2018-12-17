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
    public class MyAxisValueFormatter : Java.Lang.Object, IAxisValueFormatter
    {
        private DecimalFormat mFormat;

        public MyAxisValueFormatter()
        {
            mFormat = new DecimalFormat("###,###,###,##0.0");
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            return mFormat.Format(value) + " $";
        }
    }
}
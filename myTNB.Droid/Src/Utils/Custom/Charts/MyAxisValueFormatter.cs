using Java.Text;
using MikePhil.Charting.Components;
using MikePhil.Charting.Formatter;

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
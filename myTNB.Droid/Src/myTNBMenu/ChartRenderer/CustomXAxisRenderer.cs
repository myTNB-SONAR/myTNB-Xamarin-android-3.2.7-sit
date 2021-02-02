using System;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Renderer;
using MikePhil.Charting.Util;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.ChartRenderer
{
    public class CustomXAxisRenderer : XAxisRenderer
    {
        public BarChart barChart { get; set; }
        public Android.App.Activity currentActivity { get; set; }
        public ChartType currentChartType { get; set; }
        public ChartDataType currentChartDataType { get; set; }
        public bool isZoomIn { get; set; }

        private Transformer mTrans;

        private int selectedIndex = -1;

        public CustomXAxisRenderer(ViewPortHandler viewPortHandler, XAxis xAxis, Transformer trans) : base(viewPortHandler, xAxis, trans)
        {
            mTrans = trans;
        }

        protected CustomXAxisRenderer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        protected override void DrawLabels(Canvas p0, float p1, MPPointF p2)
        {
            Highlight[] highlighted = barChart.GetHighlighted();
            if (highlighted != null && highlighted.Length > 0)
            {
                foreach (Highlight selectedData in highlighted)
                {
                    selectedIndex = (int)selectedData.GetX();
                }
            }
            else
            {
                selectedIndex = -1;
            }

            float labelRotationAngleDegrees = 0f;
            bool centeringEnabled = MAxis.IsCenterAxisLabelsEnabled;

            float[] positions = new float[MAxis.MEntryCount * 2];

            for (int i = 0; i < positions.Length; i += 2)
            {

                // only fill x values
                if (centeringEnabled)
                {
                    positions[i] = MAxis.MCenteredEntries[i / 2];
                }
                else
                {
                    positions[i] = MAxis.MEntries[i / 2];
                }
            }

            mTrans.PointValuesToPixel(positions);

            if (TextViewUtils.IsLargeFonts)
            {
                float currentSize = MAxisLabelPaint.TextSize;
                MAxisLabelPaint.TextSize = currentSize + 6;
            }

            for (int i = 0; i < positions.Length; i += 2)
            {
                float x = positions[i];

                if (MViewPortHandler.IsInBoundsX(x))
                {
                    string label = MAxis.ValueFormatter.GetFormattedValue(MAxis.MEntries[i / 2], MAxis);

                    try
                    {
                        Typeface plain = Typeface.CreateFromAsset(currentActivity.Assets, "fonts/" + TextViewUtils.MuseoSans300);
                        MAxisLabelPaint.SetTypeface(plain);
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (currentChartType == ChartType.Month || (currentChartType == ChartType.Day && isZoomIn))
                    {
                        if (currentChartType == ChartType.Month)
                        {
                            if (selectedIndex != -1 && (i / 2 == selectedIndex))
                            {
                                try
                                {
                                    Typeface plain = Typeface.CreateFromAsset(currentActivity.Assets, "fonts/" + TextViewUtils.MuseoSans500);
                                    MAxisLabelPaint.SetTypeface(plain);
                                }
                                catch (System.Exception e)
                                {
                                    Utility.LoggingNonFatalError(e);
                                }
                            }
                        }
                        else
                        {
                            if (i / 2 == 4)
                            {
                                try
                                {
                                    Typeface plain = Typeface.CreateFromAsset(currentActivity.Assets, "fonts/" + TextViewUtils.MuseoSans500);
                                    MAxisLabelPaint.SetTypeface(plain);
                                }
                                catch (System.Exception e)
                                {
                                    Utility.LoggingNonFatalError(e);
                                }
                            }
                        }
                    }
                    MikePhil.Charting.Util.Utils.DrawXAxisValue(p0, label, x, p1, MAxisLabelPaint, p2, labelRotationAngleDegrees);
                }
            }
        }

        protected override void DrawLabel(Canvas p0, string p1, float p2, float p3, MPPointF p4, float p5)
        {

        }
    }
}

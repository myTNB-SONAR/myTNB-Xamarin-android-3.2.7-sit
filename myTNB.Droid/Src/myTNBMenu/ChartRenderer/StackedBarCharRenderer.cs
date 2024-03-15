using MikePhil.Charting.Renderer;
using MikePhil.Charting.Util;
using MikePhil.Charting.Animation;
using System;
using Android.Runtime;
using Android.Graphics;
using MikePhil.Charting.Interfaces.Datasets;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Charts;
using Android.OS;
using Android.Content;
using MikePhil.Charting.Buffer;
using System.Linq;
using Android.Util;
using MikePhil.Charting.Data;
using System.Collections.Generic;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.myTNBMenu.ChartRenderer
{
    public class StackedBarChartRenderer : BarChartRenderer
    {
        private BarChart barChart;
        public UsageHistoryData selectedHistoryData { get; set; }

        public Android.App.Activity currentActivity { get; set; }

        public float[] bufferItems { get; set; }

        private float mRadius = 100f;

        private int selectedIndex = -1;

        private float currentSelectedDrawX = -1f;

        private int currentArrayIndex = -1;

        public Bitmap dpcBitmap { get; set; }

        public ChartType currentChartType { get; set; }

        public ChartDataType currentChartDataType { get; set; }

        // Lin Siong Note: this is for use of tariff block on normal / RE inner dashboard

        public StackedBarChartRenderer(BarChart chart, ChartAnimator animator, ViewPortHandler viewPortHandler) : base(chart, animator, viewPortHandler)
        {
            barChart = chart;
        }

        public void setmRadius(float mRadius)
        {
            this.mRadius = mRadius;
        }

        protected StackedBarChartRenderer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public override void DrawExtras(Canvas canvas)
        {
            try
            {
                currentActivity.RunOnUiThread(() =>
                {
                    try
                    {
                        OnDrawExtras(canvas);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                OnDrawExtras(canvas);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnDrawExtras(Canvas canvas)
        {
            // Lin Siong Note: to get current hightlighted data entry
            try
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
                    currentArrayIndex = -1;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            try
            {
                if (barChart.Data != null && barChart.Data is BarData)
                {
                    BarData barData = barChart.Data as BarData;

                    if (selectedIndex != -1)
                    {
                        currentArrayIndex = 0;
                        BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;
                        if (selectedIndex != 0)
                        {
                            for (int j = 0; j <= selectedIndex; j++)
                            {
                                BarEntry listEntries = currentDataSet.Values[j] as BarEntry;
                                float[] yLsit = listEntries.GetYVals();
                                currentArrayIndex += yLsit.Length;
                                if (j == selectedIndex)
                                {
                                    currentArrayIndex -= 1;
                                }
                            }
                        }
                        currentArrayIndex = currentArrayIndex * 4;
                    }
                    else
                    {
                        currentArrayIndex = -1;
                    }

                    if (barData.DataSetCount > 0)
                    {
                        IBarDataSet dataset = barData.GetDataSetByIndex(0) as IBarDataSet;
                        DrawDataSet(canvas, dataset, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        protected override void DrawDataSet(Canvas canvas, IBarDataSet dataSet, int index)
        {
            try
            {
                currentActivity.RunOnUiThread(() =>
                {
                    try
                    {
                        OnDrawDataSet(canvas, dataSet, index);
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }
                });
            }
            catch (Exception e)
            {
                OnDrawDataSet(canvas, dataSet, index);
                Utility.LoggingNonFatalError(e);
            }
        }

        private void OnDrawDataSet(Canvas canvas, IBarDataSet dataSet, int index)
        {
            try
            {
                // Lin Siong Note:
                // The concept of drawing the tariff block out with which bar need to be hightlighted:
                // 1) First it will get the left point of the box that going to draw, also the next four index
                // 2) i) if the next four index already out of buffer, direct draw the full rounded bar out
                // 2) ii) if no, then determine if next index is same, if yes then draw lower rounded bar, else draw the full rounded bar out
                // 2) iii) For next if is still same as previous and next, then will draw rectangle bar, else draw upper rounded bar
                Transformer trans = barChart.GetTransformer(dataSet.AxisDependency);

                MShadowPaint.Color = new Color(dataSet.BarShadowColor);

                float phaseX = MAnimator.PhaseX;
                float phaseY = MAnimator.PhaseY;


                // initialize the buffer
                BarBuffer buffer = MBarBuffers[index];
                buffer.SetPhases(phaseX, phaseY);
                buffer.SetDataSet(index);
                buffer.SetBarWidth(barChart.BarData.BarWidth);
                buffer.SetInverted(barChart.IsInverted(dataSet.AxisDependency));

                buffer.Feed(dataSet);

                bufferItems = new float[buffer.Buffer.Count];
                buffer.Buffer.CopyTo(bufferItems, 0);
                trans.PointValuesToPixel(bufferItems);

                // if multiple colors
                if (dataSet.Colors.Count > 1)
                {
                    bool isFirstTime = true;
                    float currentItem = 0f;
                    int count = 0;

                    // Lin Siong Note: to get current hightlighted data entry
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
                        currentArrayIndex = -1;
                    }

                    if (barChart.Data != null && barChart.Data is BarData)
                    {
                        BarData barData = barChart.Data as BarData;

                        if (selectedIndex != -1)
                        {
                            currentArrayIndex = 0;
                            BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;
                            if (selectedIndex != 0)
                            {
                                for (int j = 0; j <= selectedIndex; j++)
                                {
                                    BarEntry listEntries = currentDataSet.Values[j] as BarEntry;
                                    float[] yLsit = listEntries.GetYVals();
                                    currentArrayIndex += yLsit.Length;
                                    if (j == selectedIndex)
                                    {
                                        currentArrayIndex -= 1;
                                    }
                                }
                            }
                            currentArrayIndex = currentArrayIndex * 4;
                        }
                        else
                        {
                            currentArrayIndex = -1;
                        }
                    }


                    if (currentArrayIndex != -1)
                    {
                        currentSelectedDrawX = bufferItems[currentArrayIndex];
                    }
                    else
                    {
                        currentSelectedDrawX = -1;
                    }

                    int currentRow = 0;

                    for (int j = 0; j < buffer.Size(); j += 4)
                    {
                        float left = bufferItems[j];
                        float top = bufferItems[j + 1];
                        float right = bufferItems[j + 2];
                        float bottom = bufferItems[j + 3];
                        bool isCurrentSelected = false;


                        if (!MViewPortHandler.IsInBoundsLeft(bufferItems[j + 2]))
                            continue;

                        if (!MViewPortHandler.IsInBoundsRight(bufferItems[j]))
                            break;

                        int size = dataSet.StackSize;
                        MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));

                        if (j == 0)
                        {
                            isFirstTime = false;
                            currentItem = bufferItems[j];
                            int nextIndex = j + 4;
                            // Lin Siong Note: check if need to hightlight the entry
                            // Lin Siong Note: if yes then set alpha to 255
                            // Lin Siong Note: else get the default alpha, set to 50
                            if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                            {
                                MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                            }
                            else
                            {
                                if (Math.Abs(currentSelectedDrawX - currentItem) < 0.0001)
                                {
                                    MRenderPaint.Alpha = 255;
                                    isCurrentSelected = true;
                                }
                                else
                                {
                                    MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                                }
                            }
                            if (nextIndex >= buffer.Size())
                            {
                                if (mRadius > 0)
                                {
                                    canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, true, true), MRenderPaint);
                                }
                                else
                                {
                                    canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, false, false), MRenderPaint);
                                }

                                if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                {
                                    DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                }

                                currentRow++;
                            }
                            else
                            {
                                if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                {
                                    count++;
                                    if (mRadius > 0)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                }
                                else
                                {
                                    count = 0;
                                    canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, true, true), MRenderPaint);

                                    if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                    {
                                        DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                    }

                                    currentRow++;
                                }
                            }
                        }
                        else if (!isFirstTime)
                        {
                            currentItem = bufferItems[j];
                            int nextIndex = j + 4;
                            // Lin Siong Note: check if need to hightlight the entry
                            // Lin Siong Note: if yes then set alpha to 255
                            // Lin Siong Note: else get the default alpha, set to 50
                            if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                            {
                                MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                            }
                            else
                            {
                                if (Math.Abs(currentSelectedDrawX - currentItem) < 0.0001)
                                {
                                    MRenderPaint.Alpha = 255;
                                    isCurrentSelected = true;
                                }
                                else
                                {
                                    MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                                }
                            }
                            if (nextIndex >= buffer.Size())
                            {

                                if (count == 0)
                                {
                                    if (mRadius > 0)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                }
                                else
                                {
                                    if (mRadius > 0)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, false, false), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                }

                                if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                {
                                    DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                }

                                currentRow++;
                            }
                            else
                            {
                                if (count == 0)
                                {
                                    if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                    {
                                        count++;
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        count = 0;
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, true, true), MRenderPaint);

                                        if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                        {
                                            DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                        }

                                        currentRow++;
                                    }
                                }
                                else
                                {
                                    if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                    {
                                        count++;
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                    else
                                    {
                                        count = 0;
                                        canvas.DrawPath(GenerateRoundRectangle(bufferItems[j], bufferItems[j + 1], bufferItems[j + 2], bufferItems[j + 3], mRadius, mRadius, true, true, false, false), MRenderPaint);

                                        if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                        {
                                            DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                        }

                                        currentRow++;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void DrawDPCOnCanvas(Canvas c, float top, float left, float right, bool isSelected)
        {
            try
            {
                // Lin Siong Note: Set Render Text to 10dp
                // Lin Siong Note: Set text to align center
                MRenderPaint.Color = new Color(255, 255, 255, 255);

                if (isSelected)
                {
                    MRenderPaint.Color = new Color(255, 255, 255, 255);
                }
                else
                {
                    MRenderPaint.Color = new Color(255, 255, 255, 50);
                }

                MRenderPaint.TextSize = DPUtils.ConvertDPToPx(10f);
                MRenderPaint.TextAlign = Paint.Align.Center;

                // Lin Siong Note: Set the typeface to MuseoSans500
                try
                {
                    Typeface plain = Typeface.CreateFromAsset(currentActivity.Assets, "fonts/" + TextViewUtils.MuseoSans500);
                    MRenderPaint.SetTypeface(plain);
                }
                catch (System.Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }

                float x = left + ((right - left) / 2) - DPUtils.ConvertDPToPx(6.5f);
                float y = top - DPUtils.ConvertDPToPx(12f);


                if (currentChartType == ChartType.Month)
                {
                    c.DrawBitmap(dpcBitmap, x, y, MRenderPaint);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private Path GenerateRoundRectangle(float left, float top, float right, float bottom, float rx, float ry, bool tl, bool tr, bool br, bool bl)
        {
            Path path = new Path();
            try
            {
                if (rx < 0) rx = 0;
                if (ry < 0) ry = 0;
                float width = right - left;
                float height = bottom - top;
                if (rx > width / 2) rx = width / 2;
                if (ry > height / 2) ry = height / 2;
                if (rx < ry)
                {
                    ry = rx;
                }
                float widthMinusCorners = (width - (2 * rx));
                float heightMinusCorners = (height - (2 * ry) - DPUtils.ConvertDPToPx(1f));

                path.MoveTo(right, top + ry);
                if (tr)
                    path.ArcTo(right - 2 * rx, top, right, top + 2 * ry, 0, -90, false);//top-right corner
                else
                {
                    path.RLineTo(0, -ry);
                    path.RLineTo(-rx, 0);
                }
                path.RLineTo(-widthMinusCorners, 0);
                if (tl)
                    path.ArcTo(left, top, left + 2 * rx, top + 2 * ry, 270, -90, false); //top-left corner
                else
                {
                    path.RLineTo(-rx, 0);
                    path.RLineTo(0, ry);
                }
                path.RLineTo(0, heightMinusCorners);

                if (bl)
                    path.ArcTo(left, bottom - 2 * ry, left + 2 * rx, bottom, 180, -90, false);//bottom-left corner
                else
                {
                    path.RLineTo(0, ry);
                    path.RLineTo(rx, 0);
                }

                path.RLineTo(widthMinusCorners, 0);
                if (br)
                    path.ArcTo(right - 2 * rx, bottom - 2 * ry, right, bottom, 90, -90, false); //bottom-right corner
                else
                {
                    path.RLineTo(rx, 0);
                    path.RLineTo(0, -ry);
                }

                path.RLineTo(0, -heightMinusCorners);

                path.Close();//Given close, last lineto can be removed.
            }
            catch (Exception e)
            {
                try
                {
                    path.Close();
                }
                catch (Exception ne)
                {
                    Utility.LoggingNonFatalError(ne);
                }
                Utility.LoggingNonFatalError(e);
            }

            return path;
        }

    }
}

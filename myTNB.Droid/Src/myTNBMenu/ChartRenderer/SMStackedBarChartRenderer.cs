using MikePhil.Charting.Renderer;
using MikePhil.Charting.Util;
using MikePhil.Charting.Animation;
using System;
using Android.Runtime;
using Android.Graphics;
using MikePhil.Charting.Interfaces.Datasets;
using myTNB_Android.Src.myTNBMenu.Models;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Charts;
using Android.OS;
using Android.Content;
using MikePhil.Charting.Buffer;
using System.Linq;
using Android.Util;
using MikePhil.Charting.Data;
using System.Collections.Generic;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.ChartRenderer
{
    public class SMStackedBarChartRenderer : BarChartRenderer
    {
        private BarChart barChart;
        public UsageHistoryData selectedHistoryData { get; set; }

        public Context currentContext { get; set; }

        public float[] bufferItems { get; set; }

        private float mRadius = 100f;

        private int selectedIndex = -1;

        private float currentSelectedDrawX = -1f;

        private int currentArrayIndex = -1;

        private int lastMonthIndex = -1;

        private int lastMonthFirstIndex = -1;

        private int lastMonthLastIndex = -1;

        private float lastMonthFirstPoint = -1f;

        private float lastMonthSecondPoint = -1f;

        private float lastMonthThirdPoint = -1f;

        private float lastMonthForthPoint = -1f;

        private bool isDeductedNoNeed = false;

        public bool isStacked { get; set; }

        public SMStackedBarChartRenderer(BarChart chart, ChartAnimator animator, ViewPortHandler viewPortHandler) : base(chart, animator, viewPortHandler)
        {
            barChart = chart;
        }

        public void setmRadius(float mRadius)
        {
            this.mRadius = mRadius;
        }

        protected SMStackedBarChartRenderer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public override void DrawExtras(Canvas canvas)
        {
            // base.DrawExtras(canvas);
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

                    BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;

                    if (selectedIndex != -1)
                    {
                        currentArrayIndex = 0;
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

                    lastMonthIndex = currentDataSet.Values.Count;
                    lastMonthFirstIndex = 0;
                    lastMonthLastIndex = 0;
                    for (int j = 0; j < lastMonthIndex; j++)
                    {
                        BarEntry listEntries = currentDataSet.Values[j] as BarEntry;
                        float[] yLsit = listEntries.GetYVals();
                        if (j == (lastMonthIndex - 1))
                        {
                            if (yLsit.Length != 1)
                            {
                                lastMonthFirstIndex += 1;
                                lastMonthLastIndex += yLsit.Length;
                            }
                            else
                            {
                                lastMonthFirstIndex += 1;
                                lastMonthLastIndex += 1;
                            }
                            lastMonthFirstIndex -= 1;
                            lastMonthLastIndex -= 1;
                        }
                        else
                        {
                            lastMonthFirstIndex += yLsit.Length;
                            lastMonthLastIndex += yLsit.Length;
                        }
                    }

                    lastMonthFirstIndex = lastMonthFirstIndex * 4;
                    lastMonthLastIndex = lastMonthLastIndex * 4;

                    if (barData.DataSetCount > 0)
                    {
                        IBarDataSet dataset = barData.GetDataSetByIndex(0) as IBarDataSet;
                        DrawDataSet(canvas, dataset, 0);
                    }
                }
                else
                {
                    lastMonthIndex = -1;
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
                Transformer trans = barChart.GetTransformer(dataSet.AxisDependency);

                MShadowPaint.Color = new Color(dataSet.BarShadowColor);

                MBarBorderPaint.StrokeWidth = DPUtils.ConvertDPToPx(1f);

                float phaseX = MAnimator.PhaseX;
                float phaseY = MAnimator.PhaseY;


                // initialize the buffer
                BarBuffer buffer = MBarBuffers[index];
                buffer.SetPhases(phaseX, phaseY);
                buffer.SetDataSet(index);
                buffer.SetBarWidth(barChart.BarData.BarWidth);
                buffer.SetInverted(barChart.IsInverted(dataSet.AxisDependency));

                buffer.Feed(dataSet);


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

                    BarDataSet currentDataSet = barData.GetDataSetByIndex(0) as BarDataSet;

                    if (selectedIndex != -1)
                    {
                        currentArrayIndex = 0;
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

                    lastMonthIndex = currentDataSet.Values.Count;
                    lastMonthFirstIndex = 0;
                    lastMonthLastIndex = 0;
                    for (int j = 0; j < lastMonthIndex; j++)
                    {
                        BarEntry listEntries = currentDataSet.Values[j] as BarEntry;
                        float[] yLsit = listEntries.GetYVals();
                        if (j == (lastMonthIndex - 1))
                        {
                            if (yLsit.Length != 1)
                            {
                                lastMonthFirstIndex += 1;
                                lastMonthLastIndex += yLsit.Length;
                            }
                            else
                            {
                                lastMonthFirstIndex += 1;
                                lastMonthLastIndex += 1;
                            }
                            lastMonthFirstIndex -= 1;
                            lastMonthLastIndex -= 1;
                        }
                        else
                        {
                            lastMonthFirstIndex += yLsit.Length;
                            lastMonthLastIndex += yLsit.Length;
                        }
                    }

                    lastMonthFirstIndex = lastMonthFirstIndex * 4;
                    lastMonthLastIndex = lastMonthLastIndex * 4;
                }
                else
                {
                    lastMonthIndex = -1;
                }

                bufferItems = new float[buffer.Buffer.Count];
                float[] newBufferItems = new float[4];
                buffer.Buffer.CopyTo(bufferItems, 0);

                float offsetValue = 0f;

                if (lastMonthIndex != -1)
                {
                    newBufferItems[0] = bufferItems[lastMonthFirstIndex] - 0.05f;
                    newBufferItems[1] = bufferItems[lastMonthLastIndex + 1] - 0.05f;
                    newBufferItems[2] = bufferItems[lastMonthLastIndex + 2] + 0.05f;
                    newBufferItems[3] = bufferItems[lastMonthFirstIndex + 3] + 0.05f;
                    trans.PointValuesToPixel(newBufferItems);
                }

                trans.PointValuesToPixel(bufferItems);

                // if multiple colors
                if (dataSet.Colors.Count > 1)
                {
                    bool isFirstTime = true;
                    float currentItem = 0f;
                    int count = 0;

                    if (lastMonthIndex != -1)
                    {
                        lastMonthFirstPoint = bufferItems[lastMonthFirstIndex];
                        lastMonthSecondPoint = bufferItems[lastMonthLastIndex + 1];
                        lastMonthThirdPoint = bufferItems[lastMonthLastIndex + 2];
                        lastMonthForthPoint = bufferItems[lastMonthFirstIndex + 3];
                        offsetValue = newBufferItems[0] - lastMonthFirstPoint;
                    }
                    else
                    {
                        lastMonthFirstPoint = -1f;
                        lastMonthSecondPoint = -1f;
                        lastMonthThirdPoint = -1f;
                        lastMonthForthPoint = -1f;
                    }


                    if (currentArrayIndex != -1)
                    {
                        currentSelectedDrawX = bufferItems[currentArrayIndex];
                    }
                    else
                    {
                        currentSelectedDrawX = -1;
                    }

                    for (int j = 0; j < buffer.Size(); j += 4)
                    {
                        float left = bufferItems[j];
                        float top = bufferItems[j + 1];
                        float right = bufferItems[j + 2];
                        float bottom = bufferItems[j + 3];

                        if (!MViewPortHandler.IsInBoundsLeft(right))
                            continue;

                        if (!MViewPortHandler.IsInBoundsRight(left))
                            break;

                        int size = dataSet.StackSize;
                        MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));

                        if (j == 0)
                        {
                            currentItem = left;

                            isFirstTime = false;
                           
                            int nextIndex = j + 4;
                            if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                            {
                                MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                            }
                            else
                            {
                                if (Math.Abs(currentSelectedDrawX - currentItem) < 0.0001)
                                {
                                    MRenderPaint.Alpha = 255;
                                }
                                else
                                {
                                    MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                                }
                            }

                            if (lastMonthIndex != -1)
                            {
                                if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                                {
                                    MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                }
                                else
                                {
                                    if (Math.Abs(lastMonthFirstPoint - currentSelectedDrawX) < 0.0001)
                                    {
                                        MBarBorderPaint.Color = new Color(255, 255, 255, 255);
                                    }
                                    else
                                    {
                                        MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                    }
                                }
                            }

                            if (nextIndex >= buffer.Size())
                            {
                                if (mRadius > 0)
                                {
                                    if (isStacked)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                }
                                else
                                {
                                    if (isStacked)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                    }
                                }
                            }
                            else
                            {
                                if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                {
                                    count++;
                                    if (mRadius > 0)
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, true, true), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, true, true), MRenderPaint);
                                        }
                                    }
                                    else
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                    }
                                }
                                else
                                {
                                    count = 0;
                                    if (isStacked)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                }
                            }
                        }
                        else if (!isFirstTime)
                        {
                            int nextIndex = j + 4;
                            currentItem = left;
                            if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                            {
                                MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                            }
                            else
                            {
                                if (Math.Abs(currentSelectedDrawX - currentItem) < 0.0001)
                                {
                                    MRenderPaint.Alpha = 255;
                                }
                                else
                                {
                                    MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));
                                }
                            }

                            if (lastMonthIndex != -1)
                            {
                                if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                                {
                                    MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                }
                                else
                                {
                                    if (Math.Abs(lastMonthFirstPoint - currentSelectedDrawX) < 0.0001)
                                    {
                                        MBarBorderPaint.Color = new Color(255, 255, 255, 255);
                                    }
                                    else
                                    {
                                        MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                    }
                                }
                            }

                            if (nextIndex >= buffer.Size())
                            {

                                if (count == 0)
                                {
                                    if (mRadius > 0)
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, bottom, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                        }
                                    }
                                    else
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                    }
                                }
                                else
                                {
                                    if (mRadius > 0)
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                    }
                                    else
                                    {
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (count == 0)
                                {
                                    if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                    {
                                        count++;
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, true, true), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, true, true), MRenderPaint);
                                        }
                                    }
                                    else
                                    {
                                        count = 0;
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Math.Abs(currentItem - bufferItems[nextIndex]) < 0.001)
                                    {
                                        count++;
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, false, false, false, false), MRenderPaint);
                                        }
                                    }
                                    else
                                    {
                                        count = 0;
                                        if (isStacked)
                                        {
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    if (lastMonthIndex != -1 && bufferItems.Length > 0)
                    {
                        canvas.DrawPath(GenerateRoundRectangleWithNoSpace(lastMonthFirstPoint + offsetValue, lastMonthSecondPoint + offsetValue, lastMonthThirdPoint - offsetValue, lastMonthForthPoint - offsetValue, mRadius, mRadius, true, true, true, true), MBarBorderPaint);
                    }
                }
            }
            catch (Exception e)
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
                float heightMinusCorners = (height - (2 * ry) - 4f);

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

        private Path GenerateRoundRectangleWithNoSpace(float left, float top, float right, float bottom, float rx, float ry, bool tl, bool tr, bool br, bool bl)
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
                float heightMinusCorners = (height - (2 * ry));

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
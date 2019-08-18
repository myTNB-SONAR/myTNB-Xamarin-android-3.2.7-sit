﻿using MikePhil.Charting.Renderer;
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

namespace myTNB_Android.Src.myTNBMenu.ChartRenderer
{
    public class StackedBarChartRenderer : BarChartRenderer
    {
        private BarChart barChart;
        public UsageHistoryData selectedHistoryData { get; set; }

        public Context currentContext { get; set; }

        private int currentSelectedIndex = -1;

        private float mRadius = 60f;

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

        public override void DrawExtras(Canvas p0)
        {
            base.DrawExtras(p0);
            int lastIndex = (selectedHistoryData != null && selectedHistoryData.ByMonth != null && selectedHistoryData.ByMonth.Months != null)
                ? selectedHistoryData.ByMonth.Months.Count : -1;
            Highlight[] highlighted = barChart.GetHighlighted();
            if (highlighted == null)
            {
                currentSelectedIndex = -1;
                return;
            }

            if (lastIndex == -1)
            {
                currentSelectedIndex = -1;
                return;
            }

            foreach (Highlight selectedData in highlighted)
            {

                int selectedIndex = (int)selectedData.GetX();
                /*if (currentSelectedIndex != selectedIndex)
                {
                    currentSelectedIndex = selectedIndex;
                    Vibrator vibrator = (Vibrator)currentContext.GetSystemService(Context.VibratorService);
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.O)
                    {
                        vibrator.Vibrate(VibrationEffect.CreateOneShot(250, 12));

                    }
                    else
                    {
                        vibrator.Vibrate(250);

                    }
                }

                if (selectedIndex == (lastIndex - 1))
                {
                    break;
                }
                int nextIndex = selectedIndex + 1;*/
                //Path fillPathBuffer = MGenerateFilledPathBuffer;

                // generateFilledPath(p0, selectedIndex, nextIndex, fillPathBuffer);
            }
        }

        protected override void DrawDataSet(Canvas canvas, IBarDataSet dataSet, int index)
        {
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

            float[] bufferItems = new float[buffer.Buffer.Count];
            buffer.Buffer.CopyTo(bufferItems, 0);
            trans.PointValuesToPixel(bufferItems);

            // if multiple colors
            if (dataSet.Colors.Count > 1)
            {
                bool isFirstTime = true;
                float currentItem = 0f;
                int count = 0;
                for (int j = 0; j < buffer.Size(); j += 4)
                {

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
                            }
                        }
                    }
                    else if (!isFirstTime)
                    {
                        int nextIndex = j + 4;
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
                        }
                        else
                        {
                            currentItem = bufferItems[j];
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
                                }
                            }
                        }
                    }

                }
            }
        }

        private Path GenerateRoundRectangle(float left, float top, float right, float bottom, float rx, float ry, bool tl, bool tr, bool br, bool bl)
        {
            Path path = new Path();
            if (rx < 0) rx = 0;
            if (ry < 0) ry = 0;
            float width = right - left;
            float height = bottom - top;
            if (rx > width / 2) rx = width / 2;
            if (ry > height / 2) ry = height / 2;
            float widthMinusCorners = (width - (2 * rx));
            float heightMinusCorners = (height - (2 * ry) - 4f);

            path.MoveTo(right, top + ry);
            if (tr)
                path.RQuadTo(0, -ry, -rx, -ry);//top-right corner
            else
            {
                path.RLineTo(0, -ry);
                path.RLineTo(-rx, 0);
            }
            path.RLineTo(-widthMinusCorners, 0);
            if (tl)
                path.RQuadTo(-rx, 0, -rx, ry); //top-left corner
            else
            {
                path.RLineTo(-rx, 0);
                path.RLineTo(0, ry);
            }
            path.RLineTo(0, heightMinusCorners);

            if (bl)
                path.RQuadTo(0, ry, rx, ry);//bottom-left corner
            else
            {
                path.RLineTo(0, ry);
                path.RLineTo(rx, 0);
            }

            path.RLineTo(widthMinusCorners, 0);
            if (br)
                path.RQuadTo(rx, 0, rx, -ry); //bottom-right corner
            else
            {
                path.RLineTo(rx, 0);
                path.RLineTo(0, -ry);
            }

            path.RLineTo(0, -heightMinusCorners);

            path.Close();//Given close, last lineto can be removed.

            return path;
        }

    }
}
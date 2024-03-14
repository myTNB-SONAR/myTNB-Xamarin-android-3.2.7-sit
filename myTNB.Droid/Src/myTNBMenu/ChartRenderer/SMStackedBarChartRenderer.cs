using MikePhil.Charting.Renderer;
using MikePhil.Charting.Util;
using MikePhil.Charting.Animation;
using System;
using Android.Runtime;
using Android.Graphics;
using MikePhil.Charting.Interfaces.Datasets;
using myTNB.Android.Src.myTNBMenu.Models;
using MikePhil.Charting.Highlight;
using MikePhil.Charting.Charts;
using Android.OS;
using Android.Content;
using MikePhil.Charting.Buffer;
using System.Linq;
using Android.Util;
using MikePhil.Charting.Data;
using System.Collections.Generic;
using myTNB.Android.Src.Utils;
using Android.Icu.Text;

namespace myTNB.Android.Src.myTNBMenu.ChartRenderer
{
    public class SMStackedBarChartRenderer : BarChartRenderer
    {
        private BarChart barChart;

        public SMUsageHistoryData selectedSMHistoryData { get; set; }

        public Android.App.Activity currentActivity { get; set; }

        public ChartType currentChartType { get; set; }

        public ChartDataType currentChartDataType { get; set; }

        public List<bool> missingReadingList { get; set; }

        public bool isMDMSDown { get; set; }

        public bool isZoomIn { get; set; }

        public float[] bufferItems { get; set; }

        private float mRadius = 100f;

        private int selectedIndex = -1;

        private float currentSelectedDrawX = -1f;

        private int currentArrayIndex = -1;

        private int lastMonthIndex = -1;

        private int lastMonthFirstIndex = -1;

        private int lastMonthLastIndex = -1;

        private float lastMonthLeftPoint = -1f;

        private float lastMonthTopPoint = -1f;

        private float lastMonthRightPoint = -1f;

        private float lastMonthBottomPoint = -1f;

        private bool isDeductedNoNeed = false;

        public bool isStacked { get; set; }


        // private DecimalFormat decimalFormat = new DecimalFormat("#,###,##0.00");
        // private DecimalFormat kwhFormat = new DecimalFormat("#,###,##0");
        public Bitmap mdmsBitmap { get; set; }
        public Bitmap missingBitmap { get; set; }
        public Bitmap dpcBitmap { get; set; }

        // Lin Siong Note: this is for use of tariff block on smart meter inner dashboard
        // Lin Siong Note: Smart Meter Chart Renderer support isStacked Flag, to determine whether wanna have spacing between bar or not


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
            try
            {
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

                    // Lin Siong Note: to determine on whether which bar is last month
                    // Lin Siong Note: but actually have quesiton on this, if the smart meter just open and one have one month of data
                    // Lin Siong Note: How backend return the data to us?
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

                    // Lin Siong Note: to determine on whether which bar is last month
                    // Lin Siong Note: but actually have quesiton on this, if the smart meter just open and one have one month of data
                    // Lin Siong Note: How backend return the data to us?
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
                        lastMonthLeftPoint = bufferItems[lastMonthFirstIndex];
                        lastMonthTopPoint = bufferItems[lastMonthLastIndex + 1];
                        lastMonthRightPoint = bufferItems[lastMonthLastIndex + 2];
                        lastMonthBottomPoint = bufferItems[lastMonthFirstIndex + 3];
                        offsetValue = newBufferItems[0] - lastMonthLeftPoint;
                    }
                    else
                    {
                        lastMonthLeftPoint = -1f;
                        lastMonthTopPoint = -1f;
                        lastMonthRightPoint = -1f;
                        lastMonthBottomPoint = -1f;
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

                    try
                    {
                        for (int j = 0; j < buffer.Size(); j += 4)
                        {
                            float left = bufferItems[j];
                            float top = bufferItems[j + 1];
                            float right = bufferItems[j + 2];
                            float bottom = bufferItems[j + 3];
                            bool isCurrentSelected = false;

                            /*if (!MViewPortHandler.IsInBoundsLeft(right))
                                continue;

                            if (!MViewPortHandler.IsInBoundsRight(left))
                                break;*/

                            int size = dataSet.StackSize;
                            MRenderPaint.Color = new Color(dataSet.GetColor(j / 4));

                            if (j == 0)
                            {
                                currentItem = left;

                                isFirstTime = false;

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

                                if (lastMonthIndex != -1)
                                {
                                    if (Math.Abs(currentSelectedDrawX - -1f) < 0.0001)
                                    {
                                        MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                    }
                                    else
                                    {
                                        if (Math.Abs(lastMonthLeftPoint - currentSelectedDrawX) < 0.0001)
                                        {
                                            MBarBorderPaint.Color = new Color(255, 255, 255, 255);
                                        }
                                        else
                                        {
                                            MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                        }
                                    }
                                }

                                if (currentChartType == ChartType.Day && !isZoomIn)
                                {
                                    MRenderPaint.Alpha = 255;
                                }

                                if (nextIndex >= buffer.Size())
                                {
                                    if (isStacked)
                                    {
                                        canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }
                                    else
                                    {
                                        canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, true, true), MRenderPaint);
                                    }

                                    if (currentChartType == ChartType.Day && missingReadingList[currentRow])
                                    {
                                        DrawMissingReadingDownOnCanvas(canvas, top, left, right, isZoomIn, isCurrentSelected);
                                    }

                                    if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedSMHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                    {
                                        if (isMDMSDown)
                                        {
                                            if (currentRow != selectedSMHistoryData.ByMonth.Months.Count - 1)
                                            {
                                                DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                            }
                                        }
                                        else
                                        {
                                            DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                        }
                                    }

                                    currentRow++;
                                }
                                else
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

                                        if (currentChartType == ChartType.Day && missingReadingList[currentRow])
                                        {
                                            DrawMissingReadingDownOnCanvas(canvas, top, left, right, isZoomIn, isCurrentSelected);
                                        }

                                        if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedSMHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                        {
                                            if (isMDMSDown)
                                            {
                                                if (currentRow != selectedSMHistoryData.ByMonth.Months.Count - 1)
                                                {
                                                    DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                                }
                                            }
                                            else
                                            {
                                                DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                            }
                                        }

                                        currentRow++;
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
                                        isCurrentSelected = true;
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
                                        if (Math.Abs(lastMonthLeftPoint - currentSelectedDrawX) < 0.0001)
                                        {
                                            MBarBorderPaint.Color = new Color(255, 255, 255, 255);
                                        }
                                        else
                                        {
                                            MBarBorderPaint.Color = new Color(255, 255, 255, 50);
                                        }
                                    }
                                }

                                if (currentChartType == ChartType.Day && !isZoomIn)
                                {
                                    MRenderPaint.Alpha = 255;
                                }

                                if (nextIndex >= buffer.Size())
                                {
                                    if (count == 0)
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
                                            canvas.DrawPath(GenerateRoundRectangle(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                        else
                                        {
                                            canvas.DrawPath(GenerateRoundRectangleWithNoSpace(left, top, right, bottom, mRadius, mRadius, true, true, false, false), MRenderPaint);
                                        }
                                    }

                                    if (currentChartType == ChartType.Day && missingReadingList[currentRow])
                                    {
                                        DrawMissingReadingDownOnCanvas(canvas, top, left, right, isZoomIn, isCurrentSelected);
                                    }

                                    if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedSMHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                    {
                                        if (isMDMSDown)
                                        {
                                            if (currentRow != selectedSMHistoryData.ByMonth.Months.Count - 1)
                                            {
                                                DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                            }
                                        }
                                        else
                                        {
                                            DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                        }
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
                                            if (currentChartType == ChartType.Day && missingReadingList[currentRow])
                                            {
                                                DrawMissingReadingDownOnCanvas(canvas, top, left, right, isZoomIn, isCurrentSelected);
                                            }

                                            if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedSMHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                            {
                                                if (isMDMSDown)
                                                {
                                                    if (currentRow != selectedSMHistoryData.ByMonth.Months.Count - 1)
                                                    {
                                                        DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                                    }
                                                }
                                                else
                                                {
                                                    DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                                }
                                            }

                                            currentRow++;
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

                                            if (currentChartType == ChartType.Day && missingReadingList[currentRow])
                                            {
                                                DrawMissingReadingDownOnCanvas(canvas, top, left, right, isZoomIn, isCurrentSelected);
                                            }

                                            if (currentChartType == ChartType.Month && currentChartDataType == ChartDataType.kWh && selectedSMHistoryData.ByMonth.Months[currentRow].DPCIndicator)
                                            {
                                                if (isMDMSDown)
                                                {
                                                    if (currentRow != selectedSMHistoryData.ByMonth.Months.Count - 1)
                                                    {
                                                        DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                                    }
                                                }
                                                else
                                                {
                                                    DrawDPCOnCanvas(canvas, top, left, right, isCurrentSelected);
                                                }
                                            }

                                            currentRow++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.LoggingNonFatalError(ex);
                    }

                    if (currentChartType == ChartType.Month)
                    {
                        if (lastMonthIndex != -1 && bufferItems.Length > 0)
                        {
                            // Lin Siong Note: Draw MDMS Down Scenario
                            if (isMDMSDown)
                            {
                                bool isSelected = false;
                                if (Math.Abs(lastMonthLeftPoint - currentSelectedDrawX) < 0.0001)
                                {
                                    isSelected = true;
                                }
                                else
                                {
                                    isSelected = false;
                                }

                                DrawMDMSDownOnCanvas(canvas, lastMonthTopPoint, lastMonthLeftPoint, lastMonthRightPoint, isSelected);
                            }
                            // Lin Siong Note: Draw Ring on Last bar, Hide Now as requirement changed
                            // Lin Siong Note: Draw Text On Lasg bar
                            // canvas.DrawPath(GenerateRoundRectangleWithNoSpace(lastMonthLeftPoint + offsetValue, lastMonthTopPoint + offsetValue, lastMonthRightPoint - offsetValue, lastMonthBottomPoint - offsetValue, mRadius, mRadius, true, true, true, true), MBarBorderPaint);
                            // DrawTextOnCanvas(canvas, lastMonthTopPoint, lastMonthLeftPoint, lastMonthRightPoint, selectedSMHistoryData.ByMonth.Months[selectedSMHistoryData.ByMonth.Months.Count - 1].Currency, selectedSMHistoryData.ByMonth.Months[selectedSMHistoryData.ByMonth.Months.Count - 1].AmountTotal, selectedSMHistoryData.ByMonth.Months[selectedSMHistoryData.ByMonth.Months.Count - 1].UsageUnit, selectedSMHistoryData.ByMonth.Months[selectedSMHistoryData.ByMonth.Months.Count - 1].UsageTotal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void DrawMDMSDownOnCanvas(Canvas c, float top, float left, float right, bool isSelected)
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

                float x = left + ((right - left) / 2)  - DPUtils.ConvertDPToPx(6.5f);
                float y = top - DPUtils.ConvertDPToPx(12f);


                if (currentChartType == ChartType.Month)
                {
                    c.DrawBitmap(mdmsBitmap, x, y, MRenderPaint);

                    /*float textX = left + ((right - left) / 2);
                    float textY = top - DPUtils.ConvertDPToPx(27f);
                    string firstTxt = "Unavailable";
                    c.DrawText(firstTxt, textX, textY, MRenderPaint);
                    string secondTxt = "Currently";
                    textY = top - DPUtils.ConvertDPToPx(41f);
                    c.DrawText(secondTxt, textX, textY, MRenderPaint);*/
                }
            }
            catch (System.Exception e)
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

        private void DrawMissingReadingDownOnCanvas(Canvas c, float top, float left, float right, bool isZoom, bool isSelected)
        {
            try
            {
                if (!isZoom || (isZoom && !isSelected))
                {
                    if (!isZoom)
                    {
                        MRenderPaint.Color = new Color(255, 255, 255, 255);
                    }
                    else
                    {
                        MRenderPaint.Color = new Color(255, 255, 255, 50);
                    }

                    MRenderPaint.TextAlign = Paint.Align.Center;

                    float x = left + ((right - left) / 2) - DPUtils.ConvertDPToPx(2.5f);
                    float y = top - DPUtils.ConvertDPToPx(7f);
                    if (isZoom)
                    {
                        x = left + ((right - left) / 2) - DPUtils.ConvertDPToPx(6.5f);
                        y = top - DPUtils.ConvertDPToPx(18f);
                    }

                    c.DrawBitmap(missingBitmap, x, y, MRenderPaint);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        // Lin Siong Note: To draw the text on last bar and always show it
        /*private void DrawTextOnCanvas(Canvas c, float top, float left, float right, string currencyUnitTxt, double amount, string usageUnitTxt, double usage)
        {
            try
            {
                // Lin Siong Note: Set Render Paint to white with alpha 50
                // Lin Siong Note: Set Render Text to 10dp
                // Lin Siong Note: Set text to align center
                MRenderPaint.Color = new Color(255, 255, 255, 50);
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

                // Lin Siong Note: calculate the point that need the text to write on
                // Lin Siong Note: the x is on left point with offset which consist with right point - left point and divided by two
                // Lin Siong Note: the y is top point + 7dp 
                float x = left + ((right - left) / 2);
                float y = top - DPUtils.ConvertDPToPx(7f);

                // Lin Siong Note: to show either RM or kWh
                if (currentChartType == ChartType.Month)
                {
                    if (currentChartDataType == ChartDataType.RM)
                    {
                        float val = (float)amount;
                        string txt = currencyUnitTxt + " " + decimalFormat.Format(val);
                        c.DrawText(txt, x, y, MRenderPaint);
                    }
                    else if (currentChartDataType == ChartDataType.kWh)
                    {
                        float valKwh = (float)usage;
                        string txt = kwhFormat.Format(Math.Abs(valKwh)) + " " + usageUnitTxt;
                        c.DrawText(txt, x, y, MRenderPaint);
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }*/

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
namespace myTNB
{
    public static class ChartHelper
    {
        /// <summary>
        /// Updates the chart value for renewable account.
        /// </summary>
        /// <returns>The chart value for re.</returns>
        /// <param name="chartValue">Chart value.</param>
        public static double UpdateValueForRE(double chartValue)
        {
            if (chartValue < 0)
            {
                return chartValue * -1.0;
            }

            return 0;
        }
    }
}
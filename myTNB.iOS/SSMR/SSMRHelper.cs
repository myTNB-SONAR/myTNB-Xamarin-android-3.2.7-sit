using System;
namespace myTNB.SSMR
{
    public class SSMRHelper
    {
        /// <summary>
        /// Checks if description text is Estimated Reading
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsEstimatedReading(string text)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.ToLower().Contains(SSMRConstants.STR_EstimatedReading))
                {
                    res = true;
                }
            }
            return res;
        }
    }
}

using System;
using System.Threading;

namespace myTNB_Android.Src.Utils
{
    public class CancellationTokenSourceWrapper
    {
        private CancellationTokenSourceWrapper()
        {
        }
        public static bool isOvervoltageClaimPilotNonPilotTimeout;
        /// <summary>
        /// Gets the CancellationToken with default timeout
        /// </summary>
        /// <returns></returns>
        public static CancellationToken GetToken()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(isOvervoltageClaimPilotNonPilotTimeout ? 25000 : Constants.SERVICE_TIMEOUT_DEFAULT);
            return cancellationTokenSource.Token;
        }

        /// <summary>
        /// Gets the CancellationToken with defined timeout
        /// </summary>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public static CancellationToken GetTokenWithDelay(int millisecondsDelay)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(millisecondsDelay);
            return cancellationTokenSource.Token;
        }

        /// <summary>
        /// Gets the CancellationTokenSource instance with default timeout
        /// </summary>
        /// <returns></returns>
        public static CancellationTokenSource GetCancellationTokenSource()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(Constants.SERVICE_TIMEOUT_DEFAULT);
            return cancellationTokenSource;
        }
    }
}

using System;
using System.Threading;

namespace myTNB_Android.Src.Utils
{
    public class CancellationTokenSourceWrapper
    {
        private CancellationTokenSource cancellationTokenSource;
        private int DEFAULT_DELAY_MILLISECONDS = Constants.SERVICE_TIMEOUT_DEFAULT;

        public CancellationTokenSourceWrapper()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets the CancellationToken with default timeout
        /// </summary>
        /// <returns></returns>
        public CancellationToken GetToken()
        {
            this.cancellationTokenSource.CancelAfter(DEFAULT_DELAY_MILLISECONDS);
            return this.cancellationTokenSource.Token;
        }

        /// <summary>
        /// Gets the CancellationToken with defined timeout
        /// </summary>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public CancellationToken GetTokenWithDelay(int millisecondsDelay)
        {
            this.cancellationTokenSource.CancelAfter(millisecondsDelay);
            return this.cancellationTokenSource.Token;
        }

        /// <summary>
        /// Gets the CancellationTokenSource instance with default timeout
        /// </summary>
        /// <returns></returns>
        public CancellationTokenSource GetCancellationTokenSource()
        {
            this.cancellationTokenSource.CancelAfter(DEFAULT_DELAY_MILLISECONDS);
            return this.cancellationTokenSource;
        }
    }
}

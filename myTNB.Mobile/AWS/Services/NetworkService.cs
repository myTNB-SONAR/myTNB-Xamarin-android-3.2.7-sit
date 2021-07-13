using System.Threading;

namespace myTNB.Mobile.AWS
{
    public class NetworkService
    {
        public static CancellationToken GetCancellationToken(int? timeOut = null)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(timeOut == null
                ? AWSConstants.TimeOut
                : timeOut.Value);
            CancellationToken token = tokenSource.Token;
            return token;
        }
    }
}
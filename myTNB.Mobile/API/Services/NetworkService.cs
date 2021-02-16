using System.Threading;

namespace myTNB.Mobile.API
{
    public class NetworkService
    {
        public static CancellationToken GetCancellationToken()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(Constants.APITimeOut);
            CancellationToken token = tokenSource.Token;
            return token;
        }
    }
}
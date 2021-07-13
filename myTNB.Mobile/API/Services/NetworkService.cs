using System.Threading;

namespace myTNB.Mobile.API
{
    public class NetworkService
    {
        public static CancellationToken GetCancellationToken()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(MobileConstants.APITimeOut);
            CancellationToken token = tokenSource.Token;
            return token;
        }
    }
}
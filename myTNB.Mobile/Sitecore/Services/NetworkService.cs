using System.Threading;

namespace myTNB.Mobile.Sitecore
{
    public class NetworkService
    {
        public static CancellationToken GetCancellationToken()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(Constants.SitecoreTimeOut);
            CancellationToken token = tokenSource.Token;
            return token;
        }
    }
}
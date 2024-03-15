using Android.Content;
using Android.Net;
using Android.OS;

namespace myTNB.AndroidApp.Src.Utils
{
    public class ConnectionUtils
    {
        public static bool HasInternetConnection(Context context)
        {
            ConnectivityManager connectivity = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                Network[] networks = connectivity.GetAllNetworks();
                NetworkInfo networkInfo;
                foreach (Network mNetwork in networks)
                {
                    networkInfo = connectivity.GetNetworkInfo(mNetwork);
                    if (networkInfo.GetState().Equals(NetworkInfo.State.Connected))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (connectivity != null)
                {
                    //noinspection deprecation
                    NetworkInfo[] info = connectivity.GetAllNetworkInfo();
                    if (info != null)
                    {
                        foreach (NetworkInfo anInfo in info)
                        {
                            if (anInfo.GetState() == NetworkInfo.State.Connected)
                            {

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool ChceckInternetConnection(Context context)
        {
            ConnectivityManager connectivity = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeNetwork = connectivity.ActiveNetworkInfo;

            return activeNetwork != null &&
                      activeNetwork.IsConnectedOrConnecting;
        }
    }
}
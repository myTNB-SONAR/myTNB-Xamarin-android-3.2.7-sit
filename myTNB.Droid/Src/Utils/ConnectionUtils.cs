using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;

namespace myTNB_Android.Src.Utils
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
    }
}
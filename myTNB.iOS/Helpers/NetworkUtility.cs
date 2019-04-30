using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace myTNB
{
    public static class NetworkUtility
    {
        /// <summary>
        /// Is network reachable.
        /// </summary>
		public static bool isReachable = false;
        /// <summary>
        /// Checks if network available.
        /// </summary>
        /// <returns><c>true</c>, if network available was ised, <c>false</c> otherwise.</returns>
		public static bool IsNetworkAvailable()
        {
            bool isAvailable = CrossConnectivity.Current.IsConnected;
            Debug.WriteLine("[DEBUG] Network Availablity: " + isAvailable);
            return isAvailable;
        }
        /// <summary>
        /// Checks the connectivity.
        /// </summary>
        /// <returns>The connectivity.</returns>
		public static Task CheckConnectivity()
        {
            return Task.Factory.StartNew(() =>
            {
                isReachable = Reachability.IsHostReachable("http://google.com");
            });
        }
    }
}
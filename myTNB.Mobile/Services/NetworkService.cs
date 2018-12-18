using System;
using System.Threading;
using Refit;

namespace myTNB.Mobile.Services
{
    public class NetworkService
    {
        public static int ApiTimeout = 60000;

        public static IApiService apiService;

        /// <summary>
        /// Gets the API service.
        /// </summary>
        /// <returns>The API service.</returns>
        public static IApiService GetApiService()
        {
            apiService = RestService.For<IApiService>(Constants.ApiDomain);
            return apiService;
        }

        public static CancellationToken GetCancellationToken()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(ApiTimeout); 
            CancellationToken token = tokenSource.Token;
            return token;
        }
    }
}

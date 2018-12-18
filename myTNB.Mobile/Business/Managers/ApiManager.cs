using System;
using System.Diagnostics;
using System.Threading.Tasks;
using myTNB.Mobile.Model;
using myTNB.Mobile.Services;
using Refit;

namespace myTNB.Mobile.Business
{
    public sealed class ApiManager
    {
        private static readonly Lazy<ApiManager> lazy =
        new Lazy<ApiManager>(() => new ApiManager());

        public static ApiManager Instance { get { return lazy.Value; } }

        private ApiManager()
        {
        }

        /// <summary>
        /// Gets the phone verification status.
        /// </summary>
        /// <returns>The phone verification status.</returns>
        public async Task<PhoneVerificationStatusResponse> GetPhoneVerificationStatus(string email, string userId, 
            string deviceId)
        {
            var apiService = NetworkService.GetApiService();
            var requestParameter = new PhoneVerificationStatusRequest
            {
                ApiKeyId = Constants.ApiKeyId,
                SSPUserID = userId,
                Email = email,
                DeviceID = deviceId
            };


            try
            {
                return await apiService.GetPhoneVerifyStatus(Constants.ApiUrlPath, requestParameter, NetworkService.GetCancellationToken());
            }
            catch(ApiException apiEx)
            {
                //var content = apiEx.GetContentAs<Dictionary<string, string>>();
#if DEBUG
                Debug.WriteLine(apiEx.Message);
#endif
            }
            catch(Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }

            return new PhoneVerificationStatusResponse();
        }

    }
}

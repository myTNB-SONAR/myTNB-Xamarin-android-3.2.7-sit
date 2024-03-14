using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Android.Src.SSMR.SMRApplication.Api;
using myTNB.Android.Src.SSMRTerminate.MVP;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.SSMRTerminate.Api
{
    public class SSMRTerminateImpl : SSMRTerminateContract.SSMRTerminateApiPresenter
    {
        SSMRTerminateApi api = null;

        public SSMRTerminateImpl()
        {
#if DEBUG
            HttpClient httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<SSMRTerminateApi>(httpClient);
#else
            api = RestService.For<SSMRTerminateApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(GetRegisteredContactInfoRequest request)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return api.GetRegisteredContactInfo(encryptedRequest, new System.Threading.CancellationToken());
        }

        public Task<SMRTerminationReasonsResponse> GetSMRTerminationReasons(GetSMRTerminationReasonsRequest request)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return api.GetSMRTerminationReasons(encryptedRequest, new System.Threading.CancellationToken());
        }

        public Task<SMRregistrationSubmitResponse> SubmitSMRApplication(SubmitSMRApplicationRequest request)
        {
            var encryptedRequest = myTNB.Mobile.APISecurityManager.Instance.GetEncryptedRequest(request);
            return api.SubmitSMRApplication(encryptedRequest, new System.Threading.CancellationToken());
        }
    }
}

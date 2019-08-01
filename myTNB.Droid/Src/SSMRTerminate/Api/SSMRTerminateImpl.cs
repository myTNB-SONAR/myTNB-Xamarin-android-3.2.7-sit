using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.SSMR.SMRApplication.Api;
using myTNB_Android.Src.SSMRTerminate.MVP;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.SSMRTerminate.Api
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
            return api.GetRegisteredContactInfo(request, new System.Threading.CancellationToken());
        }

        public Task<SMRTerminationReasonsResponse> GetSMRTerminationReasons(GetSMRTerminationReasonsRequest request)
        {
            return api.GetSMRTerminationReasons(request, new System.Threading.CancellationToken());
        }
    }
}

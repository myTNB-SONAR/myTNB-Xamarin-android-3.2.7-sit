using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB.Android.Src.SSMRMeterHistory.Api;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.SSMR.SMRApplication.Api
{
    public class SMRregistrationApiImpl : SMRregistrationApi
    {
        SMRregistrationApi api = null;
        HttpClient httpClient = null;
        public SMRregistrationApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<SMRregistrationApi>(httpClient);
#else
            api = RestService.For<SMRregistrationApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] EncryptedRequest encryptedRequest)
        {
            return api.GetRegisteredContactInfo(encryptedRequest);
        }

        public Task<SMRregistrationSubmitResponse> SubmitSMRApplication([Body] EncryptedRequest encryptedRequest)
        {
            return api.SubmitSMRApplication(encryptedRequest);
        }

        public Task<GetAccountsSMREligibilityResponse> GetAccountsSMREligibility([Body] EncryptedRequest encryptedRequest)
        {
            return api.GetAccountsSMREligibility(encryptedRequest);
        }
    }
}

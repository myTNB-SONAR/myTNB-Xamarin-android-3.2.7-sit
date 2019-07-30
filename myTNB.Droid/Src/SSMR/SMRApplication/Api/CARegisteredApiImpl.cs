using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.SSMR.SMRApplication.Api
{
    public class CARegisteredApiImpl : ApplicationFormSMRContract.IApiNotification
    {
        CARegisteredContactInfoApi api = null;
        HttpClient httpClient = null;
        public CARegisteredApiImpl()
        {
#if DEBUG
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<CARegisteredContactInfoApi>(httpClient);
#else
            notificationApi = RestService.For<CARegisteredContactInfoApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<CARegisteredContactInfoResponse> GetCARegisteredContactInfo(object requestObject)
        {
            return api.GetRegisteredContactInfo(requestObject);
        }
    }
}

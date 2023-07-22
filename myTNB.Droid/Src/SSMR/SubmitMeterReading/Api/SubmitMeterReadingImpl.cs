using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Api
{
    public class SubmitMeterReadingImpl : SubmitMeterReadingApi
    {
        SubmitMeterReadingApi api = null;
        HttpClient httpClient = null;
        public SubmitMeterReadingImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<SubmitMeterReadingApi>(httpClient);
#else
            api = RestService.For<SubmitMeterReadingApi>(Constants.SERVER_URL.END_POINT);
#endif
        }
        public Task<SubmitMeterReadingResponse> SubmitSMRMeetingReading([Body] EncryptedRequest request)
        {
            return api.SubmitSMRMeetingReading(request);
        }

        public Task<GetMeterReadingOCRResponse> GetMeterReadingOCRValue([Body] myTNB_Android.Src.SSMR.SMRApplication.Api.BaseRequest request)
        {
            return api.GetMeterReadingOCRValue(request);
        }
    }
}

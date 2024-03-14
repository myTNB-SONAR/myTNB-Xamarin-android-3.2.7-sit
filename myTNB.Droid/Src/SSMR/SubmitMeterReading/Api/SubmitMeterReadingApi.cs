using System;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using Refit;

namespace myTNB.Android.Src.SSMR.SubmitMeterReading.Api
{
    public interface SubmitMeterReadingApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/SubmitSMRMeterReading")]
        Task<SubmitMeterReadingResponse> SubmitSMRMeetingReading([Body] EncryptedRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetMeterReadingOCRValue")]
        Task<GetMeterReadingOCRResponse> GetMeterReadingOCRValue([Body] EncryptedRequest request);
    }
}

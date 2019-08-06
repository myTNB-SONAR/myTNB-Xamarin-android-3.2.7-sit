﻿using System;
using System.Threading.Tasks;
using Refit;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Api
{
    public interface SubmitMeterReadingApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/SubmitSMRMeterReading")]
        Task<SubmitMeterReadingResponse> SubmitSMRMeetingReading([Body] myTNB_Android.Src.SSMR.SMRApplication.Api.BaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetMeterReadingOCRValue")]
        Task<GetMeterReadingOCRResponse> GetMeterReadingOCRValue([Body] myTNB_Android.Src.SSMR.SMRApplication.Api.BaseRequest request);
    }
}

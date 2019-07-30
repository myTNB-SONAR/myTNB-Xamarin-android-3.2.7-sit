﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.SSMR.SMRApplication.Api
{
    public class SMRregistrationApiImpl : SMRregistrationApi
    {
        SMRregistrationApi api = null;
        HttpClient httpClient = null;
        public SMRregistrationApiImpl()
        {
#if DEBUG
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<SMRregistrationApi>(httpClient);
#else
            notificationApi = RestService.For<CARegisteredContactInfoApi>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<CARegisteredContactInfoResponse> GetRegisteredContactInfo([Body] BaseRequest request)
        {
            return api.GetRegisteredContactInfo(request);
        }

        public Task<CARegisteredContactInfoResponse> SubmitSMRApplication([Body] BaseRequest request)
        {
            return api.SubmitSMRApplication(request);
        }
    }
}

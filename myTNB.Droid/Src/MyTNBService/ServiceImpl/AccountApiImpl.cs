﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.MyTNBService.ServiceImpl
{
    public class AccountApiImpl : IAccountAPI
    {
        IAccountAPI api;
        HttpClient httpClient;

        public AccountApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<IAccountAPI>(httpClient);
#else
            api = RestService.For<IBillingAPI>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<T> GetLanguagePreference<T>([Body] APIBaseRequest request)
        {
            return api.GetLanguagePreference<T>(request);
        }

        public Task<T> SaveLanguagePreference<T>([Body] APIBaseRequest request)
        {
            return api.SaveLanguagePreference<T>(request);
        }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB.Android.Src.Base.Request;
using myTNB.Android.Src.MyTNBService.InterfaceAPI;
using myTNB.Android.Src.Utils;
using Refit;

namespace myTNB.Android.Src.MyTNBService.ServiceImpl
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
            api = RestService.For<IAccountAPI>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<T> GetLanguagePreference<T>([Body] EncryptedRequest encryptedRequest)
        {
            return api.GetLanguagePreference<T>(encryptedRequest);
        }

        public Task<T> SaveLanguagePreference<T>([Body] EncryptedRequest encryptedRequest)
        {
            return api.SaveLanguagePreference<T>(encryptedRequest);
        }
    }
}

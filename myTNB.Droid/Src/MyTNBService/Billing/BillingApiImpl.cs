using System;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.MyTNBService.InterfaceAPI;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.MyTNBService.Billing
{
    public class BillingApiImpl : IBillingAPI
    {
        IBillingAPI api = null;
        HttpClient httpClient = null;
        public BillingApiImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<IBillingAPI>(httpClient);
#else
            api = RestService.For<IBillingAPI>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<T> GetAccountBillPayHistory<T>([Body] APIBaseRequest request)
        {
            return api.GetAccountBillPayHistory<T>(request);
        }

        public Task<T> GetPaymentReceipt<T>([Body] APIBaseRequest request)
        {
            return api.GetPaymentReceipt<T>(request);
        }

        public Task<T> GetPaymentTransactionId<T>([Body] APIBaseRequest request)
        {
            return api.GetPaymentTransactionId<T>(request);
        }
    }
}

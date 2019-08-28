using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.myTNBMenu.Api;
using myTNB_Android.Src.Utils;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API
{
    public class ItemisedBillingAPIImpl : ItemisedBillingAPI
    {
        ItemisedBillingAPI api = null;
        HttpClient httpClient = null;
        public ItemisedBillingAPIImpl()
        {
#if DEBUG || DEVELOP || SIT
            httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
            api = RestService.For<ItemisedBillingAPI>(httpClient);
#else
            api = RestService.For<ItemisedBillingAPI>(Constants.SERVER_URL.END_POINT);
#endif
        }

        public Task<AccountChargesResponse> GetAccountsCharges([Body] APIBaseRequest request)
        {
            return api.GetAccountsCharges(request);
        }

        public Task<AccountBillPayHistoryResponse> GetAccountBillPayHistory([Body] APIBaseRequest request)
        {
            return api.GetAccountBillPayHistory(request);
        }
    }
}

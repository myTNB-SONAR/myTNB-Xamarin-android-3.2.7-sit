using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Base.Response;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Fragments.ItemisedBillingMenu.API
{
    public interface ItemisedBillingAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsCharges")]
        Task<AccountChargesResponse> GetAccountsCharges([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountBillPayHistory")]
        Task<AccountBillPayHistoryResponse> GetAccountBillPayHistory([Body] APIBaseRequest request);
    }
}

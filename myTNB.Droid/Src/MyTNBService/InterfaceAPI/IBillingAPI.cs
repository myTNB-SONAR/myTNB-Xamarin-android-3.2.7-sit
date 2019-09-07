using System;
using System.Threading.Tasks;
using myTNB_Android.Src.Base.Request;
using Refit;

namespace myTNB_Android.Src.MyTNBService.InterfaceAPI
{
    public interface IBillingAPI
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountsCharges")]
        Task<T> GetAccountsCharges<T>([Body] APIBaseRequest request);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountBillPayHistory")]
        Task<T> GetAccountBillPayHistory<T>([Body] APIBaseRequest request);
    }
}

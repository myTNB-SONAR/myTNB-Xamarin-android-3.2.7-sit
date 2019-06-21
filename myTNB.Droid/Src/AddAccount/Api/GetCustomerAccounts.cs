using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface GetCustomerAccounts
    {

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetCustomerBillingAccountList")]
        Task<AccountResponseV5> GetCustomerAccountV5([Body] GetCustomerAccountsRequest getAccountRequest);

    }
}
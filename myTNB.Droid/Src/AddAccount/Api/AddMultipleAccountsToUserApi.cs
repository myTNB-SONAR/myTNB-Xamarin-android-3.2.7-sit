using myTNB_Android.Src.AddAccount.Models;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface AddMultipleAccountsToUserApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/AddMultipleSupplyAccountsToUserReg")]
        Task<AddMultipleAccountResponse> AddMultipleAccounts([Body] AddMultipleAccountRequest getAccountRequest);
    }
}
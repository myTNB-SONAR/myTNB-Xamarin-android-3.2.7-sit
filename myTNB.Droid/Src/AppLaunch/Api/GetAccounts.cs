using myTNB.Android.Src.AppLaunch.Models;
using myTNB.Android.Src.AppLaunch.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB.Android.Src.Base.Api
{
    public interface GetAccounts
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/GetAccountType")]
        Task<AccountTypesResponse> GetAccountTypes([Body] GetAccountTypesRequest getAccountRequest);
    }
}
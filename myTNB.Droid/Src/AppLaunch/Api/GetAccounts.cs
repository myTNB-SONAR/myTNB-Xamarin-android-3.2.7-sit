using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.AppLaunch.Requests;
using Refit;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.Base.Api
{
    public interface GetAccounts
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/GetAccountType")]
        Task<AccountTypesResponse> GetAccountTypes([Body] GetAccountTypesRequest getAccountRequest);
    }
}
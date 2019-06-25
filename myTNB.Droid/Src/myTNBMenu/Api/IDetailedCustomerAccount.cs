using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IDetailedCustomerAccount
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetBillingAccountDetails")]
        Task<AccountDetailsResponse> GetDetailedAccount([Body] AccountDetailsRequest accountDetailResponse, CancellationToken token);
    }
}
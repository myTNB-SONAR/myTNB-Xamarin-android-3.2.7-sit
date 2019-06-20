using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.AddAccount.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface AddAccountToCustomer
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/AddTNBAccountToUserReg")]
        Task<AddAccountResponse> AddAccountToCustomer([Body] AddAccountToCustomerRequest addAccountRequest, CancellationToken cancellationToken);
    }
}
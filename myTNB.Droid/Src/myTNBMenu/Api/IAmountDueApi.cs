using myTNB.Mobile.Business;
using myTNB.Android.Src.myTNBMenu.Models;
using myTNB.Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.Android.Src.myTNBMenu.Api
{
    public interface IAmountDueApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountDueAmount")]
        Task<AccountDueAmountResponse> GetAccountDueAmount([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}
using myTNB.Mobile.Business;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IAmountDueApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountDueAmount")]
        Task<AccountDueAmountResponse> GetAccountDueAmount([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}
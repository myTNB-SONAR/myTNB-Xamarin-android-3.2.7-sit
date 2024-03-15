using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.myTNBMenu.Models;
using myTNB.AndroidApp.Src.myTNBMenu.Requests;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Api
{
    public interface IAmountDueApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountDueAmount")]
        Task<AccountDueAmountResponse> GetAccountDueAmount([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}
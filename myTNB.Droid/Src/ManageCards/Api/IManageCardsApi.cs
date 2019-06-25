using myTNB_Android.Src.ManageCards.Models;
using myTNB_Android.Src.ManageCards.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ManageCards.Api
{
    public interface IManageCardsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RemoveRegisteredCard")]
        Task<RemoveRegisteredCardResponse> RemoveCard([Body] RemoveRegisteredCardRequest request, CancellationToken cancellationToken);

    }
}
using myTNB_Android.Src.ManageSupplyAccount.Models;
using myTNB_Android.Src.ManageSupplyAccount.Request;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB_Android.Src.ManageSupplyAccount.Api
{
    public interface IManageSupplyAccountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RemoveTNBAccountForUserFav")]
        Task<RemoveTNBAccountForUserFavResponse> RemoveTNBAccountForUserFav([Body] RemoveTNBAccountForUserFavRequest request, CancellationToken cancellationToken);

    }
}
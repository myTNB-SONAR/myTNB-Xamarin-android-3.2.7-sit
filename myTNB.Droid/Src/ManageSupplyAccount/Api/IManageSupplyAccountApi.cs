using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using System.Threading;
using myTNB_Android.Src.ManageSupplyAccount.Request;
using System.Threading.Tasks;
using myTNB_Android.Src.ManageSupplyAccount.Models;

namespace myTNB_Android.Src.ManageSupplyAccount.Api
{
    public interface IManageSupplyAccountApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RemoveTNBAccountForUserFav")]
        Task<RemoveTNBAccountForUserFavResponse> RemoveTNBAccountForUserFav([Body] RemoveTNBAccountForUserFavRequest request, CancellationToken cancellationToken);

    }
}
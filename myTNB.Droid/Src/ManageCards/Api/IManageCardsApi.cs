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
using myTNB_Android.Src.ManageCards.Request;
using System.Threading.Tasks;
using myTNB_Android.Src.ManageCards.Models;

namespace myTNB_Android.Src.ManageCards.Api
{
    public interface IManageCardsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/RemoveRegisteredCard")]
        Task<RemoveRegisteredCardResponse> RemoveCard([Body] RemoveRegisteredCardRequest request, CancellationToken cancellationToken);

    }
}
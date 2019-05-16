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
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using System.Threading.Tasks;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IAmountDueApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetAccountDueAmount")]
        //[Post("/v5/my_billingssp.asmx/GetAccountDueAmountV2")]
        Task<AccountDueAmountResponse> GetAccountDueAmount([Body] AccountDueAmountRequest request, CancellationToken cancellationToken);
    }
}
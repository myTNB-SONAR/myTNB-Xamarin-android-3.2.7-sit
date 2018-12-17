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
using myTNB_Android.Src.AddAccount.Models;
using System.Threading.Tasks;
using myTNB_Android.Src.AddAccount.Requests;
using System.Threading;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IDetailedCustomerAccount
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetBillingAccountDetails")]
        Task<AccountDetailsResponse> GetDetailedAccount([Body] AccountDetailsRequest accountDetailResponse , CancellationToken token);
    }
}
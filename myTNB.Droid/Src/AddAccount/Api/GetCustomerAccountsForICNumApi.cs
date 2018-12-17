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

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface GetCustomerAccountsForICNumApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetCustomerAccountsForICNum")]
        Task<BCRMAccountResponse> GetCustomerAccountByIc([Body] GetBCRMAccountRequest getAccountRequest);
    }
}
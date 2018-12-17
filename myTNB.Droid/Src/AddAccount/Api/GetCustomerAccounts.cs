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
using myTNB_Android.Src.AddAccount.Requests;
using Android.Accounts;
using System.Threading.Tasks;
using myTNB_Android.Src.AddAccount.Models;

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface GetCustomerAccounts
    {
        
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/GetCustomerBillingAccountList")]
        Task<AccountResponseV5> GetCustomerAccountV5([Body] GetCustomerAccountsRequest getAccountRequest);

    }
}
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
using myTNB_Android.Src.AppLaunch.Models;
using System.Threading.Tasks;
using Refit;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.AppLaunch.Requests;

namespace myTNB_Android.Src.Base.Api
{
    public interface GetAccounts
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/GetAccountType")]
        Task<AccountTypesResponse> GetAccountTypes([Body] GetAccountTypesRequest getAccountRequest);
    }
}
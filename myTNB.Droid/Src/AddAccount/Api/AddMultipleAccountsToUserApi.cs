﻿using myTNB_Android.Src.AddAccount.Models;
using Refit;
using System.Threading.Tasks;

namespace myTNB_Android.Src.AddAccount.Api
{
    public interface AddMultipleAccountsToUserApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/AddAccounts")]
        Task<AddMultipleAccountResponse> AddMultipleAccounts([Body] object getAccountRequest);
    }
}

﻿using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.MVP.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.myTNBMenu.Fragments.HomeMenu.Api
{
    public interface IGetIsSmrApplyAllowedApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetIsSmrApplyAllowed")]
        Task<GetIsSmrApplyAllowedResponse> GetIsSmrApplyAllowed([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}
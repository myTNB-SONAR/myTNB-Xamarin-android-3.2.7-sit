﻿using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;
using myTNB.Mobile.Business;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IGetInstallationDetailsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAccountStatus")]
        Task<GetInstallationDetailsResponse> GetInstallationDetails([Body] EncryptedRequest request, CancellationToken cancellationToken);
    }
}

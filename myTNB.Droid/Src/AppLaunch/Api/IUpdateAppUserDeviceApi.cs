using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using myTNB.AndroidApp.Src.AppLaunch.Requests;
using Refit;

namespace myTNB.AndroidApp.Src.AppLaunch.Api
{
    public interface IUpdateAppUserDeviceApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/UpdateAppUserDevices")]
        Task<UpdateAppUserDeviceResponse> UpdateAppUserDevice([Body] EncryptedRequest request, CancellationToken token);
    }
}
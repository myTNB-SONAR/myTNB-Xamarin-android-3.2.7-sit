using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB.Mobile.Business;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.AppLaunch.Requests;
using Refit;

namespace myTNB_Android.Src.AppLaunch.Api
{
    public interface IUpdateAppUserDeviceApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/my_billingssp.asmx/UpdateAppUserDevices")]
        Task<UpdateAppUserDeviceResponse> UpdateAppUserDevice([Body] EncryptedRequest request, CancellationToken token);
    }
}
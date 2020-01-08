using System;
using System.Threading;
using System.Threading.Tasks;
using myTNB_Android.Src.myTNBMenu.Requests;
using myTNB_Android.Src.myTNBMenu.Models;
using Refit;

namespace myTNB_Android.Src.myTNBMenu.Api
{
    public interface IGetInstallationDetailsApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v6/mytnbappws.asmx/GetAccountStatus")]
        Task<GetInstallationDetailsResponse> GetInstallationDetails([Body] GetInstallationDetailsRequest request, CancellationToken cancellationToken);
    }
}

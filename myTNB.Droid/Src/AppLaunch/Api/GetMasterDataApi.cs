using myTNB.Mobile.Business;
using myTNB.AndroidApp.Src.AppLaunch.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace myTNB.AndroidApp.Src.Base.Api
{
    public interface GetMasterDataApi
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v7/mytnbws.asmx/GetAppLaunchMasterData")]
        Task<MasterDataResponse> GetAppLaunchMasterData([Body] EncryptedRequest encryptedRequest, CancellationToken token);
    }
}